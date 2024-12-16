using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] public SimpleRandomWalkSO randomWalkParameters;
    [SerializeField] private GameObject torchPrefab;    // Torch prefab
    [SerializeField] private GameObject enemySpawner;   // Single EnemySpawner prefab
    [SerializeField] private GameObject chestPrefab;    // Chest prefab
    [SerializeField] private GameObject player;

    [SerializeField] private int torchPlacementFrequency = 5; // Place a torch every 5 tiles
    [SerializeField] private int spawnerPlacementCount = 4;   // Number of spawn points
    [SerializeField] private int numberOfChests = 3;          // Number of chests to place

    private List<GameObject> spawnedObjects = new List<GameObject>(); // Track all spawned objects

    protected override void RunProceduralGeneration()
    {
        ClearPreviousGeneration();

        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);

        // Clear and paint floor tiles
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);

        // Generate walls
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        PlacePlayer(floorPositions);

        // Place torches
        PlaceTorches(floorPositions);

        // Set up spawn points for the single EnemySpawner
        SetupEnemySpawner(floorPositions);

        // Place chests
        PlaceChests(floorPositions);
    }

    protected void PlacePlayer(HashSet<Vector2Int> floorPositions)
    {
        var floorList = floorPositions.ToList();
        Vector3 pos = GetRandomSpawnPosition(floorList);
        player.transform.position = pos;
    }

    protected void PlaceTorches(HashSet<Vector2Int> floorPositions)
    {
        int counter = 0;

        // Find all existing torches to reuse
        var existingTorches = spawnedObjects.Where(obj => obj != null && obj.name.StartsWith("Torch")).ToList();
        int reuseCount = 0;

        foreach (var position in floorPositions)
        {
            // Ensure torches are placed only on floor tiles
            if (counter % torchPlacementFrequency == 0)
            {
                Vector3 worldPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0); // Adjusted to center on tile

                if (reuseCount < existingTorches.Count)
                {
                    // Reuse an existing torch
                    existingTorches[reuseCount].transform.position = worldPosition;
                    existingTorches[reuseCount].SetActive(true);
                    reuseCount++;
                }
                else
                {
                    // Instantiate a new torch if needed
                    var torch = Instantiate(torchPrefab, worldPosition, Quaternion.identity);
                    torch.name = "Torch"; // Ensure no "(Clone)" suffix
                    spawnedObjects.Add(torch);
                }
            }
            counter++;
        }

        // Disable unused torches if any
        for (int i = reuseCount; i < existingTorches.Count; i++)
        {
            existingTorches[i].SetActive(false);
        }
    }

    protected void PlaceChests(HashSet<Vector2Int> floorPositions)
    {
        // Convert floor positions to a list for easier indexing
        var floorList = floorPositions.ToList();

        // Keep track of how many chests are placed
        int chestsPlaced = 0;

        while (chestsPlaced < numberOfChests && floorList.Count > 0)
        {
            // Get a random position from the floor list
            Vector3 chestPosition = GetRandomSpawnPosition(floorList);

            // Instantiate a chest prefab at this position
            var chest = Instantiate(chestPrefab, chestPosition, Quaternion.identity);
            chest.name = "Chest"; // Name the chest without "(Clone)" suffix

            // Add the chest to the spawned objects list
            spawnedObjects.Add(chest);

            chestsPlaced++;
        }
    }

    protected void SetupEnemySpawner(HashSet<Vector2Int> floorPositions)
    {
        // Exit early if no enemy spawner is defined
        if (enemySpawner == null) return;

        var spawnerScript = enemySpawner.GetComponent<EenemySpawner>();
        if (spawnerScript == null) return;

        ConfigureSpawnPoints(enemySpawner, spawnerScript, floorPositions);
    }

    private void ConfigureSpawnPoints(GameObject spawnerInstance, EenemySpawner spawnerScript, HashSet<Vector2Int> floorPositions)
    {
        var spawnPoints = spawnerScript.spawnPoints;
        spawnPoints.Clear(); // Reset existing spawn points

        var floorList = floorPositions.ToList();
        int childCount = spawnerInstance.transform.childCount;

        for (int i = 0; i < spawnerPlacementCount && floorList.Count > 0; i++)
        {
            Vector3 worldPosition = GetRandomSpawnPosition(floorList);

            GameObject spawnPoint = i < childCount
                ? ReuseSpawnPoint(spawnerInstance.transform.GetChild(i).gameObject, worldPosition)
                : CreateNewSpawnPoint(spawnerInstance, i, worldPosition);

            spawnPoints.Add(spawnPoint);
        }

        DisableExtraSpawnPoints(spawnerInstance, spawnerPlacementCount);
    }

    private Vector3 GetRandomSpawnPosition(List<Vector2Int> floorList)
    {
        int index = Random.Range(0, floorList.Count);
        Vector2Int spawnPosition = floorList[index];
        floorList.RemoveAt(index);
        return new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0); // Centered position
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
        // Destroy all previously spawned objects and clear the list
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(obj); // Editor mode
#else
                Destroy(obj); // Play mode
#endif
            }
        }
        spawnedObjects.Clear();
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
}
