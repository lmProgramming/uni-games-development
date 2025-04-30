using Agents;
using UnityEngine;

namespace States.Movement
{
    public class Grounded : CharacterState
    {
        private bool _jumpRequested;

        public Grounded(Character character, MovementStateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Entering Grounded State");

            _jumpRequested = false;
            if (Character.VerticalVelocity < 0f)
                Character.VerticalVelocity = -2f;
        }

        public override void HandleInput()
        {
            if (Character.JumpRequested)
                _jumpRequested = true;
        }

        public override void PhysicsUpdate()
        {
            var move = Character.transform.right * Character.MoveDirection.x +
                       Character.transform.forward * Character.MoveDirection.y;

            var horizontalMovement = move.normalized * Character.moveSpeed;

            Character.Controller.Move((horizontalMovement + Vector3.up * Character.VerticalVelocity) *
                                      Time.fixedDeltaTime);

            if (_jumpRequested)
            {
                Character.VerticalVelocity = Mathf.Sqrt(Character.jumpHeight * -2f * Character.gravityValue);
                _jumpRequested = false;
                Machine.ChangeState(Character.Airborne);
                return;
            }

            if (Character.IsGrounded && Character.VerticalVelocity < 0.0f)
                Character.VerticalVelocity = -2f;
        }

        public override void PostPhysicsUpdate()
        {
            if (!Character.IsGrounded)
                Machine.ChangeState(Character.Airborne);
        }

        public override void Exit()
        {
            Debug.Log("Exiting Grounded State");
        }
    }
}