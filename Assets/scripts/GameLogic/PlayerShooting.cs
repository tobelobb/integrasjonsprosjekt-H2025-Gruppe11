using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Fire")]
    public float fireRate = 6f;
    private float cooldown;

    [Header("Audio")]
    public AudioClip shootSfx;
    [Range(0f, 1f)] public float shootVolume = 0.8f;
    private AudioSource audioSrc;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    void Update()
    {
        // In multiplayer, only the owner controls shooting
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && !IsOwner) return;

        cooldown -= Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && cooldown <= 0f)
        {
            cooldown = 1f / fireRate;

            // Play audio locally
            if (audioSrc != null && shootSfx != null)
                audioSrc.PlayOneShot(shootSfx, shootVolume);

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                // Multiplayer → ask server to spawn bullet
                ShootServerRpc();
            }
            else
            {
                // Singleplayer → just spawn bullet locally
                SpawnBulletLocal();
            }
        }
    }

    [ServerRpc]
    void ShootServerRpc()
    {
        SpawnBulletLocal();
    }

    void SpawnBulletLocal()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("BulletPrefab or FirePoint not assigned!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // If running multiplayer, spawn as a network object
        var netObj = bullet.GetComponent<NetworkObject>();
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && netObj != null)
        {
            netObj.Spawn();
        }
    }
}
