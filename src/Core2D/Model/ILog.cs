using System;

namespace Core2D
{
    public interface ILog : IDisposable
    {
        string LastMessage { get; }

        void Initialize(string path);

        void Close();

        void LogInformation(string message);

        void LogInformation(string format, params object[] args);

        void LogWarning(string message);

        void LogWarning(string format, params object[] args);

        void LogError(string message);

        void LogError(string format, params object[] args);

        void LogException(Exception ex);
    }
}
