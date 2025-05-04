using System;
using System.Threading;
using Agents;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace States.Fighting
{
    public class Swinging : FightingState
    {
        private readonly float _swingTime;
        private CancellationTokenSource _cancellationTokenSource;

        public Swinging(Character character, FightingStateMachine stateMachine, float swingTime) : base(character,
            stateMachine)
        {
            _swingTime = swingTime;
        }

        public override void Enter()
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                Character.destroyCancellationToken
            );

            Swing(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid Swing(CancellationToken cancellationToken)
        {
            var weapon = Character.CurrentWeapon;

            if (!(weapon && weapon.CompareTag("SwingWeapon"))) return;

            weapon.transform.DOLocalMoveX(weapon.transform.localPosition.x + 1.4f, _swingTime)
                .SetEase(Ease.InOutSine);
            weapon.transform.DOLocalRotate(new Vector3(0, 0, -90), _swingTime)
                .SetEase(Ease.InOutSine);

            await UniTask.Delay(TimeSpan.FromSeconds(_swingTime * 2 / 3), cancellationToken: cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;

            var hits = new RaycastHit[3];
            Physics.BoxCastNonAlloc(Character.transform.position, Vector3.one * 1f,
                Character.transform.forward, hits, Quaternion.identity, 3f, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits) hit.collider?.GetComponent<Damageable>()?.TakeDamage(10);

            await UniTask.Delay(TimeSpan.FromSeconds(_swingTime * 1 / 3), cancellationToken: cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;

            Machine.ChangeState(Character.GetRecoveryState(weapon.RecoveryTime));
        }
    }
}