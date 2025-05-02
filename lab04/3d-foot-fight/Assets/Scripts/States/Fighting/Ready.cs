using Agents;

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
        }
    }
}