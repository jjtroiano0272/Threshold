using UnityEngine;
using UnityEngine.UI;

public class CT_HealthBar : MonoBehaviour
{
    public int health;
    public int numHearts;
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    Damageable playerDamageable;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("No player found!");
        }
        playerDamageable = player.GetComponent<Damageable>();
    }

    private void Start()
    {
        // OnPlayerHealthChanged(playerDamageable.Health, playerDamageable.MaxHealth);
    }

    private void Update()
    {
        if (health > numHearts)
            health = numHearts;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
            if (i < numHearts)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }

    private void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        // healthSlider.value = newHealth;
        health = newHealth;
        numHearts = maxHealth;
        // CalculateHealth();
    }
}
