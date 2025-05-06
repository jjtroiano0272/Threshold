using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float minAttackTime = 1f;
    public float maxAttackTime = 3f;
    public int attackDamage = 10;

    private void Start()
    {
        ScheduleNextAttack();
    }

    void Attack()
    {
        Debug.Log("Enemy Attacks!");

        // Add attack logic here, e.g., damage the player

        ScheduleNextAttack(); // Schedule the next attack after this one
    }

    void ScheduleNextAttack()
    {
        float randomDelay = Random.Range(minAttackTime, maxAttackTime);
        Invoke(nameof(Attack), randomDelay);
    }
}
