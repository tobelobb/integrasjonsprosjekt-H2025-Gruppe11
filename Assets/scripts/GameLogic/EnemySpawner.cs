using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Area (X range)")]
    public float minX = -7.5f;
    public float maxX = 7.5f;

    [Header("Spawn Height")]
    public float spawnY = 6.5f;     // a bit above the top of the camera view

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public float spawnRate = 1.0f;  // enemies per second
    public float difficultyRamp = 0.98f; // each spawn reduces interval a bit (0.98 ~ 2% faster)

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        float interval = 1f / Mathf.Max(0.01f, spawnRate);

        while (true)
        {
            SpawnOne();
            yield return new WaitForSeconds(interval);
            interval *= difficultyRamp; // slowly speed up spawns
            interval = Mathf.Max(0.25f, interval); // clamp: don’t go too fast
        }
    }

    void SpawnOne()
    {
        if (!enemyPrefab) return;

        float x = Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x, spawnY, 0f);

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}
