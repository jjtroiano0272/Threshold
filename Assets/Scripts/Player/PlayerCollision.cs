using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    [Header("Wall Check Settings")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;

    public bool IsGrounded { get; private set; }
    public bool IsTouchingWallLeft { get; private set; }
    public bool IsTouchingWallRight { get; private set; }

    void Update()
    {
        // Check if touching ground
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Check if touching walls
        IsTouchingWallLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer);
        IsTouchingWallRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);
    }

    void OnDrawGizmosSelected()
    {
        // Draw ground check
        if (groundCheck != null)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Draw wall checks
        if (wallCheckLeft != null)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheckLeft.position, wallCheckRadius);

        if (wallCheckRight != null)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheckRight.position, wallCheckRadius);
    }
}
