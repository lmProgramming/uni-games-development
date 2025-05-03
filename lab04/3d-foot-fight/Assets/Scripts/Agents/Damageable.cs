using System;
using UnityEngine;
using UnityEngine.Events;

namespace Agents
{
    public class Damageable : MonoBehaviour
    {
        public float maxHealth = 100;
        public float currentHealth;

        [SerializeField] public bool fullHealthOnStart = true;
        public UnityEvent<float> OnDamageTakenUnityEvent;
        public UnityEvent OnDeathUnityEvent;

        public Action<float> OnDamageTaken;
        public Action OnDeath;

        private void Start()
        {
            if (fullHealthOnStart) currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (damage < 0)
            {
                Debug.LogError("Damage taken is negative");
                return;
            }

            if (damage == 0 || currentHealth <= 0) return;

            OnDamageTaken?.Invoke(damage);
            OnDamageTakenUnityEvent?.Invoke(damage);

            currentHealth -= damage;

            if (!(currentHealth <= 0)) return;

            OnDeath?.Invoke();
            OnDeathUnityEvent?.Invoke();
        }

        public void SetHealth(float initialHealth, float? newMaxHealth = null)
        {
            if (initialHealth < 0)
            {
                Debug.LogError("Initial health is negative");
                return;
            }

            maxHealth = newMaxHealth ?? maxHealth;
            currentHealth = initialHealth;

            if (!(currentHealth > maxHealth)) return;

            Debug.LogError("Initial health is greater than max health");
            currentHealth = maxHealth;
        }
    }
}