using UnityEngine;

public class CT_Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector3 knockback = Vector3.zero;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            // NOTE: Change the z value iff you decide to use positioning into the z scale later on
            // Vector3 deliveredKnockback = new Vector3(
            //     knockback.x * Mathf.Sign(transform.parent.localScale.x),
            //     knockback.y,
            //     0
            // );
            // Vector3 deliveredKnockback =
            //     transform.parent.localScale.x > 0
            //         ? knockback
            //         : new Vector3(knockback.x, knockback.y, 0);

            Vector3 deliveredKnockback = knockback;
            if (transform.parent.localScale.x < 0)
            {
                deliveredKnockback.x *= -1;
            }

            // previously was -knockback.x but this caused weirdness. Still does too.

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            Debug.Log($"X > 0, {transform.parent.localScale.x > 0}");

            if (gotHit)
                Debug.Log(collision.name + " hit for damage " + attackDamage);
        }
    }
}
