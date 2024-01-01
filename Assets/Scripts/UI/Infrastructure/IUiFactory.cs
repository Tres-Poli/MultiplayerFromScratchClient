using UnityEngine;

namespace UI
{
    public interface IUiFactory
    {
        ScreenType ScreenType { get; }
        IUiController AddUiScreen(RectTransform canvas, GameObject prefab);
    }
}