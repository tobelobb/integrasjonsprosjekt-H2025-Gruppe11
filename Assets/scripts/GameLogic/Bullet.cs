using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 3f;

    private float timer;

    void Update()
    {
        // Multiplayer → only server moves bullets
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            if (!IsServer) return;
        }

        // Singleplayer → always move
        timer += Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (timer >= lifetime)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                if (IsServer && IsSpawned)
                    GetComponent<NetworkObject>().Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
