using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float minX = -7.5f;   // adjust for camera width
    public float maxX = 7.5f;

    private bool facingRight = true;

    void Update()
    {
        // --- Horizontal Movement (Arrow keys / A & D) ---
        float input = Input.GetAxisRaw("Horizontal"); // returns -1, 0, or 1
        transform.Translate(Vector3.right * input * moveSpeed * Time.deltaTime);

        // Flip sprite if direction changes
        if (input > 0 && !facingRight)
        {
            Flip();
        }
        else if (input < 0 && facingRight)
        {
            Flip();
        }

        // Clamp player inside screen
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
