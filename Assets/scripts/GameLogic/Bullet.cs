using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.15f;

    float nextFireTime = 0f;

    void Update()
    {
        // Press Space to shoot once per press:
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }

        // If you prefer holding mouse to autofire, use this instead:
        // if (Input.GetMouseButton(0) && Time.time >= nextFireTime) { Shoot(); nextFireTime = Time.time + fireCooldown; }
    }

    void Shoot()
    {
        // Spawn bullet at muzzle, pointing up
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}
