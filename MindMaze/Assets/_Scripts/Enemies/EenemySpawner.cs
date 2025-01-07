using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EenemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyList = new GameObject[4];
    [SerializeField]
    public List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
    [SerializeField]
    private int count = 20;
    [SerializeField]
    private float minDelay = 0.8f, maxDelay = 1.5f;

    IEnumerator SpawnCoroutine()
    {
        while (count > 0)
        {
            count--;
            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogError("No spawn points available!");
                yield break;
            }

            var randomIndex = Random.Range(0, spawnPoints.Count);
            var spawnPoint = spawnPoints[randomIndex];

            if (spawnPoint.spawnPoint != null)
            {
                SpawnEnemy(spawnPoint);
            }
            else
            {
                Debug.LogError("Spawn point GameObject is null!");
            }

            var randomTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(randomTime);
        }
    }

    private void SpawnEnemy(EnemySpawnPoint spawnPoint)
    {
        if (spawnPoint.spawnPoint == null)
        {
            Debug.LogError("Spawn point GameObject is null!");
            return;
        }

        GameObject enemyPrefab = enemyList[spawnPoint.type % enemyList.Length];
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned!");
            return;
        }
        Debug.Log("enemy name : "+enemyPrefab.name);
        Instantiate(
            enemyPrefab,
            spawnPoint.spawnPoint.transform.position,
            Quaternion.identity
        );
    }

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("Spawn points are not assigned or empty!");
            return;
        }

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.spawnPoint == null)
            {
                Debug.LogError("A spawn point GameObject is missing in the spawnPoints list!");
                continue;
            }
            SpawnEnemy(spawnPoint);
        }

        StartCoroutine(SpawnCoroutine());
    }
}

[Serializable]
public struct EnemySpawnPoint
{
    public int type;
    public GameObject spawnPoint;
}
