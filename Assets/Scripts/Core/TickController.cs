using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    internal sealed class TickController : MonoBehaviour, ITickController
    {
        public static float TimeStep = 0.1f;

        private List<IUpdateController> _updateControllers;
        private List<IFixedController> _fixedControllers;
        private List<ITimestampController> _timestampControllers;
        
        private Action<IUpdateController> _updateControllerFiniteCallback;
        private Action<IFixedController> _fixedControllerFiniteCallback;
        private Action<ITimestampController> _timestampControllerFiniteCallback;

        private float _timeStep;

        private void Awake()
        {
            _updateControllers = new List<IUpdateController>(32);
            _fixedControllers = new List<IFixedController>(32);
            _timestampControllers = new List<ITimestampController>(32);
            _updateControllerFiniteCallback = UpdateControllerFinite_Callback;
            _fixedControllerFiniteCallback = FixedControllerFinite_Callback;
            _timestampControllerFiniteCallback = TimestempControllerFinite_Callback;

            _timeStep = TimeStep;
        }

        private void Update()
        {
            for (int i = _updateControllers.Count - 1; i >= 0; i--)
            {
                _updateControllers[i].UpdateController(Time.deltaTime);
            }

            _timeStep -= Time.deltaTime;
            if (_timeStep <= 0f)
            {
                for (int i = 0; i < _timestampControllers.Count; i++)
                {
                    _timestampControllers[i].UpdateController();
                }

                _timeStep = TimeStep;
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

        public IFinite AddController(ITimestampController timestampController)
        {
            ControllerHolder<ITimestampController> holder = new ControllerHolder<ITimestampController>(timestampController);
            holder.OnControllerFinite += _timestampControllerFiniteCallback;
            _timestampControllers.Add(timestampController);

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

        private void TimestempControllerFinite_Callback(ITimestampController controller)
        {
            _timestampControllers.Remove(controller);
        }
    }
}