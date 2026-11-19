using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("Rock")]
    [SerializeField] private GameObject rockPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxRocks = 200;

    [Header("Reference")]
    [SerializeField] private CircleAI circleAI;

    private readonly List<GameObject> spawnedRocks = new();

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Stop kalau level selesai
            if (circleAI != null && circleAI.hasFinished)
                yield break;

            // Hapus reference batu yang sudah hancur
            spawnedRocks.RemoveAll(r => r == null);

            if (spawnedRocks.Count < maxRocks)
            {
                SpawnRock();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRock()
    {
        if (spawnPoints.Length == 0 || rockPrefab == null)
            return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject rock = Instantiate(
            rockPrefab,
            point.position,
            point.rotation);

        spawnedRocks.Add(rock);
    }

    
}