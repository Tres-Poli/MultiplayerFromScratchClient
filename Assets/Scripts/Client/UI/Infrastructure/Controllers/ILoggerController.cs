using Core;

namespace Common.UI.Controllers
{
    public interface ILoggerController : IUiController
    {
        void LogEntry(string message);
    }
}