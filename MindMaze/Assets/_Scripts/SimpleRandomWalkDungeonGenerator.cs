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
        foreach (var position in floorPositions)
        {
            // Ensure torches are placed only on floor tiles
            if (counter % torchPlacementFrequency == 0)
            {
                Vector3 worldPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0); // Adjusted to center on tile
                var torch = Instantiate(torchPrefab, worldPosition, Quaternion.identity);
                spawnedObjects.Add(torch); // Track for cleanup
            }
            counter++;
        }
    }

    private void SetupEnemySpawner(HashSet<Vector2Int> floorPositions)
    {
        // Ensure the EnemySpawner exists
        if (enemySpawner == null) return;

        // Instantiate the spawner and track it
        var spawnerInstance = Instantiate(enemySpawner, Vector3.zero, Quaternion.identity);
        spawnedObjects.Add(spawnerInstance);

        var spawnerScript = spawnerInstance.GetComponent<EenemySpawner>();
        if (spawnerScript == null) return;

        // Set up spawn points
        var spawnPoints = new List<GameObject>();
        var floorList = floorPositions.ToList();

        for (int i = 0; i < spawnerPlacementCount; i++)
        {
            if (floorList.Count == 0) break;

            // Randomly select a valid floor tile
            Vector2Int spawnPosition = floorList[Random.Range(0, floorList.Count)];
            Vector3 worldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0); // Adjusted to center on tile

            // Create an empty GameObject for the spawn point
            var spawnPoint = new GameObject($"SpawnPoint_{i}");
            spawnPoint.transform.position = worldPosition;
            spawnPoints.Add(spawnPoint);
            spawnedObjects.Add(spawnPoint); // Track for cleanup

            // Remove the position to avoid duplicates
            floorList.Remove(spawnPosition);
        }

        // Assign spawn points to the spawner
        spawnerScript.spawnPoints = spawnPoints;
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
