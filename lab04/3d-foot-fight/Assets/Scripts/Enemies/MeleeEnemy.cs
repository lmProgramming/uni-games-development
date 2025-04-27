using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class MeleeEnemy : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Damageable _damageable;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _damageable = GetComponent<Damageable>();
        }

        private void Start()
        {
            _damageable.OnDeath += () => Destroy(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            _agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
        }
    }
}