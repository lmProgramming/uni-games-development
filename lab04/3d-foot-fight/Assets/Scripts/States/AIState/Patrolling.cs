using Agents;
using Agents.Enemies;
using LM;
using UnityEngine;

namespace States.AIState
{
    public class Patrolling : AIState
    {
        private const float FieldOfView = 90f;

        private readonly Target[] _patrolTargets;
        private int _currentTargetIndex;

        public Patrolling(Target[] patrolTargets, Enemy enemy, AIStateMachine stateMachine) : base(enemy,
            stateMachine)
        {
            _patrolTargets = patrolTargets;
        }

        public override void LogicUpdate()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                var fromPosition = Enemy.transform.position.SetY(Enemy.transform.position.y + 2f);

                var directionToPlayer = -(fromPosition - player.transform.position).normalized;

                var enemyForward = Enemy.transform.forward.normalized;

                var angle = Vector3.Angle(enemyForward, directionToPlayer);

                var playerInFieldOfView = angle < FieldOfView / 2f;

                if (playerInFieldOfView && Physics.Raycast(fromPosition, directionToPlayer, out var hit, 100))
                    if (hit.transform.CompareTag("Player"))
                    {
                        Enemy.SoundManager.Play("hello");
                        Machine.ChangeState(new GoToTarget(Enemy, Machine, new Target(hit.transform)));
                    }
            }

            if ((Enemy.transform.position - _patrolTargets[_currentTargetIndex].CurrentPosition).sqrMagnitude < 1f)
            {
                _currentTargetIndex++;
                if (Random.value < 0.2f) Enemy.SoundManager.Play("wind");
            }

            if (_currentTargetIndex >= _patrolTargets.Length) _currentTargetIndex = 0;

            Enemy.GoTo(_patrolTargets[_currentTargetIndex]);
        }
    }
}