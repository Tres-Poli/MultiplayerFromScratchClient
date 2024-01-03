using UnityEngine;

namespace DataStructures
{
    public struct ReconNode
    {
        public ReconNode(uint reconId, Vector3 deltaPosition)
        {
            ReconId = reconId;
            DeltaPosition = deltaPosition;
        }
        
        public uint ReconId;
        public Vector3 DeltaPosition;
    }
}