using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CustomGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float groundingForce = -1f;
    public float fallAcceleration = 80f;
    public float maxFallSpeed = 20f;
    public float jumpEndEarlyGravityModifier = 3f;
    public float groundCheckDistance = 0.05f;
    public LayerMask groundLayer;

    protected Rigidbody2D rb;
    protected Collider2D col;

    protected bool grounded;
    protected bool endedJumpEarly;

    protected Vector2 velocity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        CheckGrounded();
        ApplyGravity();
        ApplyVelocity();
    }

    void CheckGrounded()
    {
        grounded = Physics2D.CapsuleCast(col.bounds.center, col.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, groundCheckDistance, groundLayer);
    }

    void ApplyGravity()
    {
        if (grounded && velocity.y <= 0f)
        {
            velocity.y = groundingForce;
        }
        else
        {
            float gravity = fallAcceleration;
            if (endedJumpEarly && velocity.y > 0)
                gravity *= jumpEndEarlyGravityModifier;

            velocity.y = Mathf.MoveTowards(velocity.y, -maxFallSpeed, gravity * Time.fixedDeltaTime);
        }
    }

    void ApplyVelocity()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocity.y);
    }

    // Optional: call this externally if needed
    public void SetVerticalVelocity(float y)
    {
        velocity.y = y;
    }

    public void EndJumpEarly()
    {
        endedJumpEarly = true;
    }

    public bool IsGrounded() => grounded;
}
