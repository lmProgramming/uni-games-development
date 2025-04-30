using Agents;
using Agents.Enemies;

namespace States.AIState
{
    public class GoToTarget : AIState
    {
        private readonly Target _target;

        public GoToTarget(Enemy enemy, AIStateMachine stateMachine, Target target) : base(enemy, stateMachine)
        {
            _target = target;
        }

        public override void LogicUpdate()
        {
            if ((Enemy.transform.position - _target.CurrentPosition).sqrMagnitude < 1f)
            {
                //todo
            }

            Enemy.GoTo(_target);
        }
    }
}