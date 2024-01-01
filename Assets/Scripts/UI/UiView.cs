using Core;
using UnityEngine;

namespace UI
{
    public abstract class UiView : MonoBehaviour, IFinite
    {
        public virtual void Initialize()
        {
            
        }

        public abstract void Finite();
    }
}