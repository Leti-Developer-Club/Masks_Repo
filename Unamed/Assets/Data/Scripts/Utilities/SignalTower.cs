using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SignalTower : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image fillImage;


    [Header("Events")]
    public UnityEvent onTowerDestroyed;
    public UnityEvent<float> onHealthChanged; // For UI updates (pass normalized health)

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    private void Update()
    {
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Update UI
        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    public void HealPercentage(float percent)
    {
        currentHealth = Mathf.Clamp(currentHealth + maxHealth * percent, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    void UpdateHealthBar()
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log("Signal Tower Destroyed!");
        onTowerDestroyed?.Invoke();
        // Optional: play VFX, disable scripts, end game, etc.
    }
}

