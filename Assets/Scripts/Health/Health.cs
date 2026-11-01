using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] bool destroyOnDeath = true;

    public UnityEvent onDeath;

    float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        Debug.Log($"{name} recibió {amount} de daño ({currentHealth}/{maxHealth})");

        if (IsDead)
        {
            onDeath?.Invoke();
            if (destroyOnDeath) Destroy(gameObject);
        }
    }
}