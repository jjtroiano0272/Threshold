// {/* =====================================================================================
// === A: Version learning each thing on its own and self-coding it =============================================
// ========================================================================================= */}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TarodevController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
    public class CT_PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 10f;
        public float runSpeed = 14f;
        public float airWalkSpeed = 5f;
        public float jumpImpulse = 10f;

        [Header("Particles & Movement FX")]
        [SerializeField]
        private ParticleSystem jumpParticles;

        [SerializeField]
        private ParticleSystem moveParticles;

        Vector2 moveInput;
        TouchingDirections touchingDirections;
        Damageable damageable;

        // public bool IsRunning { get; private set; }


        [Header("Squash/Stretch Animation FX")]
        [SerializeField]
        private float scaleTimer = 0f;
        private bool isScalingJumpAnim = false;
        private Vector3 baseScale;
        private AnimationCurve xScaleCurve;

        [SerializeField]
        private AnimationCurve yScaleCurve;

        [SerializeField]
        private float scaleDuration = 0.2f;

        [SerializeField]
        private float curveMultiplier = 1f;

        [Header("Jump Config")]
        private float coyoteTime = 0.5f; // Effects the height when holding jump versus tapping
        private float coyoteTimeCounter;
        private float jumpBufferTime = 0.2f;
        private float jumpBufferCounter;

        [SerializeField]
        private float apexModifier = 0.5f;

        public float CurrentMoveSpeed
        {
            get
            {
                if (CanMove)
                {
                    if (IsMoving && !touchingDirections.IsOnWall)
                        if (touchingDirections.IsGrounded)
                        { // located at https://youtu.be/qcRRYeGMZ68?si=N7JbWbls9sa0FP7e&t=1830 you can also set this to use wall jumps or slides
                            {
                                // TODO Enable if you want to use bimodal movement (run/walk diffences)
                                // if (IsRunning)
                                // {
                                //     return runSpeed;
                                // }
                                // else
                                // {
                                return walkSpeed;
                                // }
                            }
                        }
                        // air state checks
                        else
                        {
                            return airWalkSpeed;
                        }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    // Chang ethis for the speed you can move around while attacking. Default was 0.
                    return 0;
                }
            }
        }
        private bool _isMoving = false;
        public bool IsMoving
        {
            get { return _isMoving; }
            private set
            {
                _isMoving = value;
                animator.SetBool(AnimationStrings.isMoving, value);
            }
        }

        [Header("Directionals")]
        public bool _isFacingRight = true;
        public bool IsFacingRight
        {
            get { return _isFacingRight; }
            private set
            {
                // TODO dunno, this might cause issues later
                if (_isFacingRight != value)
                {
                    // transform.localScale *= new Vector2(-1, 1);

                    transform.localScale = new Vector3(
                        transform.localScale.x * -1,
                        transform.localScale.y,
                        transform.localScale.z
                    );
                }
                _isFacingRight = value;
            }
        }
        public bool CanMove
        {
            get { return animator.GetBool(AnimationStrings.canMove); }
        }

        public bool IsAlive
        {
            get { return animator.GetBool(AnimationStrings.isAlive); }
        }

        Rigidbody2D rb;
        Animator animator;

        // TODO for later if you want dash or run etc.
        // private bool _isRunning = false;
        // public bool IsRunning
        // {
        //     get { return _isRunning; }
        //     private set
        //     {
        //         _isRunning = value;
        //         animator.SetBool("isRunning", value);
        //     }
        // }

        // TODO this probs needs to get fixed--causing move particles to get squashed on x
        private void JumpScaleAnimation()
        {
            scaleTimer += Time.deltaTime;
            float t = scaleTimer / scaleDuration;
            int facingDirection = _isFacingRight ? 1 : -1;

            if (t >= 1f)
            {
                isScalingJumpAnim = false;
                transform.localScale = new Vector3(
                    baseScale.x * facingDirection,
                    baseScale.y,
                    baseScale.z
                );
            }
            else
            {
                float curveValueX = xScaleCurve.Evaluate(t);
                float curveValueY = xScaleCurve.Evaluate(t);
                transform.localScale = new Vector3(
                    curveValueX * facingDirection * curveMultiplier,
                    curveValueY * curveMultiplier,
                    baseScale.z
                );
            }
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            touchingDirections = GetComponent<TouchingDirections>();
            damageable = GetComponent<Damageable>();

            baseScale = transform.localScale;
        }

        private void Update()
        {
            jumpBufferCounter -= Time.deltaTime;

            // Update coyote time
            if (touchingDirections.IsGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            // Attempt jump
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && CanMove)
            {
                Jump();
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
        }

        private void Jump()
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
            jumpParticles.Play();

            scaleTimer = 0f;
            isScalingJumpAnim = true;
        }

        private void FixedUpdate()
        {
            if (IsAlive)
            {
                if (!damageable.LockVelocity)
                {
                    rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocity.y);
                }

                animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);

                // Handle scale animation if active and jumping
                if (isScalingJumpAnim)
                {
                    JumpScaleAnimation();
                }
            }
            // Disable player movement if dead
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();

            if (IsAlive)
            {
                IsMoving = moveInput != Vector2.zero;
                SetFacingDirection(moveInput);
                moveParticles.Play();

                if (touchingDirections.IsGrounded)
                {
                    coyoteTimeCounter = coyoteTime;
                }
                else
                {
                    coyoteTimeCounter -= Time.deltaTime;
                }
            }
            else
            {
                IsMoving = false;
                moveParticles.Stop();
            }
        }

        // When you press JUMP, we set jumpBufferCounter to jumpBufferTime
        // This is assigned to the jump button specifically in the editor?
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else if (context.canceled && rb.linearVelocity.y > 0f)
            {
                // Cut jump short if released early
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * apexModifier
                );
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                animator.SetTrigger(AnimationStrings.attackTrigger);
            }
        }

        // Stunlock the player when hurt
        public void OnHit(int damage, Vector3 knockback)
        {
            rb.linearVelocity = new Vector3(knockback.x, rb.linearVelocity.y, 0);
        }

        private void SetFacingDirection(Vector2 moveInput)
        {
            if (moveInput.x > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (moveInput.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }
    }
}


// =====================================================================================
// ===  B: Version copy pasting in Tarodev stuff into mine =============================================
// =========================================================================================

// using System;
// using UnityEngine;
// using UnityEngine.InputSystem;

// namespace TarodevController
// {
//     [RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
//     public class CT_PlayerController : MonoBehaviour
//     {
//         [Header("Movement Settings")]
//         public float walkSpeed = 10f;
//         public float runSpeed = 14f;
//         public float airWalkSpeed = 5f;
//         public float jumpImpulse = 10f;

//         [Header("Particles & Movement FX")]
//         [SerializeField]
//         private ParticleSystem jumpParticles;

//         [SerializeField]
//         private ParticleSystem moveParticles;

//         Vector2 moveInput;
//         TouchingDirections touchingDirections;
//         Damageable damageable;

//         // public bool IsRunning { get; private set; }

//         [SerializeField]
//         private AnimationCurve xScaleCurve;

//         [SerializeField]
//         private AnimationCurve yScaleCurve;

//         [SerializeField]
//         private float scaleDuration = 0.2f;

//         private float scaleTimer = 0f;
//         private bool isScaling = false;
//         private Vector3 baseScale;

//         [SerializeField]
//         private float curveMultiplier = 1f;

//         // New stuff for retuned jump
//         public bool _grounded;

//         private Vector2 _frameVelocity;
//         private float _time;

//         #region Gravity
//         [SerializeField]
//         private ScriptableStats _stats;
//         private bool _endedJumpEarly;

//         private void HandleGravity()
//         {
//             if (_grounded && _frameVelocity.y <= 0f)
//             {
//                 _frameVelocity.y = _stats.GroundingForce;
//             }
//             else
//             {
//                 var inAirGravity = _stats.FallAcceleration;
//                 if (_endedJumpEarly && _frameVelocity.y > 0)
//                     inAirGravity *= _stats.JumpEndEarlyGravityModifier;
//                 _frameVelocity.y = Mathf.MoveTowards(
//                     _frameVelocity.y,
//                     -_stats.MaxFallSpeed,
//                     inAirGravity * Time.fixedDeltaTime
//                 );
//             }
//         }

//         private void CheckCollisions()
//         {
//             Physics2D.queriesStartInColliders = false;

//             // Ground and Ceiling
//             bool groundHit = Physics2D.CapsuleCast(
//                 _col.bounds.center,
//                 _col.size,
//                 _col.direction,
//                 0,
//                 Vector2.down,
//                 _stats.GrounderDistance,
//                 ~_stats.PlayerLayer
//             );
//             bool ceilingHit = Physics2D.CapsuleCast(
//                 _col.bounds.center,
//                 _col.size,
//                 _col.direction,
//                 0,
//                 Vector2.up,
//                 _stats.GrounderDistance,
//                 ~_stats.PlayerLayer
//             );

//             // Hit a Ceiling
//             if (ceilingHit)
//                 _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

//             // Landed on the Ground
//             if (!_grounded && groundHit)
//             {
//                 _grounded = true;
//                 _coyoteUsable = true;
//                 _bufferedJumpUsable = true;
//                 _endedJumpEarly = false;
//                 GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
//             }
//             // Left the Ground
//             else if (_grounded && !groundHit)
//             {
//                 _grounded = false;
//                 _frameLeftGrounded = _time;
//                 GroundedChanged?.Invoke(false, 0);
//             }

//             Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
//         }
//         #endregion
//         public float CurrentMoveSpeed
//         {
//             get
//             {
//                 if (CanMove)
//                 {
//                     if (IsMoving && !touchingDirections.IsOnWall)
//                         if (touchingDirections.IsGrounded)
//                         { // located at https://youtu.be/qcRRYeGMZ68?si=N7JbWbls9sa0FP7e&t=1830 you can also set this to use wall jumps or slides
//                             {
//                                 // TODO Enable if you want to use bimodal movement (run/walk diffences)
//                                 // if (IsRunning)
//                                 // {
//                                 //     return runSpeed;
//                                 // }
//                                 // else
//                                 // {
//                                 return walkSpeed;
//                                 // }
//                             }
//                         }
//                         // air state checks
//                         else
//                         {
//                             return airWalkSpeed;
//                         }
//                     else
//                     {
//                         return 0;
//                     }
//                 }
//                 else
//                 {
//                     // Chang ethis for the speed you can move around while attacking. Default was 0.
//                     return 0;
//                 }
//             }
//         }
//         private bool _isMoving = false;
//         public bool IsMoving
//         {
//             get { return _isMoving; }
//             private set
//             {
//                 _isMoving = value;
//                 animator.SetBool(AnimationStrings.isMoving, value);
//             }
//         }

//         [Header("Directionals")]
//         public bool _isFacingRight = true;
//         public bool IsFacingRight
//         {
//             get { return _isFacingRight; }
//             private set
//             {
//                 // TODO dunno, this might cause issues later
//                 if (_isFacingRight != value)
//                 {
//                     // transform.localScale *= new Vector2(-1, 1);

//                     transform.localScale = new Vector3(
//                         transform.localScale.x * -1,
//                         transform.localScale.y,
//                         transform.localScale.z
//                     );
//                 }
//                 _isFacingRight = value;
//             }
//         }
//         public bool CanMove
//         {
//             get { return animator.GetBool(AnimationStrings.canMove); }
//         }

//         public bool IsAlive
//         {
//             get { return animator.GetBool(AnimationStrings.isAlive); }
//         }

//         Rigidbody2D rb;
//         Animator animator;

//         // TODO for later if you want dash or run etc.
//         // private bool _isRunning = false;
//         // public bool IsRunning
//         // {
//         //     get { return _isRunning; }
//         //     private set
//         //     {
//         //         _isRunning = value;
//         //         animator.SetBool("isRunning", value);
//         //     }
//         // }

//         void Awake()
//         {
//             rb = GetComponent<Rigidbody2D>();
//             animator = GetComponent<Animator>();
//             touchingDirections = GetComponent<TouchingDirections>();
//             damageable = GetComponent<Damageable>();

//             baseScale = transform.localScale;
//         }

//         void Update()
//         {
//             _time += Time.deltaTime;
//             GatherInput();
//         }

//         private FrameInput _frameInput;

//         private void GatherInput()
//         {
//             _frameInput = new FrameInput
//             {
//                 JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
//                 JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
//                 Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
//             };

//             // if (_stats.SnapInput)
//             // {
//             //     _frameInput.Move.x =
//             //         Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold
//             //             ? 0
//             //             : Mathf.Sign(_frameInput.Move.x);
//             //     _frameInput.Move.y =
//             //         Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold
//             //             ? 0
//             //             : Mathf.Sign(_frameInput.Move.y);
//             // }

//             // if (_frameInput.JumpDown)
//             // {
//             //     _jumpToConsume = true;
//             //     _timeJumpWasPressed = _time;
//             // }
//         }

//         private void FixedUpdate()
//         {
//             if (IsAlive)
//             {
//                 if (!damageable.LockVelocity)
//                 {
//                     rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocity.y);
//                 }
//                 animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);

//                 // Handle scale animation if active and jumping
//                 if (isScaling)
//                 {
//                     scaleTimer += Time.deltaTime;
//                     float t = scaleTimer / scaleDuration;
//                     int facingDirection = _isFacingRight ? 1 : -1;

//                     if (t >= 1f)
//                     {
//                         isScaling = false;
//                         transform.localScale = new Vector3(
//                             baseScale.x * facingDirection,
//                             baseScale.y,
//                             baseScale.z
//                         );
//                     }
//                     else
//                     {
//                         float curveValueX = xScaleCurve.Evaluate(t);
//                         float curveValueY = xScaleCurve.Evaluate(t);
//                         transform.localScale = new Vector3(
//                             curveValueX * facingDirection * curveMultiplier,
//                             curveValueY * curveMultiplier,
//                             baseScale.z
//                         );
//                     }
//                 }

//                 // CheckCollisions();

//                 // HandleJump();
//                 // HandleDirection();
//                 HandleGravity();

//                 // ApplyMovement();
//             }
//             else
//             {
//                 rb.linearVelocity = Vector2.zero;
//             }
//         }

//         public void OnMove(InputAction.CallbackContext context)
//         {
//             moveInput = context.ReadValue<Vector2>();

//             if (IsAlive)
//             {
//                 IsMoving = moveInput != Vector2.zero;
//                 SetFacingDirection(moveInput);

//                 moveParticles.Play();
//             }
//             else
//             {
//                 IsMoving = false;

//                 moveParticles.Stop();
//             }
//         }

//         public void OnJump(InputAction.CallbackContext context)
//         {
//             if (context.started && touchingDirections.IsGrounded && CanMove)
//             {
//                 animator.SetTrigger(AnimationStrings.jumpTrigger);
//                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);

//                 jumpParticles.Play();

//                 scaleTimer = 0f;
//                 isScaling = true;
//             }
//         }

//         public void OnAttack(InputAction.CallbackContext context)
//         {
//             if (context.started)
//             {
//                 animator.SetTrigger(AnimationStrings.attackTrigger);
//             }
//         }

//         // Stunlock the player when hurt
//         public void OnHit(int damage, Vector3 knockback)
//         {
//             rb.linearVelocity = new Vector3(knockback.x, rb.linearVelocity.y, 0);
//         }

//         private void SetFacingDirection(Vector2 moveInput)
//         {
//             if (moveInput.x > 0 && !IsFacingRight)
//             {
//                 IsFacingRight = true;
//             }
//             else if (moveInput.x < 0 && IsFacingRight)
//             {
//                 IsFacingRight = false;
//             }
//         }
//     }
// }
