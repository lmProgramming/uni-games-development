using System.Linq;
using States.AIState;
using UnityEngine;
using UnityEngine.AI;

namespace Agents.Enemies
{
    public class MeleeEnemy : Enemy
    {
        [SerializeField] private Transform[] patrolPositions;

        private GoToTarget _goToTargetState;

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
            _goToTargetState = new GoToTarget(this, _stateMachine, null);

            _stateMachine.Initialize(_patrollingState);
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

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}