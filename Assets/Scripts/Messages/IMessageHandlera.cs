using System;

namespace Messages
{
    public interface IMessageHandler<T>
    {
        public event Action<T> OnMessageHandled;
    }
}