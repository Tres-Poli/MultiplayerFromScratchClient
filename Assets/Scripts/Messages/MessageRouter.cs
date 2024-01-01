using System.Collections.Generic;
using Core;
using Riptide;
using Systems;

namespace Messages
{
    public sealed class MessageRouter : IMessageRouter
    {
        private readonly ILoggerService _logger;
        private Dictionary<ushort, IMessageHandler> _subscribers;
        
        public MessageRouter(ILoggerService logger)
        {
            _logger = logger;
            _subscribers = new Dictionary<ushort, IMessageHandler>();
        }

        public void Send(Message message)
        {
            NetworkSystem.Client.Send(message);
            message.Release();
        }

        public void Handle(ushort messageType, Message message)
        {
            if (_subscribers.TryGetValue(messageType, out IMessageHandler handler))
            {
                handler.HandleMessage(message);
            }
            
            message.Release();
        }

        public void Subscribe(ushort messageType, IMessageHandler handler)
        {
            if (_subscribers.ContainsKey(messageType))
            {
                _logger.LogError("Only one message handler per message type is allowed");   
                return;
            }
            
            _subscribers.Add(messageType, handler);
        }

        public void Unsubscribe(ushort messageType)
        {
            _subscribers.Remove(messageType);
        }
    }
}