using Core;
using UnityEngine;

namespace Common.UI.Logger
{
    internal sealed class LoggerFactory : UiFactory<LoggerView, LoggerController>
    {
        private readonly ILoggerService _logger;

        public override ScreenType ScreenType => ScreenType.Logger;
        public LoggerFactory(ILoggerService logger)
        {
            _logger = logger;
        }


        public override LoggerController AddUiScreen(RectTransform canvas, GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab, canvas);
            ((RectTransform)obj.transform).anchoredPosition = Vector2.zero;
            
            LoggerView view = obj.GetComponent<LoggerView>();
            return new LoggerController(_logger, view);
        }
    }
}