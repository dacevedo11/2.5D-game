using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Vector2 moveInput;
    private Rigidbody rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Allow jump if the jump button is pressed and the player is grounded
        if (context.performed && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);  // Apply jump force on Y-axis
        }
    }

    private void FixedUpdate()
    {
        // Move the player on the X-axis using Vector3 to account for 3D space
        rb.velocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, rb.velocity.z);

        // Grounded check using Physics.CheckSphere
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
    }
}
