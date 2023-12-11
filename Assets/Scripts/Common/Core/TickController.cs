using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    internal sealed class TickController : MonoBehaviour, ITickController
    {
        private List<IUpdateController> _updateControllers;
        private List<IFixedController> _fixedControllers;

        private Action<IUpdateController> _updateControllerFiniteCallback;
        private Action<IFixedController> _fixedControllerFiniteCallback;

        private void Awake()
        {
            _updateControllers = new List<IUpdateController>(32);
            _fixedControllers = new List<IFixedController>(32);

            _updateControllerFiniteCallback = UpdateControllerFinite_Callback;
            _fixedControllerFiniteCallback = FixedControllerFinite_Callback;
        }

        private void Update()
        {
            for (int i = _updateControllers.Count - 1; i >= 0; i--)
            {
                _updateControllers[i].Update(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            for (int i = _fixedControllers.Count - 1; i >= 0; i--)
            {
                _fixedControllers[i].FixedUpdate(Time.fixedDeltaTime);
            }
        }
        
        public IFinite AddController(IUpdateController updateController)
        {
            ControllerHolder<IUpdateController> holder = new ControllerHolder<IUpdateController>(updateController);
            holder.OnControllerFinite += _updateControllerFiniteCallback;
            _updateControllers.Add(updateController);

            return holder;
        }

        public IFinite AddController(IFixedController fixedController)
        {
            ControllerHolder<IFixedController> holder = new ControllerHolder<IFixedController>(fixedController);
            holder.OnControllerFinite += _fixedControllerFiniteCallback;
            _fixedControllers.Add(fixedController);

            return holder;
        }

        private void UpdateControllerFinite_Callback(IUpdateController controller)
        {
            _updateControllers.Remove(controller);
        }

        private void FixedControllerFinite_Callback(IFixedController controller)
        {
            _fixedControllers.Remove(controller);
        }
    }
}