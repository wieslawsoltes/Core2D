// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// Trace message logger.
    /// </summary>
    public class TraceLog : ObservableObject, ILog
    {
        private string _lastMessage;

        /// <inheritdoc/>
        public string LastMessage
        {
            get { return _lastMessage; }
            set { Update(ref _lastMessage, value); }
        }

        /// <inheritdoc/>
        public void Initialize(string path)
        {
            try
            {
                Trace.Listeners.Add(
                    new TextWriterTraceListener(
                        path,
                        "listener"));
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        public void Close()
        {
            try
            {
                Trace.Flush();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        public void LogInformation(string message)
        {
            Trace.TraceInformation(message);
            LastMessage = "Information: " + message;
        }

        /// <inheritdoc/>
        public void LogInformation(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
            LastMessage = "Information: " + string.Format(format, args);
        }

        /// <inheritdoc/>
        public void LogWarning(string message)
        {
            Trace.TraceWarning(message);
            LastMessage = "Warning: " + message;
        }

        /// <inheritdoc/>
        public void LogWarning(string format, params object[] args)
        {
            Trace.TraceWarning(format, args);
            LastMessage = "Warning: " + string.Format(format, args);
        }

        /// <inheritdoc/>
        public void LogError(string message)
        {
            Trace.TraceError(message);
            LastMessage = "Error: " + message;
        }

        /// <inheritdoc/>
        public void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
            LastMessage = "Error: " + string.Format(format, args);
        }
    }
}
