using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    protected bool hasJumped;
    protected bool jumpReleased;
    protected Vector2 inputDirection;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void ComputeVelocity()
    {
        return; //This is directly from the tutorial. I'm using the new input system instead
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButton("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        targetVelocity = move * maxSpeed;
    }

    public void HandleJumpInput(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            hasJumped = true;
            velocity.y = jumpTakeOffSpeed;
        }
        else if (context.canceled)
        {
            jumpReleased = true;
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }
    }

    public void HandleMoveInput(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
        Vector2 move = Vector2.zero;

        move.x = inputDirection.x;

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }
}
