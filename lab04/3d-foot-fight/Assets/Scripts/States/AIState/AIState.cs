using Agents.Enemies;
using EasyStateMachine;
using UnityEngine;

namespace States.AIState
{
    public abstract class AIState : State<AIStateMachine, AIState>
    {
        protected readonly Enemy Enemy;

        protected AIState(Enemy enemy, AIStateMachine stateMachine) : base(stateMachine)
        {
            Enemy = enemy;
        }

        public override void Enter()
        {
            Debug.Log(this);
        }

        public override void Exit()
        {
        }

        public virtual void LogicUpdate()
        {
        }
    }
}