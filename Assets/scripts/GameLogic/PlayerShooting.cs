using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Fire")]
    public float fireRate = 6f;
    private float cooldown;

    [Header("Audio")]
    public AudioClip shootSfx;      // Shooting audio
    [Range(0f, 1f)] public float shootVolume = 0.8f;
    private AudioSource audioSrc;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>(); // Added on player
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && cooldown <= 0f)
        {
            Shoot();
            cooldown = 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab && firePoint)
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // play the sound without interrupting previous ones
        if (audioSrc && shootSfx)
            audioSrc.PlayOneShot(shootSfx, shootVolume);
    }
}
