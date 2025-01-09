using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorridorFirstDungeonGenerator : AbstractDungeonGenerator
{
    // Serialized Fields from SimpleRandomWalkDungeonGenerator
    [SerializeField] public SimpleRandomWalkSO randomWalkParameters;
    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private GameObject npcShopPrefab;
    [SerializeField] private GameObject player;

    [SerializeField] private int torchPlacementFrequency = 5;
    [SerializeField] private int spawnerPlacementCount = 4;
    [SerializeField] private int numberOfChests = 3;
    [SerializeField] private int numberOfNormalNpcs = 1;
    [SerializeField] private int numberOfShopNpcs = 1;

    [SerializeField] private GameObject doorPrefab;
    [SerializeField] protected bool spawnDoorAtRandom = true; // Toggle for random placement or near player

    [SerializeField] private float enemyDistanceChangeType = 10f;
    [SerializeField] private int numberOfEnemyTypes = 4;

    // Fields from CorridorFirstDungeonGenerator
    [SerializeField] private int corridorLength = 14, corridorCount = 5;
    [SerializeField] [Range(0.1f, 1)] private float roomPercent = 0.8f;

    // Mode selector
    [SerializeField] private GenerationMode generationMode = GenerationMode.SimpleRandomWalk;
    public enum GenerationMode { SimpleRandomWalk, CorridorFirst }

    private Vector3 playerSpawnPoint;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
    private Vector3 doorSpawnedPosition;

    // Dictionary to track floor types (true = room, false = corridor)
    private Dictionary<Vector2Int, bool> floorTypes = new Dictionary<Vector2Int, bool>();

    protected override void RunProceduralGeneration()
    {
        ClearPreviousGeneration();
        occupiedPositions.Clear();
        floorTypes.Clear();

        HashSet<Vector2Int> floorPositions;

        switch (generationMode)
        {
            case GenerationMode.SimpleRandomWalk:
                floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
                foreach (var pos in floorPositions)
                {
                    floorTypes[pos] = true; // Treat all as rooms
                }
                break;
            case GenerationMode.CorridorFirst:
                var generationResult = CorridorFirstGeneration();
                floorPositions = generationResult.floorPositions;
                foreach (var pos in floorPositions)
                {
                    if (generationResult.roomPositions.Contains(pos))
                    {
                        floorTypes[pos] = true; // Room
                    }
                    else
                    {
                        floorTypes[pos] = false; // Corridor
                    }
                }
                break;
            default:
                floorPositions = new HashSet<Vector2Int>();
                break;
        }

        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        // Common placement methods
        PlacePlayer(floorPositions);
        PlaceTorches(floorPositions);
        PlaceChests(floorPositions);
        PlaceNpcs(floorPositions);

        if (spawnDoorAtRandom)
        {
            PlaceDoorAtRandomFloor(floorPositions);
        }
        else
        {
            PlaceDoorNextToPlayer();
        }

        SetupEnemySpawner(floorPositions);
    }

    // Methods from SimpleRandomWalkDungeonGenerator
    protected void PlaceDoorAtRandomFloor(HashSet<Vector2Int> floorPositions)
    {
        if (doorPrefab == null)
        {
            Debug.LogWarning("Door Prefab is not assigned!");
            return;
        }

        var floorList = floorPositions.ToList();
        Vector2Int doorPosition = GetAvailablePosition(floorList);

        if (doorPosition == Vector2Int.zero)
        {
            Debug.LogWarning("Failed to place the door. No valid floor position found.");
            return;
        }

        Vector3 worldPosition = new Vector3(doorPosition.x + 0.5f, doorPosition.y + 0.5f, 0);
        doorSpawnedPosition = worldPosition;
        GameObject door = Instantiate(doorPrefab, worldPosition, Quaternion.identity);
        door.name = "ProceduralDoor";

        occupiedPositions.Add(doorPosition);
        Debug.Log("Door placed randomly on the map.");
    }

    protected void PlaceDoorNextToPlayer()
    {
        if (doorPrefab == null)
        {
            Debug.LogWarning("Door Prefab is not assigned!");
            return;
        }

        Vector3 playerPosition = playerSpawnPoint;
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var direction in directions)
        {
            Vector2Int possiblePosition = new Vector2Int(
                Mathf.RoundToInt(playerPosition.x) + direction.x,
                Mathf.RoundToInt(playerPosition.y) + direction.y
            );

            if (!occupiedPositions.Contains(possiblePosition))
            {
                Vector3 worldPosition = new Vector3(possiblePosition.x + 0.5f, possiblePosition.y + 0.5f, 0);
                doorSpawnedPosition = worldPosition;
                GameObject door = Instantiate(doorPrefab, worldPosition, Quaternion.identity);
                door.name = "ProceduralDoor";

                occupiedPositions.Add(possiblePosition);
                Debug.Log("Door placed next to the player.");
                return;
            }
        }

        Debug.LogWarning("Failed to place the door next to the player. No valid adjacent tile found.");
    }

    private Vector2Int GetAvailablePosition(List<Vector2Int> floorList)
    {
        while (floorList.Count > 0)
        {
            int index = Random.Range(0, floorList.Count);
            Vector2Int position = floorList[index];

            if (!occupiedPositions.Contains(position))
            {
                floorList.RemoveAt(index);
                occupiedPositions.Add(position);
                return position;
            }

            floorList.RemoveAt(index);
        }

        Debug.LogWarning("No available positions left for placement!");
        return Vector2Int.zero;
    }

    protected void PlacePlayer(HashSet<Vector2Int> floorPositions)
    {
        var floorList = floorPositions.ToList();
        Vector2Int pos = GetAvailablePosition(floorList);
        player.transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
        playerSpawnPoint = player.transform.position;
    }

    protected void PlaceTorches(HashSet<Vector2Int> floorPositions)
    {
        var floorList = floorPositions.ToList();
        int counter = 0;
        var existingTorches = spawnedObjects.Where(obj => obj != null && obj.name.StartsWith("Torch")).ToList();
        int reuseCount = 0;

        foreach (var position in floorList.ToList())
        {
            if (counter % torchPlacementFrequency == 0)
            {
                if (occupiedPositions.Contains(position))
                    continue;

                Vector3 worldPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);

                if (reuseCount < existingTorches.Count)
                {
                    existingTorches[reuseCount].transform.position = worldPosition;
                    existingTorches[reuseCount].SetActive(true);
                    reuseCount++;
                }
                else
                {
                    var torch = Instantiate(torchPrefab, worldPosition, Quaternion.identity);
                    torch.name = "Torch";
                    spawnedObjects.Add(torch);
                }

                occupiedPositions.Add(position);
            }
            counter++;
        }

        for (int i = reuseCount; i < existingTorches.Count; i++)
        {
            existingTorches[i].SetActive(false);
        }
    }

    protected void PlaceChests(HashSet<Vector2Int> floorPositions)
    {
        var floorList = floorPositions.ToList();
        int chestsPlaced = 0;

        while (chestsPlaced < numberOfChests && floorList.Count > 0)
        {
            Vector2Int position = GetAvailablePosition(floorList);
            if (position == Vector2Int.zero) break;

            if (floorTypes.ContainsKey(position) && floorTypes[position]) // Only place in rooms
            {
                Vector3 chestPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
                var chest = Instantiate(chestPrefab, chestPosition, Quaternion.identity);
                chest.name = "Chest";
                spawnedObjects.Add(chest);
                chestsPlaced++;
            }
        }
    }

    protected void PlaceNpcs(HashSet<Vector2Int> floorPositions)
    {
        var floorList = floorPositions.ToList();
        int normalNpcsPlaced = 0;
        int shopNpcsPlaced = 0;

        while (normalNpcsPlaced < numberOfNormalNpcs && floorList.Count > 0)
        {
            Vector2Int position = GetAvailablePosition(floorList);
            if (position == Vector2Int.zero) break;

            if (floorTypes.ContainsKey(position) && floorTypes[position]) // Only place in rooms
            {
                Vector3 npcPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
                var npc = Instantiate(npcPrefab, npcPosition, Quaternion.identity);
                spawnedObjects.Add(npc);
                normalNpcsPlaced++;
            }
        }

        while (shopNpcsPlaced < numberOfShopNpcs && floorList.Count > 0)
        {
            Vector2Int position = GetAvailablePosition(floorList);
            if (position == Vector2Int.zero) break;

            if (floorTypes.ContainsKey(position) && floorTypes[position]) // Only place in rooms
            {
                Vector3 npcPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
                var npc = Instantiate(npcShopPrefab, npcPosition, Quaternion.identity);
                spawnedObjects.Add(npc);
                shopNpcsPlaced++;
            }
        }
    }

    protected void SetupEnemySpawner(HashSet<Vector2Int> floorPositions)
    {
        if (enemySpawner == null) return;

        var spawnerScript = enemySpawner.GetComponent<EenemySpawner>();
        if (spawnerScript == null) return;

        ConfigureSpawnPoints(enemySpawner, spawnerScript, floorPositions);

        // Calculate door position and spawn boss near the door
        Vector3 doorPosition = GetDoorWorldPosition();
        spawnerScript.SpawnBoss(doorSpawnedPosition);
    }

    private Vector3 GetDoorWorldPosition()
    {
        if (doorPrefab == null)
        {
            Debug.LogWarning("Door prefab is not assigned!");
            return Vector3.zero;
        }

        if (doorPrefab != null)
        {
            return doorPrefab.transform.position; // Access the Transform from the GameObject
        }

        return Vector3.zero;
    }

    private void ConfigureSpawnPoints(GameObject spawnerInstance, EenemySpawner spawnerScript, HashSet<Vector2Int> floorPositions)
    {
        var spawnPoints = spawnerScript.spawnPoints;
        spawnPoints.Clear();

        var floorList = floorPositions.ToList();
        int childCount = spawnerInstance.transform.childCount;

        for (int i = 0; i < spawnerPlacementCount && floorList.Count > 0; i++)
        {
            Vector2Int position = GetAvailablePosition(floorList);
            if (position == Vector2Int.zero) break;

            if (floorTypes.ContainsKey(position) && floorTypes[position]) // Only place in rooms
            {
                Vector3 worldPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);

                GameObject spawnPoint = i < childCount
                    ? ReuseSpawnPoint(spawnerInstance.transform.GetChild(i).gameObject, worldPosition)
                    : CreateNewSpawnPoint(spawnerInstance, i, worldPosition);

                EnemySpawnPoint sp;
                sp.spawnPoint = spawnPoint;
                float distance = Vector3.Distance(spawnPoint.transform.position, playerSpawnPoint);
                int type = Math.Min((int)(distance / enemyDistanceChangeType), numberOfEnemyTypes - 1);
                sp.type = type;
                spawnPoints.Add(sp);
            }
        }

        DisableExtraSpawnPoints(spawnerInstance, spawnerPlacementCount);
    }

    private GameObject ReuseSpawnPoint(GameObject spawnPoint, Vector3 worldPosition)
    {
        spawnPoint.transform.position = worldPosition;
        spawnPoint.SetActive(true);
        return spawnPoint;
    }

    private GameObject CreateNewSpawnPoint(GameObject spawnerInstance, int index, Vector3 worldPosition)
    {
        GameObject spawnPoint = new GameObject($"SpawnPoint_{index}");
        spawnPoint.transform.position = worldPosition;
        spawnPoint.transform.parent = spawnerInstance.transform;
        return spawnPoint;
    }

    private void DisableExtraSpawnPoints(GameObject spawnerInstance, int activeCount)
    {
        int childCount = spawnerInstance.transform.childCount;
        for (int i = activeCount; i < childCount; i++)
        {
            spawnerInstance.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    protected void ClearPreviousGeneration()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(obj);
#else
                Destroy(obj);
#endif
            }
        }
        spawnedObjects.Clear();
        floorTypes.Clear();
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }

    // Methods from CorridorFirstDungeonGenerator
    private (HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> roomPositions) CorridorFirstGeneration()
    {
        HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(corridorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(corridorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        floorPositions.UnionWith(corridorPositions);
        floorPositions.UnionWith(roomPositions);

        return (floorPositions, roomPositions);
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (!roomFloors.Contains(position))
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
                foreach (var pos in room)
                {
                    floorTypes[pos] = true; // Mark as room
                }
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
            foreach (var pos in roomFloor)
            {
                floorTypes[pos] = true; // Mark as room
            }
        }
        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        floorTypes[currentPosition] = false; // Mark as corridor

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
            foreach (var pos in corridor)
            {
                floorTypes[pos] = false; // Mark as corridor
            }
        }
    }
}