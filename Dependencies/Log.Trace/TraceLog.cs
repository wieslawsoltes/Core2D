// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class TraceLog : ObservableObject, ILog
    {
        private string _lastMessage;

        /// <summary>
        ///
        /// </summary>
        public string LastMessage
        {
            get { return _lastMessage; }
            set { Update(ref _lastMessage, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogInformation(string message)
        {
            Trace.TraceInformation(message);
            LastMessage = "Information: " + message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void LogInformation(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
            LastMessage = "Information: " + string.Format(format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogWarning(string message)
        {
            Trace.TraceWarning(message);
            LastMessage = "Warning: " + message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void LogWarning(string format, params object[] args)
        {
            Trace.TraceWarning(format, args);
            LastMessage = "Warning: " + string.Format(format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void LogError(string message)
        {
            Trace.TraceError(message);
            LastMessage = "Error: " + message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
            LastMessage = "Error: " + string.Format(format, args);
        }
    }
}
