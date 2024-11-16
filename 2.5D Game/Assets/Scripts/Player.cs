using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private SimpleFlash simpleFlash;


    [Header("Player Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private AudioSource audioSource;
    [Header("Player Audio Settings")]
    public AudioClip jumpSound;
    public AudioClip attackSound;
    public AudioClip footstepSound;
    public AudioClip bashSound;

    private Vector2 moveInput;
    private Rigidbody rb;
    private bool isGrounded;
    private bool isWalking;
    private bool isBlocking;
    private bool isPlayingFootsteps = false;

    [Header("Player Combat Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    private int currentCombo = 0;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // OnMove is called when the player moves
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // OnJump is called when the player jumps
    public void OnJump(InputAction.CallbackContext context)
    {
        // Allow jump if the jump button is pressed and the player is grounded
        if (context.performed && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            // Play jump sound
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    // OnAttack is called when the player attacks
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (Time.time >= nextAttackTime)
        {
            if (context.performed)
            {
                HandleAttack();
                nextAttackTime = Time.time + 1f / attackRate;
                if (currentCombo < 2)
                {
                    currentCombo++;
                }
                else
                {
                    currentCombo = 0;
                }
            }
        }
    }

    // Attack logic
    private void HandleAttack()
    {
        switch (currentCombo)
        {
            case 0:
                animator.SetTrigger("Attack1");
                break;
            case 1:
                animator.SetTrigger("Attack2");
                break;
            case 2:
                animator.SetTrigger("Attack3");
                break;
            default:
                animator.SetTrigger("Attack1");
                break;
        }

        // Play attack sound
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage enemies
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Dummy>().TakeDamage();
            // Show flash effect
            simpleFlash.Flash();
        }
    }

    // OnBlock is called when the player blocks
    public void OnBash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Bash");  // Trigger the Block animation

            // Play bash sound
            if (bashSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(bashSound);
            }
        }

        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage enemies
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Dummy>().TakeDamage();
            // Show flash effect
            simpleFlash.Flash();
        }
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        // Move the player on the X-axis using Vector3 to account for 3D space
        rb.velocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, rb.velocity.z);

        // Grounded check using Physics.CheckSphere
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
        isWalking = moveInput.x != 0;
        animator.SetBool("isWalking", isWalking);

        HandleFootsteps();

        // Set IsJumping based on grounded state
        animator.SetBool("isJumping", !isGrounded);

        // Flip the player's direction based on movement
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);  // Facing right
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);  // Facing left
        }
    }

    // Handle footstep sounds
    private void HandleFootsteps()
    {
        // If player is walking, grounded, and not already playing footsteps
        if (isWalking && isGrounded && !isPlayingFootsteps)
        {
            isPlayingFootsteps = true;
            InvokeRepeating("PlayFootstepSound", 0f, 0.25f); // Adjust timing as needed
        }
        else if ((!isWalking || !isGrounded) && isPlayingFootsteps)
        {
            isPlayingFootsteps = false;
            CancelInvoke("PlayFootstepSound");
        }
    }

    // Play footstep sound
    private void PlayFootstepSound()
    {
        if (footstepSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(footstepSound);
        }
    }

    // OnDrawGizmosSelected is called when the object is selected
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}