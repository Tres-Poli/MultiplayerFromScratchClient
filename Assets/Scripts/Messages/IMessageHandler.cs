using Riptide;

namespace Messages
{
    public interface IMessageHandler
    {
        void HandleMessage(Message message);
    }
}