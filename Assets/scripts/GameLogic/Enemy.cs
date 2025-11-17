using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public int pointsOnDeath = 100;
    public float moveSpeed = 2f;

    [Header("Game Over Triggers")]
    public float loseY = -4.5f;   // if enemy goes below this y, game over

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
        if (state == State.Alive)
        {
            // Move down
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);

            // Game over if pass the bottom line
            if (transform.position.y <= loseY)
            {
                GameManager.Instance?.GameOver();
                state = State.Dying;
                col.enabled = false;
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state != State.Alive) return;

        // Bullet hit
        var bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(1);
            Destroy(other.gameObject);
            return;
        }

        // Player collision → Game Over
        if (other.CompareTag("Player"))
        {
            GameManager.Instance?.GameOver();
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

        // Stop further hits
        if (col) col.enabled = false;

        // Add score
        GameManager.Instance?.AddScore(pointsOnDeath);

        // Destroy
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        if (state == State.Alive)
            Destroy(gameObject);
    }
}
