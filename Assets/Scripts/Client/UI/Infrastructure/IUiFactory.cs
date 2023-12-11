using UnityEngine;

namespace Common.UI
{
    public interface IUiFactory
    {
        ScreenType ScreenType { get; }
        IUiController AddUiScreen(RectTransform canvas, GameObject prefab);
    }
}