using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2.5f;        // downward speed

    [Header("Gameplay")]
    public int pointsOnDeath = 100;   // score reward

    [Header("SFX (optional)")]
    public AudioClip deathSfx;
    [Range(0f, 1f)] public float deathVolume = 0.8f;

    void Update()
    {
        // moving straight down
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }

    // If it leaves the camera view (below), clean up
    void OnBecameInvisible()
    {
        // only destroy if it actually went off-screen
        Destroy(gameObject);
    }

    // Bullet hits are triggers
    void OnTriggerEnter2D(Collider2D other)
    {
        // Did a bullet hit us? (Don’t rely on tags; check for the Bullet script)
        if (other.GetComponent<Bullet>() != null)
        {
            Destroy(other.gameObject);    // remove the bullet
            Die();
        }
        // (Optional) If you want to hurt the player on contact later, check for a Player component/tag here.
    }

    void Die()
    {
        // award score
        GameManager.Instance?.AddScore(pointsOnDeath);

        // death-sound when death
        if (deathSfx != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(deathSfx, Camera.main.transform.position, deathVolume);

        Destroy(gameObject);
    }
}
