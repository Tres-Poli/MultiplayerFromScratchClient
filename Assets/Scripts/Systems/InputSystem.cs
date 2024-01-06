using Components;
using Leopotam.Ecs;
using Messages;
using Riptide;
using UnityEngine;

namespace Systems
{
    public sealed class InputSystem : IEcsRunSystem
    {
        private readonly Camera _mainCamera;
        private readonly IMessageRouter _messageRouter;
        private readonly int _layerMask;
        private EcsFilter<PlayerInputComponent, MoveComponent> _filter;

        public InputSystem(Camera mainCamera, IMessageRouter messageRouter)
        {
            _mainCamera = mainCamera;
            _messageRouter = messageRouter;
            _layerMask = 1 << LayerMask.NameToLayer("Navigation");
        }
        
        public void Run()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, _layerMask))
                {
                    for (int i = 0; i < _filter.GetEntitiesCount(); i++)
                    {
                        ref MoveComponent moveComponent = ref _filter.Get2(i);
                        moveComponent.TargetPosition = hitInfo.point;
                        
                        MoveInputMessage inputMessage = new MoveInputMessage();
                        inputMessage.Point = hitInfo.point;
                        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)MessageType.DirectionInput);
                        inputMessage.Serialize(message);
            
                        _messageRouter.Send(message);
            
                        message.Release();
                    }
                }
            }
        }
    }
}