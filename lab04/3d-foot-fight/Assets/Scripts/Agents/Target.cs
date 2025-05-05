using System;
using UnityEngine;

namespace Agents
{
    public class Target
    {
        private readonly Transform _target;
        private readonly Vector3 _targetPosition;

        private readonly TargetType _targetType;

        public Target(Transform target)
        {
            _target = target;
            _targetType = TargetType.Transform;
        }

        public Target(Vector3 position)
        {
            _targetPosition = position;
            _targetType = TargetType.Position;
        }

        public Vector3 CurrentPosition
        {
            get
            {
                return _targetType switch
                {
                    TargetType.Transform => _target.position,
                    TargetType.Position => _targetPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private enum TargetType
        {
            Transform,
            Position
        }
    }
}