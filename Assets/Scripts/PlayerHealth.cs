using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Image healthBarFill; // полоска здоровья

    [Header("Death UI")]
    public GameObject deathPanel; // сюда перетащите вашу Panel Game Over

    private bool isDead = false; // предотвращает спам смерти

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Скрываем панель смерти в начале игры
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // если уже умер, не принимаем урон

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
        if (isDead) return; // защита от повторного вызова
        isDead = true;

        Debug.Log("Игрок погиб!");

        // Включаем панель смерти
        if (deathPanel != null)
            deathPanel.SetActive(true);

        // Отключаем управление игроком
        var controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        // Отключаем скрипт движения игрока (если есть)
        var movement = GetComponent<Player>();
        if (movement != null)
            movement.enabled = false;

        // Можно здесь добавить остановку анимаций, звуков и т.д.
    }
}
