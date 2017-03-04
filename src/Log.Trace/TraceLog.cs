// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D;
using Core2D.Interfaces;
using System;
using SD = System.Diagnostics;

namespace Log.Trace
{
    /// <summary>
    /// Trace message logger.
    /// </summary>
    public sealed class TraceLog : ObservableObject, ILog
    {
        private const string InformationPrefix = "Information: ";
        private const string WarningPrefix = "Warning: ";
        private const string ErrorPrefix = "Error: ";

        private string _lastMessage;

        /// <inheritdoc/>
        string ILog.LastMessage
        {
            get { return _lastMessage; }
        }

        void SetLastMessage(string message)
        {
            _lastMessage = message;
            Notify("LastMessage");
        }

        /// <inheritdoc/>
        void ILog.Initialize(string path)
        {
            try
            {
                SD.Trace.Listeners.Add(new SD.TextWriterTraceListener(path, "listener"));
            }
            catch (Exception ex)
            {
                SD.Debug.WriteLine(ex.Message);
                SD.Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        void ILog.Close()
        {
            try
            {
                SD.Trace.Flush();
            }
            catch (Exception ex)
            {
                SD.Debug.WriteLine(ex.Message);
                SD.Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        void ILog.LogInformation(string message)
        {
            SD.Trace.TraceInformation(message);
            SetLastMessage(InformationPrefix + message);
        }

        /// <inheritdoc/>
        void ILog.LogInformation(string format, params object[] args)
        {
            SD.Trace.TraceInformation(format, args);
            SetLastMessage(InformationPrefix + string.Format(format, args));
        }

        /// <inheritdoc/>
        void ILog.LogWarning(string message)
        {
            SD.Trace.TraceWarning(message);
            SetLastMessage(WarningPrefix + message);
        }

        /// <inheritdoc/>
        void ILog.LogWarning(string format, params object[] args)
        {
            SD.Trace.TraceWarning(format, args);
            SetLastMessage(WarningPrefix + string.Format(format, args));
        }

        /// <inheritdoc/>
        void ILog.LogError(string message)
        {
            SD.Trace.TraceError(message);
            SetLastMessage(ErrorPrefix + message);
        }

        /// <inheritdoc/>
        void ILog.LogError(string format, params object[] args)
        {
            SD.Trace.TraceError(format, args);
            SetLastMessage(ErrorPrefix + string.Format(format, args));
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        ~TraceLog()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                (this as ILog).Close();
            }
        }
    }
}
