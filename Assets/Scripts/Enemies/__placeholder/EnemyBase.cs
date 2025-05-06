using System.Collections;
using UnityEngine;

public class Enemy : CustomGravity
{

    [Header("Enemy Parameters")]
    public int totalHealth = 100;
    int currentHealth;
    [SerializeField] private bool damageable = true;
    [SerializeField] private float invulnerabilityTime = .2f;
    [SerializeField] private ParticleSystem _damageParticles;
    public bool giveUpwardForce = true;
    private bool hit;

    [Header("Animation & Audio")]
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip[] attackClips;
    public AudioClip[] hurtClips;

    [Header("Attack Settings")]
    public Transform player;
    public float attackRange = 5f;
    public LayerMask playerLayer;
    public float minAttackDelay = 1f;
    public float maxAttackDelay = 2f;
    private bool playerInRange;
    private bool isAttacking;
    private bool isDead = false;


    // For the damage flash
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;
    private Coroutine _damageFlashCoroutine;

    private void Awake()
    {

        currentHealth = totalHealth;

        // For damage flash
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();


        _materials = new Material[_spriteRenderers.Length];

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            // Does this cause particle system to re-instantiated?
            // _materials[i] = Instantiate(_spriteRenderers[i].material);
            // _spriteRenderers[i].material = _materials[i];

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = totalHealth;
    }

    void Update()
    {
        // Cast a ray toward the player so it can attack
        Vector2 direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, attackRange, playerLayer);

        playerInRange = hit.collider != null;

        if (playerInRange && !isAttacking)
        {
            StartCoroutine(AttackLoop());
        }
    }

    IEnumerator AttackLoop()
    {
        isAttacking = true;

        while (playerInRange)
        {
            float delay = Random.Range(minAttackDelay, maxAttackDelay);
            yield return new WaitForSeconds(delay);

            // Still in range?
            Vector2 direction = player.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, attackRange, playerLayer);

            if (hit.collider != null)
            {
                Debug.Log("Enemy attacks!");
                // Add your attack logic here (damage, animation, etc.)
            }
            else
            {
                break;
            }
        }

        isAttacking = false;
    }



    void PlayRandomAttackSound()
    {
        int i = Random.Range(0, attackClips.Length);
        audioSource.PlayOneShot(attackClips[i]);
    }

    #region Enemy gets hit
    public void TakeDamage(int damage, Vector2 pushback)
    {
        if (isDead) return;

        // Instantiate(_damageParticles);
        _damageParticles.Play();

        StartCoroutine(ApplyPushback(pushback));


        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        PlayHurtSound();

        if (_damageFlashCoroutine != null)
            StopCoroutine(_damageFlashCoroutine);

        _damageFlashCoroutine = StartCoroutine(DamageFlash());


        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator DamageFlash()
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetColor("_Color", _flashColor);
        }

        yield return new WaitForSeconds(_flashTime);

        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetColor("_Color", Color.white); // Or original color if different
        }

        _damageFlashCoroutine = null;
    }

    private IEnumerator ApplyPushback(Vector2 push)
    {
        float timer = 0.1f; // short burst
        while (timer > 0)
        {
            transform.position += (Vector3)push * Time.deltaTime * 10;
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    void PlayHurtSound()
    {
        if (audioSource != null && hurtClips.Length > 0)
        {
            int index = Random.Range(0, hurtClips.Length);
            audioSource.PlayOneShot(hurtClips[index]);
        }
    }

    #endregion


    void Die()
    {
        animator.SetBool("isDead", true);
        // GetComponent<Collider2D>().enabled = false;
        // TODO instead, it becomes invulnearble
        // this.enabled = false;

        isDead = true;
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);


        }
    }
}

