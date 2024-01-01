using System.Collections.Generic;
using Leopotam.Ecs;
using Components;
using Input;
using Messages;
using MessageStructs;
using Riptide;
using UnityEngine;

namespace Systems
{
    public class MoveSystem : IEcsRunSystem, IEcsDestroySystem, IMessageHandler
    {
        private const float AcceptableDesyncEpsilonSqr = 3f;
        private const int ReconciliationBufferSize = 1024;
        
        private readonly IMessageRouter _messageRouter;
        private readonly IPlayerInput _playerInput;
        private EcsFilter<IdComponent, MoveComponent, BodyComponent> _filter;

        private Vector3[] _reconciliationBuffer;
        private uint _reconciliationCounter;

        public MoveSystem(IMessageRouter messageRouter, IPlayerInput playerInput)
        {
            _reconciliationCounter = 0;
            _reconciliationBuffer = new Vector3[ReconciliationBufferSize];
            _messageRouter = messageRouter;
            _playerInput = playerInput;
            _playerInput.OnMouseInput += PlayerInput_Callback;
            
            _messageRouter.Subscribe((ushort)MessageType.Position, this);
        }

        private void PlayerInput_Callback(Vector3 point)
        {
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref IdComponent idComponent = ref _filter.Get1(i);
                if (idComponent.Id == NetworkSystem.Client.Id)
                {
                    ref BodyComponent bodyComponent = ref _filter.Get3(i);
                    ref MoveComponent moveComponent = ref _filter.Get2(i);
                    moveComponent.PrevPosition = bodyComponent.Body.position;
                    moveComponent.TargetPosition = point;
                    moveComponent.Interpolation = 0f;
                    moveComponent.InterpolationFactor =
                        (moveComponent.TargetPosition - moveComponent.PrevPosition).magnitude / moveComponent.Speed;
                    
                    break;
                }
            }

            MoveInputMessage inputMessage = new MoveInputMessage();
            inputMessage.Point = point;
            Message message = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.PointInput);
            inputMessage.Serialize(message);
            
            _messageRouter.Send(message);
            
            message.Release();
        }

        public void Run()
        {
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref IdComponent idComponent = ref _filter.Get1(i);
                ref MoveComponent moveComponent = ref _filter.Get2(i);
                ref BodyComponent bodyComponent = ref _filter.Get3(i);
                
                moveComponent.Interpolation += Time.deltaTime;
                Vector3 newPosition = Vector3.Lerp(moveComponent.PrevPosition, moveComponent.TargetPosition,
                    moveComponent.Interpolation / moveComponent.InterpolationFactor);
                Vector3 deltaPosition = newPosition - bodyComponent.Body.position;
                bodyComponent.Body.MovePosition(newPosition);

                if (idComponent.Id == NetworkSystem.Client.Id)
                {
                    _reconciliationBuffer[_reconciliationCounter % ReconciliationBufferSize] = deltaPosition;
                    _reconciliationCounter++;

                    ReconciliationSyncMessage reconSync = new ReconciliationSyncMessage();
                    reconSync.ReconciliationId = _reconciliationCounter;
                    Message message = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.ReconciliationSync);
                    reconSync.Serialize(message);
                    
                    _messageRouter.Send(message);
                }
            }
        }

        public void HandleMessage(Message message)
        {
            PositionMessage positionMessage = new PositionMessage();
            positionMessage.Deserialize(message);
            
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref IdComponent idComponent = ref _filter.Get1(i);
                if (idComponent.Id == positionMessage.Id)
                {
                    /*ref BodyComponent bodyComponent = ref _filter.Get3(i);
                    ref MoveComponent moveComponent = ref _filter.Get2(i);*/

                    Vector3 position = positionMessage.Position;
                    Debug.Log($"Reconciliate for {_reconciliationCounter - positionMessage.ReconciliationId}");
                    if (idComponent.Id == NetworkSystem.Client.Id)
                    {
                        for (uint j = positionMessage.ReconciliationId + 1; j < _reconciliationCounter; j++)
                        {
                            position += _reconciliationBuffer[j % ReconciliationBufferSize];
                        }
                    }
                    
                    /*if ((bodyComponent.Body.position - position).sqrMagnitude >= AcceptableDesyncEpsilonSqr)
                    {
                        Debug.Log($"Desync {(bodyComponent.Body.position - position).sqrMagnitude}");
                        moveComponent.PrevPosition = positionMessage.Position;
                        moveComponent.InterpolationFactor =
                            (moveComponent.TargetPosition - moveComponent.PrevPosition).magnitude / moveComponent.Speed;
                        moveComponent.Interpolation = 0f;
                    }*/

                    break;
                }
            }
        }

        public void Destroy()
        {
            _messageRouter.Unsubscribe((ushort)MessageType.Position);
            _playerInput.OnMouseInput -= PlayerInput_Callback;
        }
    }
}