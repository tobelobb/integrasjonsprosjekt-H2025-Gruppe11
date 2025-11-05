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
        if (!IsOwner) return;

        cooldown -= Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && cooldown <= 0f)
        {
            cooldown = 1f / fireRate;
            ShootServerRpc();
        }
    }

    [ServerRpc]
    void ShootServerRpc()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }

        if (audioSrc && shootSfx)
        {
            audioSrc.PlayOneShot(shootSfx, shootVolume);
        }
    }
}
