using System;
using UnityEngine;

namespace Core
{
    internal sealed class Logger : ILoggerService
    {
        public event Action<string> OnNewMessage;

        public void Log(string message)
        {
            string wrappedMessage = $"<color=blue>INFO:</color> {message}";
            Debug.Log(wrappedMessage);
            OnNewMessage?.Invoke(wrappedMessage);
        }

        public void LogError(string message)
        {
            string wrappedMessage = $"<color=red>ERROR:</color> {message}";
            Debug.Log(wrappedMessage);
            OnNewMessage?.Invoke(wrappedMessage);
        }
    }
}