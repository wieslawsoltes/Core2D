using System;

namespace Core2D
{
    /// <summary>
    /// Defines logger contract.
    /// </summary>
    public interface ILog : IDisposable
    {
        /// <summary>
        /// Gets or sets last log message.
        /// </summary>
        string LastMessage { get; }

        /// <summary>
        /// Initialize logger.
        /// </summary>
        /// <param name="path">The log file path.</param>
        void Initialize(string path);

        /// <summary>
        /// Close logger.
        /// </summary>
        void Close();

        /// <summary>
        /// Log information message.
        /// </summary>
        /// <param name="message">The log message.</param>
        void LogInformation(string message);

        /// <summary>
        /// Log formatted information message.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message arguments.</param>
        void LogInformation(string format, params object[] args);

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message">The log message.</param>
        void LogWarning(string message);

        /// <summary>
        /// Log formatted warning message.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message arguments.</param>
        void LogWarning(string format, params object[] args);

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="message">The log message.</param>
        void LogError(string message);

        /// <summary>
        /// Log formatted error message.
        /// </summary>
        /// <param name="format">The string format.</param>
        /// <param name="args">The message arguments.</param>
        void LogError(string format, params object[] args);

        /// <summary>
        /// Log exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        void LogException(Exception ex);
    }
}
