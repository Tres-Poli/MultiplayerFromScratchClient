using Components;
using DataStructures;
using Leopotam.Ecs;
using Messages;
using MessageStructs;
using Riptide;
using UnityEngine;

namespace Systems
{
    public class MoveReconciliationSystem : IEcsRunSystem, IEcsDestroySystem
    {
        private const float ReconciliationInterpolation = 0.2f;
        private const int ReconciliationBufferSize = 1024;
        
        private readonly IMessageRouter _messageRouter;
        private EcsFilter<IdComponent, MoveComponent, SpeedComponent, BodyComponent> _filter;

        private uint _reconId;
        private readonly CircularBuffer<ReconNode> _reconBuffer;

        private IMessageHandler<PositionMessage> _positionMessageHandler;

        public MoveReconciliationSystem(IMessageRouter messageRouter)
        {
            _reconId = 0;
            _reconBuffer = new CircularBuffer<ReconNode>(ReconciliationBufferSize);
            _messageRouter = messageRouter;

            _positionMessageHandler = messageRouter.GetHandler<PositionMessage>((ushort)MessageType.Position);
            _positionMessageHandler.OnMessageHandled += PositionMessage_Callback;
        }
        
        public void Run()
        {
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref MoveComponent moveComponent = ref _filter.Get2(i);
                ref SpeedComponent speedComponent = ref _filter.Get3(i);
                ref BodyComponent bodyComponent = ref _filter.Get4(i);

                Vector3 position = bodyComponent.Body.position;
                if ((position - moveComponent.TargetPosition).sqrMagnitude <= 0.025f)
                {
                    bodyComponent.Body.position = moveComponent.TargetPosition;
                }
                else
                {
                    Vector3 dir = (moveComponent.TargetPosition - position).normalized;
                    bodyComponent.Body.position = position + dir * (speedComponent.Speed * Time.fixedDeltaTime);
                }

                _reconBuffer.Push(new ReconNode(_reconId, bodyComponent.Body.position - position));

                ReconciliationSyncMessage reconSync = new ReconciliationSyncMessage
                {
                    ReconciliationId = _reconId++
                };
                
                Message message = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.ReconciliationSync);
                reconSync.Serialize(message);
                
                _messageRouter.Send(message);
            }
        }

        private void PositionMessage_Callback(PositionMessage positionMessage)
        {
            if (positionMessage.Id != NetworkSystem.Client.Id)
            {
                return;
            }
            
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref IdComponent idComponent = ref _filter.Get1(i);
                if (idComponent.Id == positionMessage.Id)
                {
                    ref BodyComponent bodyComponent = ref _filter.Get4(i);

                    Vector3 position = positionMessage.Position;
                    _reconBuffer.ResetPeekTail();
                    while (_reconBuffer.Peek(out ReconNode node) && node.ReconId <= positionMessage.ReconciliationId)
                    {
                        _reconBuffer.Pop(out ReconNode popNode);
                    }

                    _reconBuffer.ResetPeekTail();
                    while (_reconBuffer.Peek(out ReconNode node))
                    {
                        position += node.DeltaPosition;
                    }
                    
                    bodyComponent.Body.position = Vector3.Lerp(bodyComponent.Body.position, position, ReconciliationInterpolation);
                    break;
                }
            }
        }

        public void Destroy()
        {
            _positionMessageHandler.OnMessageHandled -= PositionMessage_Callback;
        }
    }
}