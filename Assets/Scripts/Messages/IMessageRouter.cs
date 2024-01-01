using Riptide;

namespace Messages
{
    public interface IMessageRouter
    {
        void Send(Message message);
        void Handle(ushort messageType, Message message);
        void Subscribe(ushort messageType, IMessageHandler handler);
        void Unsubscribe(ushort messageType);
    }
}