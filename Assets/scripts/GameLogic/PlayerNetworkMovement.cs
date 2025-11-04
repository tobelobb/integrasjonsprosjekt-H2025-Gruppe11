using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float minX = -7.5f;
    public float maxX = 7.5f;

    private bool facingRight = true;

    void Update()
    {
        // Only allow the local player to control this object
        if (!IsOwner) return;

        float input = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector3.right * input * moveSpeed * Time.deltaTime);

        if (input > 0 && !facingRight)
        {
            Flip();
        }
        else if (input < 0 && facingRight)
        {
            Flip();
        }

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
