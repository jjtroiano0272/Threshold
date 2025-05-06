using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public Transform attackOrigin;
    public float attackRadius = 1f;
    public LayerMask enemyMask;

    public float cooldownTime = .5f;
    private float cooldownTimer = 0f;
    public int attackDamage = 25;

    public Animator animator;
    public AudioSource attackSound;
    public GameObject swipePrefab; // Assign your SwipeEffect Prefab here in the Inspector
    // public Transform swipeSpawnPoint; // Assign an empty GameObject where the swipe should appear

    private void Update()
    {
        if (cooldownTimer <= 0)
        {
            if (Input.GetKey(KeyCode.K))
            {
                Attack();
            }
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void Attack()
    {
        // Spawn the swipe effect
        GameObject swipe = Instantiate(swipePrefab, transform.position, Quaternion.identity);
        Destroy(swipe, 0.5f);
        PlayAttackSound();

        // Example of playing attack animation
        // animator.SetTrigger("Attack");

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(attackOrigin.position, attackRadius, enemyMask);
        foreach (var enemy in enemiesInRange)
        {
            var direction = 1;
            // enemy.GetComponent<HealthManager>().TakeDamage(attackDamage, transform.position);
            enemy.GetComponent<Enemy>().TakeDamage(1, new Vector2(0.2f * direction, 0));

            // // ✅ Attempt to rally lost health
            // GetComponent<HealthManager>().AttemptRally(attackDamage);

        }

        cooldownTimer = cooldownTime;

    }

    void PlayAttackSound()
    {
        if (attackSound != null)
        {
            attackSound.Play();
        }
    }


    private void OnDrawGizmos()
    {

        if (attackOrigin == null) { return; }
        Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }
}