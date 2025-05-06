using System.Collections;
using UnityEngine;

public class EnemyAttack2 : MonoBehaviour
{
    public Transform player;
    public float attackRange = 5f;
    public LayerMask playerLayer;

    public float minAttackDelay = 1f;
    public float maxAttackDelay = 2f;

    private bool playerInRange;
    private bool isAttacking;

    void Update()
    {
        // Cast a ray toward the player
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

    // Optional: Draw the raycast in the editor
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
