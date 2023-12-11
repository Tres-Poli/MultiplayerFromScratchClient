using System;

namespace Core
{
    public interface ILoggerService
    {
        event Action<string> OnNewMessage;
        void Log(string message);
        void LogError(string message);
    }
}