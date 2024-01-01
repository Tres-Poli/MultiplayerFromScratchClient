using System;
using Core;
using UnityEngine;

namespace Input
{
    public sealed class PlayerInput : IPlayerInput, IUpdateController, IFinite
    {
        private readonly Camera _camera;
        private readonly IFinite _updateSubscription;

        public event Action<Vector3> OnMouseInput;
        
        public PlayerInput(Camera camera, ITickController tickController)
        {
            _camera = camera;
            _updateSubscription = tickController.AddController(this);
        }

        public void UpdateController(float deltaTime)
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                Ray ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    OnMouseInput?.Invoke(hitInfo.point);
                }
            }
        }
        
        public void Finite()
        {
            OnMouseInput = null;
            _updateSubscription.Finite();
        }
    }
}