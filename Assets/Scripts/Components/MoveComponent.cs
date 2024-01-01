using UnityEngine;

namespace Components
{
    public struct MoveComponent
    {
        public Vector3 PrevPosition;
        public Vector3 TargetPosition;
        public float InterpolationFactor;
        public float Interpolation;
        public float Speed;
    }
}