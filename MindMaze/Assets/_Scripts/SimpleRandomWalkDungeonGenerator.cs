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
    [SerializeField] private GameObject enemySpawner;  // Single EnemySpawner prefab
    [SerializeField] private int torchPlacementFrequency = 5; // Place a torch every 5 tiles
    [SerializeField] private int spawnerPlacementCount = 4; // Number of spawn points

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

        // Place torches
        PlaceTorches(floorPositions);

        // Set up spawn points for the single EnemySpawner
        SetupEnemySpawner(floorPositions);
    }

    private void PlaceTorches(HashSet<Vector2Int> floorPositions)
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

    private void SetupEnemySpawner(HashSet<Vector2Int> floorPositions)
    {
        // Ensure the EnemySpawner exists
        if (enemySpawner == null) return;

        // Check if a spawner instance already exists
        var existingSpawner = spawnedObjects.FirstOrDefault(obj => obj != null && obj.name == "EnemySpawner");

        GameObject spawnerInstance;
        if (existingSpawner != null)
        {
            spawnerInstance = existingSpawner;
        }
        else
        {
            // Instantiate the spawner and track it
            spawnerInstance = Instantiate(enemySpawner, Vector3.zero, Quaternion.identity);
            spawnerInstance.name = "EnemySpawner"; // Ensure no "(Clone)" suffix
            spawnedObjects.Add(spawnerInstance);
        }

        var spawnerScript = spawnerInstance.GetComponent<EenemySpawner>();
        if (spawnerScript == null) return;

        // Set up spawn points
        var spawnPoints = spawnerScript.spawnPoints; // Reference existing spawn points list
        spawnPoints.Clear(); // Clear existing points if any

        var floorList = floorPositions.ToList();

        for (int i = 0; i < spawnerPlacementCount; i++)
        {
            if (floorList.Count == 0) break;

            // Randomly select a valid floor tile
            Vector2Int spawnPosition = floorList[Random.Range(0, floorList.Count)];
            Vector3 worldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0); // Adjusted to center on tile

            // Check if we already have a child spawn point, or create one if needed
            GameObject spawnPoint;
            if (i < spawnerInstance.transform.childCount)
            {
                spawnPoint = spawnerInstance.transform.GetChild(i).gameObject;
                spawnPoint.transform.position = worldPosition; // Update position
                spawnPoint.SetActive(true);
            }
            else
            {
                // Create a new spawn point under the spawner
                spawnPoint = new GameObject($"SpawnPoint_{i}");
                spawnPoint.transform.position = worldPosition;
                spawnPoint.transform.parent = spawnerInstance.transform; // Parent to spawner
            }

            spawnPoints.Add(spawnPoint); // Add to spawn points list
            floorList.Remove(spawnPosition); // Avoid duplicate spawn points
        }

        // Disable unused spawn points if any
        for (int i = spawnerPlacementCount; i < spawnerInstance.transform.childCount; i++)
        {
            spawnerInstance.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ClearPreviousGeneration()
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
