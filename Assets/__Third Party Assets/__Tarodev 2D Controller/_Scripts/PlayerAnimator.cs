using System.Collections;
using UnityEngine;

namespace TarodevController
{
    /// <summary>
    /// VERY primitive animator example.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Animator _anim;

        [SerializeField] private SpriteRenderer _sprite;

        [Header("Settings")]
        [SerializeField, Range(1f, 3f)]
        private float _maxIdleSpeed = 2;

        [SerializeField] private float _maxTilt = 5;
        [SerializeField] private float _tiltSpeed = 20;

        [Header("Particles")][SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _launchParticles;
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;
        [SerializeField] private ParticleSystem _hurtParticles;

        // Combat
        public GameObject attackEffect; // assign in inspector
        // private Animator attackAnimator;
        // public string attackAnimationName = "AttackSlash"; // exact name of your clip
        private bool isAttacking;



        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip[] _footsteps;
        // TODO Sasquatch
        [Header("Camera Stuff")]
        [SerializeField] private GameObject _cameraFollowGO;

        private AudioSource _source;
        private IPlayerController _player;
        public bool _grounded;
        private ParticleSystem.MinMaxGradient _currentGradient;

        // TODO Sasquatch
        private CameraFollowObject _cameraFollowObject;
        public bool IsFacingRight { get; set; }
        public bool IsGrounded { get; set; }
        public bool _isFacingRight;



        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _player = GetComponentInParent<IPlayerController>();
            // TODO Sasquatch
            _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();

            _isFacingRight = true;

            // attackAnimator = attackEffect.GetComponent<Animator>();
            // attackEffect.SetActive(false);


        }

        private void OnEnable()
        {
            _player.Jumped += OnJumped;
            _player.GroundedChanged += OnGroundedChanged;

            _moveParticles.Play();
        }

        private void OnDisable()
        {
            _player.Jumped -= OnJumped;
            _player.GroundedChanged -= OnGroundedChanged;

            _moveParticles.Stop();
        }

        private void Update()
        {
            if (_player == null) return;

            DetectGroundColor();

            HandleSpriteFlip();

            HandleIdleSpeed();

            HandleCharacterTilt();
            HandleAttack();

        }

        private void HandleAttack()
        {
            if (Input.GetKeyDown(KeyCode.K) && !isAttacking)
            {
                StartCoroutine(PlayAttack());
            }

        }

        IEnumerator PlayAttack()
        {
            isAttacking = true;
            attackEffect.SetActive(true);
            // attackAnimator.Play("AttackSlash", 0, 0f); // play from start

            // Wait until the animation finishes
            // yield return new WaitForSeconds(attackAnimator.GetCurrentAnimatorStateInfo(0).length);
            yield return new WaitForSeconds(0.4f);

            attackEffect.SetActive(false);
            isAttacking = false;
        }


        // TODO Refer here if you want to do this differnetly https://youtu.be/9dzBrLUIF8g?si=rBpKFIypVuEB03yd&t=217
        private void HandleSpriteFlip()
        {
            if (_player.FrameInput.x != 0) _sprite.flipX = _player.FrameInput.x < 0;

            Turn();
        }

        private void HandleIdleSpeed()
        {
            var inputStrength = Mathf.Abs(_player.FrameInput.x);
            _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, inputStrength));
            _moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
        }

        private void HandleCharacterTilt()
        {
            var runningTilt = _grounded ? Quaternion.Euler(0, 0, _maxTilt * _player.FrameInput.x) : Quaternion.identity;
            _anim.transform.up = Vector3.RotateTowards(_anim.transform.up, runningTilt * Vector2.up, _tiltSpeed * Time.deltaTime, 0f);
        }

        private void OnJumped()
        {
            _anim.SetTrigger(JumpKey);
            _anim.ResetTrigger(GroundedKey);


            if (_grounded) // Avoid coyote
            {
                SetColor(_jumpParticles);
                SetColor(_launchParticles);
                _jumpParticles.Play();
            }
        }

        private void OnGroundedChanged(bool grounded, float impact)
        {
            _grounded = grounded;

            if (grounded)
            {
                DetectGroundColor();
                SetColor(_landParticles);

                _anim.SetTrigger(GroundedKey);
                _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
                _moveParticles.Play();

                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
                _landParticles.Play();
            }
            else
            {
                _moveParticles.Stop();
            }
        }

        private void DetectGroundColor()
        {
            var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

            if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
            var color = r.color;
            _currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
            SetColor(_moveParticles);
        }

        private void SetColor(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = _currentGradient;
        }
        private void Turn()
        {

            Debug.Log("In turn");
            if (IsFacingRight)
            {
                Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                IsFacingRight = !IsFacingRight;
                _cameraFollowObject.CallTurn();
            }
            else
            {
                Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                IsFacingRight = !IsFacingRight;
                _cameraFollowObject.CallTurn();
            }
        }

        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
    }
}