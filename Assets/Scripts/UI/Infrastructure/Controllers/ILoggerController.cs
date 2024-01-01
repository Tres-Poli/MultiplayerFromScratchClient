using Core;

namespace UI
{
    public interface ILoggerController : IUiController
    {
        void LogEntry(string message);
    }
}