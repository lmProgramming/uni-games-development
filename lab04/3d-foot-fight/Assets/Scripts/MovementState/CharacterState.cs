using EasyStateMachine;

namespace MovementState
{
    public abstract class CharacterState : State<CharacterStateMachine, CharacterState>
    {
        protected readonly Character Character;

        protected CharacterState(Character character, CharacterStateMachine stateMachine) : base(stateMachine)
        {
            Character = character;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
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