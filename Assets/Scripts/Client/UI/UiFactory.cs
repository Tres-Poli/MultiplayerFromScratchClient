using UnityEngine;

namespace Common.UI
{
    internal abstract class UiFactory<TView, TController> : IUiFactory where TView : UiView where TController : UiController<TView>
    {
        public abstract ScreenType ScreenType { get; }

        IUiController IUiFactory.AddUiScreen(RectTransform canvas, GameObject prefab)
        {
            return AddUiScreen(canvas, prefab);
        }
        
        public abstract TController AddUiScreen(RectTransform canvas, GameObject prefab);
    }
}