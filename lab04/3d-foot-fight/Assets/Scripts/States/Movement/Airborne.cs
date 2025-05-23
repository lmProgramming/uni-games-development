﻿using Agents;
using UnityEngine;

namespace States.Movement
{
    public class Airborne : CharacterState
    {
        public Airborne(Character character, MovementStateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void PhysicsUpdate()
        {
            Character.VerticalVelocity += Character.gravityValue * Time.fixedDeltaTime;

            var move = Character.transform.right * Character.MoveDirection.x +
                       Character.transform.forward * Character.MoveDirection.y;

            var horizontalMovement = move.normalized * (Character.moveSpeed * Character.airMoveSpeedMultiplier);

            Character.Controller.Move((horizontalMovement + Vector3.up * Character.VerticalVelocity) *
                                      Time.fixedDeltaTime);
        }

        public override void PostPhysicsUpdate()
        {
            if (Character.IsGrounded &&
                Character.VerticalVelocity <
                0.0f)
                Machine.ChangeState(Character.GetGroundedState());
        }
    }
}