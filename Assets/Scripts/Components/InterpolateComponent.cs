using DataStructures;
using UnityEngine;

namespace Components
{
    public struct InterpolateMoveComponent
    {
        public CircularBuffer<Vector3> PositionsBuffer;
        public Vector3 PrevPosition;
        public Vector3 TargetPosition;
        public float InterpolationTime;
        public float InterpolationFactor;

        public void CalculateInterpolationFactor(float speed)
        {
            InterpolationFactor = (TargetPosition - PrevPosition).magnitude / speed;
        }
    }
}