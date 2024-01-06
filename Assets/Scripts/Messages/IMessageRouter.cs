using Riptide;

namespace Messages
{
    public interface IMessageRouter
    {
        void Send(Message message);
        void Handle(ushort messageType, Message message);
        IMessageHandler<T> GetHandler<T>(ushort messageType) where T : IMessageSerializable, new();
    }
}