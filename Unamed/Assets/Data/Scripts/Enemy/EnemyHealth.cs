using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image fillImage;

    [Header("Events")]
    public UnityEvent onEnemyDestroyed;

    private PointsManager pointsManager;

    private void Awake()
    {
        currentHealth = maxHealth;
        pointsManager = FindFirstObjectByType<PointsManager>();
    }

    void Update()
    {
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        GetComponent<LootBag>().InstantiateLoot(transform.position);
        pointsManager.KillCount(1);
        onEnemyDestroyed?.Invoke();
        // Optional: play VFX, disable scripts, end game, etc.
    }
}
