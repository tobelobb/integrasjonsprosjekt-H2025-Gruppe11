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
        if (!IsServer) return;

        timer += Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (timer >= lifetime && IsSpawned)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
