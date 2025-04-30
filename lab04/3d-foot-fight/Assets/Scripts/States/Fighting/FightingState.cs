using Agents;
using EasyStateMachine;
using UnityEngine;

namespace States.Fighting
{
    public abstract class FightingState : State<FightingStateMachine, FightingState>
    {
        protected readonly Character Character;

        protected FightingState(Character character, FightingStateMachine stateMachine) : base(stateMachine)
        {
            Character = character;
        }

        public override void Enter()
        {
            Debug.Log(this);
        }

        public override void Exit()
        {
        }

        // Called every frame (Update) - Good for input checking
        public virtual void HandleInput()
        {
        }

        // Called every frame (Update) - Good for non-physics updates (like camera rotation)
        public virtual void LogicUpdate()
        {
        }

        // Called every fixed frame (FixedUpdate) - Good for physics/movement
        public virtual void PhysicsUpdate()
        {
        }

        // Called every fixed frame AFTER PhysicsUpdate - Good for checking ground state AFTER moving
        public virtual void PostPhysicsUpdate()
        {
        }
    }
}