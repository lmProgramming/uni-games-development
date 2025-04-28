using System;
using Cysharp.Threading.Tasks;
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
            Debug.Log(TimeSpan.FromSeconds(_swingTime).Seconds);
            await UniTask.Delay(TimeSpan.FromSeconds(_swingTime));

            var hits = new RaycastHit[25];
            Physics.BoxCastNonAlloc(Character.transform.position, Vector3.one * 10f,
                Character.transform.forward, hits, Quaternion.identity, 5f, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits) hit.collider?.GetComponent<Damageable>()?.TakeDamage(10);

            Machine.ChangeState(Character.Ready);
        }
    }
}