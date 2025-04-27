using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;

    public UnityEvent onDeath;
    public UnityEvent<float> onDamageTaken;

    public void TakeDamage(float damage)
    {
        if (damage < 0)
        {
            Debug.LogError("Damage taken is negative");
            return;
        }

        if (damage == 0 || currentHealth <= 0) return;

        onDamageTaken.Invoke(damage);
        currentHealth -= damage;
        if (currentHealth <= 0) onDeath.Invoke();
    }
}