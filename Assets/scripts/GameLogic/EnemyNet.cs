using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(NetworkObject))]
public class EnemyNet : NetworkBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public int pointsOnDeath = 100;
    public float moveSpeed = 2f;

    [Header("Game Over Triggers")]
    public float loseY = -4.5f;

    private enum State { Alive, Dying }
    private State state = State.Alive;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!IsServer || state != State.Alive) return;

        // Move down
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);

        // Game over if pass the bottom line
        if (transform.position.y <= loseY)
        {
            GameManagerNet.Instance?.GameOverServerRpc();
            state = State.Dying;
            col.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer || state != State.Alive) return;

        // Bullet hit
        var bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(1);
            Destroy(other.gameObject); // bullet is local, safe to destroy
            return;
        }

        // Player collision → Game Over
        if (other.CompareTag("Player"))
        {
            GameManagerNet.Instance?.GameOverServerRpc();
            state = State.Dying;
            col.enabled = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (state != State.Alive) return;

        health -= amount;
        if (health <= 0)
            Die();
    }

    void Die()
    {
        state = State.Dying;
        if (col) col.enabled = false;

        GameManagerNet.Instance?.AddScoreServerRpc(pointsOnDeath);

        if (IsServer && IsSpawned)
            GetComponent<NetworkObject>().Despawn();
    }
}
