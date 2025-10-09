using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public int pointsOnDeath = 100;
    public float moveSpeed = 2f;

    [Header("Death Visuals")]
    public Sprite aliveSprite;
    public Sprite deadSprite;
    public float deathDuration = 0.5f;   // how long the dead sprite stays visible

    [Header("Game Over Triggers")]
    public float loseY = -4.5f;          // A) if enemy goes below this y, game over

    private enum State { Alive, Dying }
    private State state = State.Alive;

    private SpriteRenderer sr;
    private Collider2D col;
    private float deathTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (aliveSprite) sr.sprite = aliveSprite;
    }

    void Update()
    {
        if (state == State.Alive)
        {
            // Move down
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);

            // A) Game over if we pass the bottom line
            if (transform.position.y <= loseY)
            {
                GameManager.Instance?.GameOver();
                // Optional: stop this enemy from moving further
                state = State.Dying;    // freeze logic
                col.enabled = false;
                return;
            }
        }
        else if (state == State.Dying)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0f)
                Destroy(gameObject);
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

        // B) Player collision → Game Over
        if (other.CompareTag("Player"))
        {
            GameManager.Instance?.GameOver();
            // Optionally also disable the player here; GameManager will usually handle it.
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

        // Swap to dead sprite
        if (deadSprite) sr.sprite = deadSprite;

        // Stop further hits
        if (col) col.enabled = false;

        // Add score
        GameManager.Instance?.AddScore(pointsOnDeath);

        // Show corpse for a short time
        deathTimer = deathDuration;
    }

    void OnBecameInvisible()
    {
        // Clean up only if still alive (don’t insta-kill the corpse)
        if (state == State.Alive)
            Destroy(gameObject);
    }
}
