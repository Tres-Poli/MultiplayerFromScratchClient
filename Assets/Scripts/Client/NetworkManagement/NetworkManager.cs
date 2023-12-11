using System;
using Core;
using Cysharp.Threading.Tasks;
using ResourceManagement;
using Riptide;
using UnityEngine;

namespace NetworkManagement
{
    internal sealed class NetworkManager : INetworkManager, IFixedController
    {
        private readonly ILoggerService _loggerService;
        private readonly ITickController _tickController;
        private const string ClientConfig = "ClientConfig";

        private Riptide.Client _client;
        private IFinite _updateSubscription;

        public NetworkManager(ILoggerService loggerService, IResourceManager resourceManager, ITickController tickController)
        {
            _loggerService = loggerService;
            _tickController = tickController;

            InitializeAsync(resourceManager).Forget();
        }

        private async UniTaskVoid InitializeAsync(IResourceManager resourceManager)
        {
            ClientConfig config = await resourceManager.LoadConfig<ClientConfig>(ClientConfig);
            if (config == null)
            {
                _loggerService.LogError("Client cannot start without proper config");
                return;
            }
            
            _client = new Riptide.Client();
            if (!_client.Connect($"{config.ValidatedIp}:{config.Port}"))
            {
                _loggerService.LogError("Cannot initialize connection to server");
                return;
            }

            _client.TimeoutTime = 5000;
            _updateSubscription = _tickController.AddController(this);

            _client.Connected += Connected_Callback;
            _client.ConnectionFailed += ConnectionFalide_Callback;
        }

        public void FixedUpdate(float deltaTime)
        {
            _client.Update();
        }

        private void Connected_Callback(object sender, EventArgs eventArgs)
        {
            _loggerService.Log($"Client connected");
            DoAfterConnectionTestRoutine().Forget();
        }

        private void ConnectionFalide_Callback(object sender, ConnectionFailedEventArgs args)
        {
            _loggerService.Log($"Client failed {args.Reason}");
        }

        private async UniTaskVoid DoAfterConnectionTestRoutine()
        {
            await UniTask.Delay(1000);
            
            Message goRightMessage = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.PositionControl);
            goRightMessage.AddFloat(1f);
            goRightMessage.AddFloat(0f);
            goRightMessage.AddFloat(0f);

            _client.Send(goRightMessage);
            await UniTask.Delay(1000);

            Message goForwardMessage = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.PositionControl);
            goForwardMessage.AddFloat(0f);
            goForwardMessage.AddFloat(0f);
            goForwardMessage.AddFloat(1f);
            
            _client.Send(goForwardMessage);
            await UniTask.Delay(1000);
            
            Message goLeftMessage = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.PositionControl);
            goLeftMessage.AddFloat(-1f);
            goLeftMessage.AddFloat(0f);
            goLeftMessage.AddFloat(0f);
            
            _client.Send(goLeftMessage);
            await UniTask.Delay(1000);
            
            Message stopMessage = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.PositionControl);
            stopMessage.AddFloat(0f);
            stopMessage.AddFloat(0f);
            stopMessage.AddFloat(0f);
            
            _client.Send(stopMessage);
        }
    }
}