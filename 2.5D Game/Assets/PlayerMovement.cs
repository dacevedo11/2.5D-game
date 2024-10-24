using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust the player's movement speed
    private Vector2 moveInput;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Ensure the player has a Rigidbody2D component
    }

    // Input system method triggered by the "Move" action
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); // Get the movement vector from input
    }

    private void FixedUpdate()
    {
        // Horizontal movement only (left/right)
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }
}
