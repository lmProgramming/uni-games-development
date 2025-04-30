using Agents;
using Agents.Enemies;
using LM;
using UnityEngine;

namespace States.AIState
{
    public class Patrolling : AIState
    {
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

                var direction = -(fromPosition - player.transform.position).normalized;
                Debug.DrawLine(fromPosition, fromPosition + direction * 100, Color.red, 0.1f);

                if (Physics.Raycast(fromPosition, direction, out var hit, 100))
                    if (hit.transform.CompareTag("Player"))
                        Machine.ChangeState(new GoToTarget(Enemy, Machine, new Target(hit.transform)));
            }

            if ((Enemy.transform.position - _patrolTargets[_currentTargetIndex].CurrentPosition).sqrMagnitude < 1f)
                _currentTargetIndex++;

            if (_currentTargetIndex >= _patrolTargets.Length) _currentTargetIndex = 0;

            Enemy.GoTo(_patrolTargets[_currentTargetIndex]);
        }
    }
}