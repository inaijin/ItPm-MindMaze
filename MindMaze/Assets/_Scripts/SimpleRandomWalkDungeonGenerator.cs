using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] public SimpleRandomWalkSO randomWalkParameters;
    [SerializeField] private GameObject torchPrefab;
    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private int torchPlacementFrequency = 5;
    [SerializeField] private int spawnerPlacementCount = 4;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private GameObject lightParent;

    protected override void RunProceduralGeneration()
    {
        ClearPreviousGeneration();

        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);

        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        SetupLightParent();

        PlaceTorches(floorPositions);
        SetupEnemySpawner(floorPositions);
    }

    private void SetupLightParent()
    {
        if (lightParent == null)
        {
            lightParent = GameObject.Find("Lights");
            if (lightParent == null)
            {
                lightParent = new GameObject("Lights");
            }
        }

        foreach (Transform child in lightParent.transform)
        {
#if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }
    }

    private void PlaceTorches(HashSet<Vector2Int> floorPositions)
    {
        int counter = 0;
        var existingTorches = spawnedObjects.Where(obj => obj != null && obj.name.StartsWith("Torch")).ToList();
        int reuseCount = 0;

        foreach (var position in floorPositions)
        {
            if (counter % torchPlacementFrequency == 0 && IsAdjacentToWall(position, floorPositions))
            {
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
                    torch.transform.parent = lightParent.transform;
                    spawnedObjects.Add(torch);
                }
            }
            counter++;
        }

        for (int i = reuseCount; i < existingTorches.Count; i++)
        {
            existingTorches[i].SetActive(false);
        }
    }

    private bool IsAdjacentToWall(Vector2Int position, HashSet<Vector2Int> floorPositions)
    {
        var directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var direction in directions)
        {
            if (!floorPositions.Contains(position + direction))
            {
                return true;
            }
        }
        return false;
    }

    private void SetupEnemySpawner(HashSet<Vector2Int> floorPositions)
    {
        if (enemySpawner == null) return;

        var existingSpawner = spawnedObjects.FirstOrDefault(obj => obj != null && obj.name == "EnemySpawner");

        GameObject spawnerInstance;
        if (existingSpawner != null)
        {
            spawnerInstance = existingSpawner;
        }
        else
        {
            spawnerInstance = Instantiate(enemySpawner, Vector3.zero, Quaternion.identity);
            spawnerInstance.name = "EnemySpawner";
            spawnedObjects.Add(spawnerInstance);
        }

        var spawnerScript = spawnerInstance.GetComponent<EenemySpawner>();
        if (spawnerScript == null) return;

        var spawnPoints = spawnerScript.spawnPoints;
        spawnPoints.Clear();

        var floorList = floorPositions.ToList();

        for (int i = 0; i < spawnerPlacementCount; i++)
        {
            if (floorList.Count == 0) break;

            Vector2Int spawnPosition = floorList[Random.Range(0, floorList.Count)];
            if (IsAdjacentToWall(spawnPosition, floorPositions)) continue;

            Vector3 worldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0);

            GameObject spawnPoint;
            if (i < spawnerInstance.transform.childCount)
            {
                spawnPoint = spawnerInstance.transform.GetChild(i).gameObject;
                spawnPoint.transform.position = worldPosition;
                spawnPoint.SetActive(true);
            }
            else
            {
                spawnPoint = new GameObject($"SpawnPoint_{i}");
                spawnPoint.transform.position = worldPosition;
                spawnPoint.transform.parent = spawnerInstance.transform;
            }

            spawnPoints.Add(spawnPoint);
            floorList.Remove(spawnPosition);
        }

        for (int i = spawnerPlacementCount; i < spawnerInstance.transform.childCount; i++)
        {
            spawnerInstance.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ClearPreviousGeneration()
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
