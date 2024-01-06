using System.Collections.Generic;
using Core;
using MessageStructs;
using Riptide;
using Systems;
using UnityEngine;

namespace Messages
{
    public sealed class MessageRouter : IMessageRouter
    {
        private readonly ILoggerService _logger;
        private Dictionary<ushort, IMessageHandler> _handlers;
        
        public MessageRouter(ILoggerService logger)
        {
            _logger = logger;
            _handlers = new Dictionary<ushort, IMessageHandler>();

            MessageHandler<PositionMessage> positionMessageHandler = new MessageHandler<PositionMessage>();
            MessageHandler<CharactersSyncMessage> charactersSyncHandler = new MessageHandler<CharactersSyncMessage>();
            MessageHandler<RemoveClientMessage> removeClientHandler = new MessageHandler<RemoveClientMessage>();
            
            _handlers.Add((ushort)MessageType.Position, positionMessageHandler);
            _handlers.Add((ushort)MessageType.CharactersSync, charactersSyncHandler);
            _handlers.Add((ushort)MessageType.RemoveClient, removeClientHandler);
        }

        public void Send(Message message)
        {
            NetworkSystem.Client.Send(message);
            message.Release();
        }

        public void Handle(ushort messageType, Message message)
        {
            _handlers[messageType].HandleMessage(message);
            message.Release();
        }

        public IMessageHandler<T> GetHandler<T>(ushort messageType) where T : IMessageSerializable, new()
        {
            if (_handlers.ContainsKey(messageType))
            {
                return (IMessageHandler<T>)_handlers[messageType];
            }

            Debug.LogError($"No handlers with type {(MessageType)messageType}");
            return default;
        }
    }
}