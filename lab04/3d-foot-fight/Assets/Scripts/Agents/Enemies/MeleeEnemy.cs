using System.Linq;
using LM;
using States.AIState;
using UnityEngine;
using UnityEngine.AI;

namespace Agents.Enemies
{
    public class MeleeEnemy : Enemy
    {
        [SerializeField] private Transform[] patrolPositions;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float damageInterval = 1f;

        private SimpleTimer _damageTimer;
        private Patrolling _patrollingState;
        private AIStateMachine _stateMachine;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Damageable = GetComponent<Damageable>();
        }

        private void Start()
        {
            _stateMachine = new AIStateMachine();

            _patrollingState =
                new Patrolling(patrolPositions.Select(p => new Target(p)).ToArray(), this, _stateMachine);

            _stateMachine.Initialize(_patrollingState);

            _damageTimer = new SimpleTimer(damageInterval, false);
        }

        private void Update()
        {
            _stateMachine.CurrentState.LogicUpdate();
        }

        private void OnEnable()
        {
            Damageable.OnDeath += Die;
        }

        private void OnDisable()
        {
            Damageable.OnDeath -= Die;
        }

        public void OnCollisionStay(Collision other)
        {
            if (!other.transform.CompareTag("Player")) return;

            if (_damageTimer.IsFinished(true))
            {
                var damageable = other.transform.GetComponent<Damageable>();
                if (damageable != null) damageable.TakeDamage(damage);

                return;
            }

            if (_damageTimer.State == SimpleTimer.TimerState.Off) _damageTimer.StartManualWait();

            _damageTimer.Tick(Time.deltaTime);
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}