using UnityEngine;
using UnityEngine.UI; // для UI

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Image healthBarFill; // перетаскиваем сюда Image заполнения HP Bar

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log("Игрок погиб!");
        // Здесь можно добавить логику смерти (рестарт сцены, анимация смерти и т.д.)
    }
}
