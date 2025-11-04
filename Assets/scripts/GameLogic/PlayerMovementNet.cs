using UnityEngine;
using Unity.Netcode;

public class PlayerMovementNet : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float minX = -7.5f;
    public float maxX = 7.5f;

    private bool facingRight = true;

    // Position sync
    private NetworkVariable<Vector3> syncedPosition = new(writePerm: NetworkVariableWritePermission.Server);

    void Update()
    {
        if (!IsOwner) return;

        float input = Input.GetAxisRaw("Horizontal");
        Vector3 move = Vector3.right * input * moveSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + move;

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
