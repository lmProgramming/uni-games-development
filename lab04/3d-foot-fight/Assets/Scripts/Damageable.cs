using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;

    [SerializeField] public bool fullHealthOnStart = true;

    public Action<float> OnDamageTaken;
    public Action OnDeath;

    public void TakeDamage(float damage)
    {
        if (damage < 0)
        {
            Debug.LogError("Damage taken is negative");
            return;
        }

        if (damage == 0 || currentHealth <= 0) return;

        OnDamageTaken.Invoke(damage);
        currentHealth -= damage;
        if (currentHealth <= 0) OnDeath.Invoke();
    }
}