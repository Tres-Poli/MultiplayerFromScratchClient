using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UI
{
    public interface IUiManager
    {
        UniTask Initialize(RectTransform canvas);
        T AddScreen<T>(ScreenType type) where T : IUiController;
    }
}