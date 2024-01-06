using System;
using Core;
using Riptide;

namespace Messages
{
    public class MessageHandler<T> : IMessageHandler<T>, IMessageHandler, IFinite where T : IMessageSerializable, new()
    {
        public event Action<T> OnMessageHandled;
        
        public void HandleMessage(Message message)
        {
            T serializable = message.GetSerializable<T>();
            OnMessageHandled?.Invoke(serializable);
        }

        public void Finite()
        {
            OnMessageHandled = null;
        }
    }
}