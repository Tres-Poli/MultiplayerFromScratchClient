using Riptide;

namespace Messages
{
    public struct ReconciliationSyncMessage : IMessageSerializable
    {
        public uint ReconciliationId;
        
        public void Serialize(Message message)
        {
            message.AddUInt(ReconciliationId);
        }

        public void Deserialize(Message message)
        {
            ReconciliationId = message.GetUInt();
        }
    }
}