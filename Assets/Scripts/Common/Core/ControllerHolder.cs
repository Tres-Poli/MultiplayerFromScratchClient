using System;

namespace Core
{
    internal sealed class ControllerHolder<T> : IFinite where T : IController
    {
        private readonly T _controller;
        
        public event Action<T> OnControllerFinite;

        public ControllerHolder(T controller)
        {
            _controller = controller;
        }
        
        public void Finite()
        {
            OnControllerFinite?.Invoke(_controller);
        }
    }
}