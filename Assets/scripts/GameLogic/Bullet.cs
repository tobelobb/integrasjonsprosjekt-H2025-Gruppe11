using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 3f;

    void Start()
    {
        // Destroy automatically after a few seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move straight up every frame
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
