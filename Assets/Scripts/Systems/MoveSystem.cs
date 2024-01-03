using Leopotam.Ecs;
using Components;
using DataStructures;
using Input;
using Messages;
using MessageStructs;
using Riptide;
using UnityEngine;

namespace Systems
{
    public class MoveSystem : IEcsRunSystem, IEcsDestroySystem, IMessageHandler
    {
        private const float ReconciliationInterpolation = 0.2f;
        private const int ReconciliationBufferSize = 1024;
        
        private readonly IMessageRouter _messageRouter;
        private readonly IPlayerInput _playerInput;
        private EcsFilter<IdComponent, MoveComponent, BodyComponent> _filter;

        private uint _reconId;
        private readonly CircularBuffer<ReconNode> _reconBuffer;

        public MoveSystem(IMessageRouter messageRouter, IPlayerInput playerInput)
        {
            _reconId = 0;
            _reconBuffer = new CircularBuffer<ReconNode>(ReconciliationBufferSize);

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
                    //ref BodyComponent bodyComponent = ref _filter.Get3(i);
                    ref MoveComponent moveComponent = ref _filter.Get2(i);
                    //moveComponent.PrevPosition = bodyComponent.Body.position;
                    moveComponent.TargetPosition = point;
                    /*moveComponent.Interpolation = 0f;
                    moveComponent.InterpolationFactor =
                        (moveComponent.TargetPosition - moveComponent.PrevPosition).magnitude / moveComponent.Speed;*/
                    
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
                
                /*moveComponent.Interpolation += Time.fixedDeltaTime;
                Vector3 newPosition = Vector3.Lerp(moveComponent.PrevPosition, moveComponent.TargetPosition,
                    moveComponent.Interpolation / moveComponent.InterpolationFactor);
                Vector3 deltaPosition = newPosition - bodyComponent.Body.position;*/
                Vector3 currPosition = bodyComponent.Body.position;
                Vector3 newPosition;
                if ((bodyComponent.Body.position - moveComponent.TargetPosition).sqrMagnitude <= 0.0025f)
                {
                    bodyComponent.Body.MovePosition(moveComponent.TargetPosition);
                    newPosition = moveComponent.TargetPosition;
                }
                else
                {
                    Vector3 dir = (moveComponent.TargetPosition - currPosition).normalized;
                    newPosition = currPosition + dir * (moveComponent.Speed * Time.fixedDeltaTime);
                    bodyComponent.Body.MovePosition(newPosition);
                }

                Vector3 deltaPosition = newPosition - currPosition;
                deltaPosition.y = 0f;

                if (idComponent.Id == NetworkSystem.Client.Id)
                {
                    _reconBuffer.Push(new ReconNode(_reconId, deltaPosition));

                    ReconciliationSyncMessage reconSync = new ReconciliationSyncMessage();
                    reconSync.ReconciliationId = _reconId++;
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
                ref MoveComponent moveComponent = ref _filter.Get2(i);
                ref BodyComponent bodyComponent = ref _filter.Get3(i);
                if (idComponent.Id == positionMessage.Id)
                {
                    Vector3 position = positionMessage.Position;
                    if (idComponent.Id == NetworkSystem.Client.Id)
                    {
                        while (_reconBuffer.Pop(out ReconNode node) && node.ReconId < positionMessage.ReconciliationId)
                        {
                        }

                        _reconBuffer.ResetPeekTail();
                        while (_reconBuffer.Peek(out ReconNode node))
                        {
                            position += node.DeltaPosition;
                        }

                        bodyComponent.Body.MovePosition(Vector3.Lerp(bodyComponent.Body.position, position, ReconciliationInterpolation));
                        
                        /*moveComponent.PrevPosition = position;//Vector3.Lerp(bodyComponent.Body.position, position, ReconciliationInterpolation);
                        moveComponent.InterpolationFactor =
                            (moveComponent.TargetPosition - moveComponent.PrevPosition).magnitude / moveComponent.Speed;
                        moveComponent.Interpolation = 0f;*/
                    }
                    
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