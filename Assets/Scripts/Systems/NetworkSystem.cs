using System;
using CharacterControllers;
using Configs;
using Core;
using Leopotam.Ecs;
using Messages;
using ResourceManagement;
using Riptide;

namespace Systems
{
    internal sealed class NetworkSystem : IEcsInitSystem, IEcsRunSystem
    {
        public static Client Client { get; private set; }
        
        private readonly ILoggerService _loggerService;
        private readonly IResourceManager _resourceManager;
        private readonly IMessageRouter _messageRouter;
        private readonly ICharacterFactory _characterFactory;
        private const string ClientConfig = "ClientConfig";

        private Client _client;

        public NetworkSystem(ILoggerService loggerService, IResourceManager resourceManager, IMessageRouter messageRouter, 
            ICharacterFactory characterFactory)
        {
            _loggerService = loggerService;
            _resourceManager = resourceManager;
            _messageRouter = messageRouter;
            _characterFactory = characterFactory;
        }
        
        public async void Init()
        {
            ClientConfig config = await _resourceManager.LoadConfig<ClientConfig>(ClientConfig);
            if (config == null)
            {
                _loggerService.LogError("Client cannot start without proper config");
                return;
            }
            
            _client = new Client();
            Client = _client;
            if (!_client.Connect($"{config.ValidatedIp}:{config.Port}", useMessageHandlers: false))
            {
                _loggerService.LogError("Cannot initialize connection to server");
                return;
            }

            _client.TimeoutTime = 5000;

            _client.Connected += Connected_Callback;
            _client.ConnectionFailed += ConnectionFailed_Callback;
            _client.MessageReceived += MessageReceived_Callback;
        }

        public void Run()
        {
            _client.Update();
        }

        private void MessageReceived_Callback(object sender, MessageReceivedEventArgs eventArgs)
        {
            _messageRouter.Handle(eventArgs.MessageId, eventArgs.Message);
        }

        private void Connected_Callback(object sender, EventArgs eventArgs)
        {
            _loggerService.Log($"Client connected");
            _characterFactory.CreateCharacter(_client.Id);
        }

        private void ConnectionFailed_Callback(object sender, ConnectionFailedEventArgs args)
        {
            _loggerService.Log($"Client failed {args.Reason}");
        }
    }
}