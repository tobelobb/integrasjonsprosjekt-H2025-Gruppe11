using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawnerNet : NetworkBehaviour
{
    [Header("Spawn Area (X range)")]
    public float minX = -7.5f;
    public float maxX = 7.5f;

    [Header("Spawn Height")]
    public float spawnY = 6.5f; // a bit above the top of the camera view

    [Header("Spawning")]
    public GameObject enemyPrefab;
    public float spawnRate = 1.0f;       // enemies per second
    public float difficultyRamp = 0.98f; // each spawn reduces interval a bit (0.98 ~ 2% faster)

    private bool isGameOver = false;
    private Coroutine spawnRoutine;

    // Do NOT auto-start spawning when host starts.
    public override void OnNetworkSpawn()
    {
        // Only the server can spawn enemies, but wait until Start button triggers BeginSpawningServerRpc.
    }

    /// <summary>
    /// Called by the Start button in MultiplayerUI to begin spawning.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void BeginSpawningServerRpc()
    {
        if (!IsServer) return;
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        float interval = 1f / Mathf.Max(0.01f, spawnRate);

        while (!isGameOver)
        {
            SpawnOne();
            yield return new WaitForSeconds(interval);

            // ramp difficulty
            interval *= difficultyRamp;
            interval = Mathf.Max(0.25f, interval); // clamp: don’t go too fast
        }
    }

    void SpawnOne()
    {
        if (!enemyPrefab) return;

        float x = Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x, spawnY, 0f);

        var enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // Spawn as a networked object so all clients see it
        var netObj = enemy.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogWarning("Enemy prefab missing NetworkObject component!");
        }
    }

    // Called by GameManager when game ends
    public void StopSpawning()
    {
        isGameOver = true;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }
}
