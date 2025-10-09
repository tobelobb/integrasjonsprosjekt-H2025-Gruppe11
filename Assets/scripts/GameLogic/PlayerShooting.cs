using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;          // assign your FirePoint here
    public GameObject bulletPrefab;      // assign Bullet prefab here

    [Header("Fire")]
    public float fireRate = 6f;          // bullets per second while holding Space
    private float _cooldown = 0f;

    void Update()
    {
        _cooldown -= Time.deltaTime;

        // Hold Space to autofire
        if (Input.GetKey(KeyCode.Space) && _cooldown <= 0f)
        {
            Shoot();
            _cooldown = 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (!firePoint || !bulletPrefab) return;

        // Spawn bullet at the fire point, facing up (world up)
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}
