using Riptide;

namespace MessageStructs
{
    public struct RemoveClientMessage : IMessageSerializable
    {
        public ushort Id;
        
        public void Serialize(Message message)
        {
            message.AddUShort(Id);
        }

        public void Deserialize(Message message)
        {
            Id = message.GetUShort();
        }
    }
}