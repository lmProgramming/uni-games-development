using Agents;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Damageable))]
public class OnDamageSpawnBlood : MonoBehaviour
{
    private Damageable _damageable;

    [Inject] private ParticleSpawner _particleSpawner;

    private void Awake()
    {
        _damageable = GetComponent<Damageable>();
    }

    public void OnEnable()
    {
        _damageable.OnDamageTaken += HandleDamageTaken;
    }

    public void OnDisable()
    {
        _damageable.OnDamageTaken -= HandleDamageTaken;
    }

    private void HandleDamageTaken(float damage)
    {
        SpawnBlood();
    }

    private void SpawnBlood()
    {
        _particleSpawner.SpawnBlood(transform.position + Vector3.up * 1.5f);
    }
}