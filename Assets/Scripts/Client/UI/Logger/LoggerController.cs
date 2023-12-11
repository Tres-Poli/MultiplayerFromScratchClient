using Common.UI.Controllers;
using Core;

namespace Common.UI.Logger
{
    public class LoggerController : UiController<LoggerView>, ILoggerController
    {
        private readonly ILoggerService _logger;

        public LoggerController(ILoggerService logger, LoggerView view) : base(view)
        {
            _logger = logger;
            _logger.OnNewMessage += NewMessage_Callback;
        }

        public void LogEntry(string message)
        {
            View.AddEntry(message);
        }

        private void NewMessage_Callback(string message)
        {
            LogEntry(message);
        }

        public override void Finite()
        {
            _logger.OnNewMessage -= NewMessage_Callback;
        }
    }
}