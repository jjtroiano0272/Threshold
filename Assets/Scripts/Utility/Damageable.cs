// using UnityEngine;

// public class Damageable : MonoBehaviour
// {
//     Animator animator;

//     [SerializeField]
//     private int _maxHealth = 100;
//     public int MaxHealth
//     {
//         get { return _maxHealth; }
//         set { _maxHealth = value; }
//     }

//     [SerializeField]
//     private int _health = 100;
//     public int Health
//     {
//         get { return _health; }
//         set
//         {
//             _health = value;

//             if (_health <= 0)
//             {
//                 IsAlive = false;
//             }
//         }
//     }

//     [SerializeField]
//     private bool _isAlive = true;

//     [SerializeField]
//     private bool isInvincible = false;
//     private float timeSinceHit = 0f;
//     public float invincibilityTime = 0.25f;

//     public bool IsAlive
//     {
//         get { return _isAlive; }
//         set
//         {
//             _isAlive = value;
//             animator.SetBool(AnimationStrings.isAlive, value);
//             Debug.Log("IsAlive: " + value);
//         }
//     }

//     void Awake()
//     {
//         animator = GetComponent<Animator>();
//     }

//     void Update()
//     {
//         if (isInvincible)
//         {
//             if (timeSinceHit > invincibilityTime)
//             {
//                 isInvincible = false;
//                 timeSinceHit = 0f;
//             }
//             timeSinceHit += Time.deltaTime;
//         }

//         Hit(10);
//     }

//     public void Hit(int damage)
//     {
//         if (IsAlive && !isInvincible)
//         {
//             Health -= damage;
//             isInvincible = true;
//         }
//     }
// }

using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector3> damageableHit;
    public UnityEvent damageableDeath;
    public UnityEvent<int, int> healthChanged;

    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool isInvincible = false;

    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("isAlive set " + value);

            if (value == false)
            {
                damageableDeath.Invoke();
            }
        }
    }
    public bool LockVelocity
    {
        get { return animator.GetBool(AnimationStrings.lockVelocity); }
        set { animator.SetBool(AnimationStrings.lockVelocity, value); }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector3 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);

            return true;
        }

        // Unable to get hit
        return false;
    }
}
