using UnityEngine;
using Unity.Netcode;

public class PlayerMovementNet : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float minX = -7.5f;
    public float maxX = 7.5f;

    private bool facingRight = true;

    //private Vector2 touchStartPos;


    // Position sync
    private NetworkVariable<Vector3> syncedPosition = new(writePerm: NetworkVariableWritePermission.Server);

    void Update()
    {
        if (!IsOwner) return;

        float input = Input.GetAxisRaw("Horizontal");
        Vector3 move = Vector3.right * input * moveSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + move;

        /*
                // --- Touchscreen movement (horizontal drag) ---
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
        
            if (t.phase == TouchPhase.Began)
            {
                touchStartPos = t.position;
            }
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                float dragDelta = t.position.x - touchStartPos.x;
        
                // Normalize drag into -1 to +1
                input = Mathf.Clamp(dragDelta / (Screen.width * 0.1f), -1f, 1f);
            }
        }

        */

        // Clamp position
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        transform.position = newPos;

        // Flip sprite
        if (input > 0 && !facingRight)
            Flip();
        else if (input < 0 && facingRight)
            Flip();

        // Send position to server
        UpdatePositionServerRpc(transform.position);
    }

    void LateUpdate()
    {
        if (!IsOwner)
        {
            transform.position = syncedPosition.Value;
        }
    }

    [ServerRpc]
    void UpdatePositionServerRpc(Vector3 pos)
    {
        syncedPosition.Value = pos;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
