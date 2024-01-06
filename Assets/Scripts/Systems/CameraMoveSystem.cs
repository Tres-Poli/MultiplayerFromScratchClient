using Components;
using Leopotam.Ecs;
using UnityEngine;

namespace Systems
{
    public class CameraMoveSystem : IEcsRunSystem
    {
        private readonly Camera _mainCamera;

        private EcsFilter<CameraComponent, MoveComponent, SpeedComponent> _filter;

        public CameraMoveSystem(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }
        
        public void Run()
        {
            Vector3 moveDelta = Vector3.zero;
            Vector3 mouseInViewPort = _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            if (mouseInViewPort.x <= 0.01f)
            {
                moveDelta.x = -1;
            }
            else if (mouseInViewPort.x >= 0.99f)
            {
                moveDelta.x = 1;
            }

            if (mouseInViewPort.y <= 0.01f)
            {
                moveDelta.z = -1;
            }
            else if (mouseInViewPort.y >= 0.99f)
            {
                moveDelta.z = 1;
            }

            for (int i = 0; i < _filter.GetEntitiesCount(); i++)
            {
                ref CameraComponent cameraComponent = ref _filter.Get1(i);
                ref MoveComponent moveComponent = ref _filter.Get2(i);
                moveComponent.TargetPosition = cameraComponent.Camera.transform.position + moveDelta;
            }
        }
    }
}