using Leopotam.Ecs;
using Components;
using Messages;
using MessageStructs;
using UnityEngine;

namespace Systems
{
    public class MoveSystem : IEcsRunSystem, IEcsDestroySystem
    {
        private readonly IMessageRouter _messageRouter;
        private EcsFilter<IdComponent, SpeedComponent, BodyComponent, InterpolateMoveComponent> _filter;

        private readonly IMessageHandler<PositionMessage> _positionMessageHandler;

        public MoveSystem(IMessageRouter messageRouter)
        {
            _messageRouter = messageRouter;
            _positionMessageHandler = _messageRouter.GetHandler<PositionMessage>((ushort)MessageType.Position);
            _positionMessageHandler.OnMessageHandled += PositionMessage_Callback;
        }

        public void Run()
        {
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref BodyComponent bodyComponent = ref _filter.Get3(i);
                ref InterpolateMoveComponent interpolateMoveComponent = ref _filter.Get4(i);
                
                interpolateMoveComponent.InterpolationTime += Time.fixedDeltaTime;
                Vector3 newPosition = Vector3.Lerp(interpolateMoveComponent.PrevPosition, interpolateMoveComponent.TargetPosition,
                    interpolateMoveComponent.InterpolationTime / interpolateMoveComponent.InterpolationFactor);
                
                bodyComponent.Body.MovePosition(newPosition);
            }
        }

        public void PositionMessage_Callback(PositionMessage positionMessage)
        {
            if (positionMessage.Id == NetworkSystem.Client.Id)
            {
                return;
            }
            
            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref IdComponent idComponent = ref _filter.Get1(i);
                ref SpeedComponent speedComponent = ref _filter.Get2(i);
                ref BodyComponent bodyComponent = ref _filter.Get3(i);
                ref InterpolateMoveComponent interpolateMoveComponent = ref _filter.Get4(i);
                if (idComponent.Id == positionMessage.Id)
                {
                    interpolateMoveComponent.PositionsBuffer.Push(positionMessage.Position);
                    interpolateMoveComponent.PositionsBuffer.Pop(out Vector3 prevPosition);
                    interpolateMoveComponent.PositionsBuffer.ResetPeekTail();
                    interpolateMoveComponent.PositionsBuffer.Peek(out Vector3 targetPosition);
                    
                    interpolateMoveComponent.PrevPosition = Vector3.Lerp(bodyComponent.Body.position, prevPosition, 0.2f);
                    interpolateMoveComponent.TargetPosition = targetPosition;
                    interpolateMoveComponent.InterpolationTime = 0f;
                    interpolateMoveComponent.CalculateInterpolationFactor(speedComponent.Speed);
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