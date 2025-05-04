using Agents;
using UnityEngine;

namespace States.Fighting
{
    public class Ready : FightingState
    {
        public Ready(Character character, FightingStateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void LogicUpdate()
        {
            if (Character.AttackRequested) Machine.ChangeState(Character.GetSwingingState(1f));

            var ray = new Ray(Character.PlayerCamera.transform.position, Character.PlayerCamera.transform.forward);

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Food"))) return;

            if (!hit.transform.CompareTag("Food") || !Input.GetKeyDown(KeyCode.E)) return;

            Object.Destroy(hit.transform.gameObject);
            Character.Damageable.Heal(10);
        }
    }
}