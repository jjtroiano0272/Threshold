using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public GameObject bloodEffect;
    public AudioSource audioSource;
    public AudioClip[] hitSounds;
    public GameObject deathEffect;

    private float rallyWindow = 2f; // ✅ Time window for rally (2 seconds)
    private float rallyTimer = 0f;
    private int potentialRallyHealth = 0; // ✅ Health that can be regained


    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20, Vector2.right);
        }
        if (rallyTimer > 0)
        {
            rallyTimer -= Time.deltaTime;
        }
        else
        {
            potentialRallyHealth = 0; // ✅ If time runs out, cancel rally health
        }

        if (Input.GetKeyDown(KeyCode.G)) // For testing damage
        {
            TakeDamage(20, Vector2.right);
        }

    }

    public void TakeDamage(int damage, Vector2 origin)
    {
        currentHealth -= damage;

        potentialRallyHealth = damage; // ✅ Store the amount that can be rallied
        rallyTimer = rallyWindow; // ✅ Reset rally timer

        // Play a random hit sound if available
        if (hitSounds.Length > 0 && audioSource != null)
        {
            audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
        }

        // Blood Particle example
        Instantiate(bloodEffect, transform.position, Quaternion.identity);

        // Camera shake code example
        //CameraShake.instance.Shake();

        // Knockback code example
        //GetComponent<Rigidbody2D>().AddForce((GetComponent<Rigidbody2D>().position - origin).normalized * 500f, ForceMode2D.Force);

        if (currentHealth <= 0)
        {
            // If health reaches zero, destroy the object with an effect
            if (currentHealth <= 0)
                Destroy();
            // Destroy(gameObject);
        }

        healthBar.SetCurrentHealth(currentHealth);
    }

    // ✅ Call this when the player hits an enemy
    public void AttemptRally(int damageDealt)
    {
        if (rallyTimer > 0 && potentialRallyHealth > 0)
        {
            int rallyAmount = Mathf.Min(damageDealt / 2, potentialRallyHealth); // ✅ Example: 50% of dealt damage is restored
            currentHealth += rallyAmount;
            potentialRallyHealth -= rallyAmount;
            healthBar.SetCurrentHealth(currentHealth);
        }
    }


    void Destroy()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
