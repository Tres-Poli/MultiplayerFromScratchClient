using Riptide;
using UnityEngine;

namespace MessageStructs
{
    public struct PositionMessage : IMessageSerializable
    {
        public ushort Id;
        public Vector3 Position;
        public uint ReconciliationId;
        
        public void Serialize(Message message)
        {
            message.AddUShort(Id);
            message.AddUInt(ReconciliationId);
            message.AddFloat(Position.x);
            message.AddFloat(Position.y);
            message.AddFloat(Position.z);
        }

        public void Deserialize(Message message)
        {
            Id = message.GetUShort();
            ReconciliationId = message.GetUInt();
            Position = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
        }
    }
}