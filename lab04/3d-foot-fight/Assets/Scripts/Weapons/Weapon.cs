using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [field: SerializeField] public float Damage { get; protected set; }
        [field: SerializeField] public float RecoveryTime { get; protected set; }
        [field: SerializeField] public Vector3 DefaultPosition { get; set; }
        [field: SerializeField] public Quaternion DefaultRotation { get; set; }

        public async UniTask GoToRestingPosition()
        {
            transform.DOLocalMove(DefaultPosition, RecoveryTime);
            transform.DORotateQuaternion(DefaultRotation, RecoveryTime);

            await UniTask.Delay(TimeSpan.FromSeconds(RecoveryTime));
        }
    }
}