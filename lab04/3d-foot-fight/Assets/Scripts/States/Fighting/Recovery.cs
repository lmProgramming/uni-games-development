using Agents;
using Cysharp.Threading.Tasks;

namespace States.Fighting
{
    public class Recovery : FightingState
    {
        private readonly float _recoveryTime;

        public Recovery(Character character, FightingStateMachine stateMachine, float recoveryTime) : base(character,
            stateMachine)
        {
            _recoveryTime = recoveryTime;
        }

        public override void Enter()
        {
            Recover().Forget();
        }

        private async UniTaskVoid Recover()
        {
            var weapon = Character.CurrentWeapon;

            if (!(weapon && weapon.CompareTag("SwingWeapon"))) return;

            await weapon.GoToRestingPosition();

            Machine.ChangeState(Character.Ready);
        }
    }
}