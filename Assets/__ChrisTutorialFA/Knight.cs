// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
// public class Knight : MonoBehaviour
// {
//     public float walkSpeed = 4f;
//     public DetectionZone attackZone;
//     public float walkStopRate = 0.07f;

//     Rigidbody2D rb;
//     TouchingDirections touchingDirections;
//     Animator animator;

//     // 2 directions for now, possibly mor elater
//     public enum WalkableDirection
//     {
//         Right,
//         Left,
//     };

//     private WalkableDirection _walkDirection;
//     private Vector2 walkDirectionVector = Vector2.right;

//     public WalkableDirection WalkDirection
//     {
//         get { return _walkDirection; }
//         set
//         {
//             if (_walkDirection != value)
//             {
//                 gameObject.transform.localScale = new Vector2(
//                     gameObject.transform.localScale.x * -1,
//                     gameObject.transform.localScale.y
//                 );

//                 if (value == WalkableDirection.Right)
//                 {
//                     walkDirectionVector = Vector2.right;
//                 }
//                 else if (value == WalkableDirection.Left)
//                 {
//                     walkDirectionVector = Vector2.left;
//                 }
//             }
//             _walkDirection = value;
//         }
//     }

//     public bool _hasTarget = false;

//     public bool HasTarget
//     {
//         get { return _hasTarget; }
//         private set
//         {
//             _hasTarget = value;
//             animator.SetBool(AnimationStrings.hasTarget, value);
//         }
//     }

//     // public bool _hasTarget = false;
//     public bool CanMove
//     {
//         get { return animator.GetBool(AnimationStrings.canMove); }
//     }

//     void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         touchingDirections = GetComponent<TouchingDirections>();
//         animator = GetComponent<Animator>();
//     }

//     void Update()
//     {
//         HasTarget = attackZone.detectedColliders.Count > 0;
//     }

//     void FixedUpdate()
//     {
//         if (touchingDirections.IsOnWall && touchingDirections.IsGrounded)
//         {
//             Debug.Log("confirmed direction flip");
//             FlipDirection();
//         }
//         if (CanMove)
//         {
//             rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
//         }
//         else
//         {
//             rb.linearVelocity = new Vector2(
//                 Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate),
//                 rb.linearVelocity.y
//             );
//         }
//     }

//     private void FlipDirection()
//     {
//         if (WalkDirection == WalkableDirection.Right)
//         {
//             WalkDirection = WalkableDirection.Left;
//         }
//         else if (WalkDirection == WalkableDirection.Left)
//         {
//             WalkDirection = WalkableDirection.Right;
//         }
//         else
//         {
//             Debug.LogError("No real walk direction!");
//         }
//     }
// }


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Knight : MonoBehaviour
{
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.7f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public enum WalkableDirection
    {
        Right,
        Left,
    };

    private WalkableDirection _walkDirection;

    // hello?
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector3(
                    gameObject.transform.localScale.x * -1,
                    gameObject.transform.localScale.y,
                    gameObject.transform.localScale.z
                );

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }

            _walkDirection = value;
        }
    }

    public bool _hasTarget;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }
    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set { animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0)); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }
        if (!damageable.LockVelocity)
        {
            if (CanMove)
                // Accelerate to max speed


                rb.linearVelocity = new Vector2(
                    // walkAcceleration * walkDirectionVector.x,
                    Mathf.Clamp(
                        rb.linearVelocity.x
                            + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime),
                        -maxSpeed,
                        maxSpeed
                    ),
                    rb.linearVelocity.y
                );
            // NOTE: adjust walkstoprate for how much the enemy slides when stopping
            else
                rb.linearVelocity = new Vector2(
                    Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate),
                    rb.linearVelocity.y
                );
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Big whoops!");
        }
    }

    // stun lock the enemy on hit
    public void OnHit(int damage, Vector3 knockback)
    {
        rb.linearVelocity = new Vector3(knockback.x, rb.linearVelocity.y, 0);
    }

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }
}
