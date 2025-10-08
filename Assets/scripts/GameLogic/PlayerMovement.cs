using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float minX = -7.5f;   // adjust for camera width
    public float maxX = 7.5f;
    void Update()
    {
        // --- Horizontal Movement (Arrow keys / A & D) ---
        float input = Input.GetAxisRaw("Horizontal"); // returns -1, 0, or 1
        transform.Translate(Vector3.right * input * moveSpeed * Time.deltaTime);

        // Clamp player inside screen
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }
}
