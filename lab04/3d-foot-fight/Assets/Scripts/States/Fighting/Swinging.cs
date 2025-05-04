using System;
using Agents;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace States.Fighting
{
    public class Swinging : FightingState
    {
        private readonly float _swingTime;

        public Swinging(Character character, FightingStateMachine stateMachine, float swingTime) : base(character,
            stateMachine)
        {
            _swingTime = swingTime;
        }

        public override void Enter()
        {
            Swing().Forget();
        }

        private async UniTaskVoid Swing()
        {
            var weapon = Character.CurrentWeapon;

            if (!(weapon && weapon.CompareTag("SwingWeapon"))) return;

            var cancellationToken = Character.CancellationToken;

            weapon.transform.DOLocalMoveX(weapon.transform.localPosition.x + 1.4f, _swingTime)
                .SetEase(Ease.InOutSine);
            weapon.transform.DOLocalRotate(new Vector3(0, 0, -90), _swingTime)
                .SetEase(Ease.InOutSine);

            await UniTask.Delay(TimeSpan.FromSeconds(_swingTime * 2 / 3), cancellationToken: cancellationToken);

            var hits = new RaycastHit[3];
            Physics.BoxCastNonAlloc(Character.transform.position, Vector3.one * 1f,
                Character.transform.forward, hits, Quaternion.identity, 3f, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits) hit.collider?.GetComponent<Damageable>()?.TakeDamage(10);

            await UniTask.Delay(TimeSpan.FromSeconds(_swingTime * 1 / 3), cancellationToken: cancellationToken);

            Machine.ChangeState(Character.GetRecoveryState(weapon.RecoveryTime));
        }
    }
}