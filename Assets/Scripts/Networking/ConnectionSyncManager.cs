using System;
using Character;
using Core;
using Messages;
using MessageStructs;

namespace Networking
{
    public sealed class ConnectionSyncManager : IConnectionSyncManager, IFinite
    {
        private readonly ICharacterProvider _characterProvider;

        private readonly IMessageHandler<CharactersSyncMessage> _charactersSyncHandler;
        private readonly IMessageHandler<RemoveClientMessage> _removeClientHandler;

        public event Action<ushort> OnClientDisconnected;
        
        public ConnectionSyncManager(ICharacterProvider characterProvider, IMessageRouter messageRouter)
        {
            _characterProvider = characterProvider;

            _charactersSyncHandler = messageRouter.GetHandler<CharactersSyncMessage>((ushort)MessageType.CharactersSync);
            _removeClientHandler = messageRouter.GetHandler<RemoveClientMessage>((ushort)MessageType.RemoveClient);

            _charactersSyncHandler.OnMessageHandled += CharacterSyncHandler_Callback;
            _removeClientHandler.OnMessageHandled += RemoveClientHandler_Callback;
        }

        private void CharacterSyncHandler_Callback(CharactersSyncMessage charactersSyncMessage)
        {
            for (int i = 0; i < charactersSyncMessage.Size; i++)
            {
                CharacterSyncElem character = charactersSyncMessage.Characters[i];
                _characterProvider.CreateCharacter(character.Id, character.Type, character.Position);
            }
        }
        
        private void RemoveClientHandler_Callback(RemoveClientMessage removeClientMessage)
        {
            ushort removeId = removeClientMessage.Id;
            _characterProvider.RemoveCharacter(removeId);
            OnClientDisconnected?.Invoke(removeId);
        }

        public void Finite()
        {
            _charactersSyncHandler.OnMessageHandled -= CharacterSyncHandler_Callback;
            _removeClientHandler.OnMessageHandled -= RemoveClientHandler_Callback;
        }
    }
}