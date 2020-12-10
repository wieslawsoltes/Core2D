// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Model;
using SD = System.Diagnostics;

namespace Core2D.Log.Trace
{
    public sealed class TraceLog : ILog
    {
        private readonly IServiceProvider _serviceProvider;

        private const string InformationPrefix = "Information: ";
        private const string WarningPrefix = "Warning: ";
        private const string ErrorPrefix = "Error: ";

        private string _lastMessage;
        private SD.TraceListener _listener;
        private System.IO.Stream _stream;

        public TraceLog(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string LastMessage => _lastMessage;

        private void SetLastMessage(string message) => _lastMessage = message;

        public void Initialize(string path)
        {
            try
            {
                Close();

                _stream = new System.IO.FileStream(path, System.IO.FileMode.Append);
                _listener = new SD.TextWriterTraceListener(_stream, "listener");

                SD.Trace.Listeners.Add(_listener);
            }
            catch (Exception ex)
            {
                SD.Debug.WriteLine(ex.Message);
                SD.Debug.WriteLine(ex.StackTrace);
            }
        }

        public void Close()
        {
            try
            {
                SD.Trace.Flush();

                if (_listener != null)
                {
                    _listener.Dispose();
                    _listener = null;
                }

                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }
            catch (Exception ex)
            {
                SD.Debug.WriteLine(ex.Message);
                SD.Debug.WriteLine(ex.StackTrace);
            }
        }

        public void LogInformation(string message)
        {
            SD.Trace.TraceInformation(message);
            Console.WriteLine(message);
            SetLastMessage(InformationPrefix + message);
        }

        public void LogInformation(string format, params object[] args)
        {
            SD.Trace.TraceInformation(format, args);
            Console.WriteLine(format, args);
            SetLastMessage(InformationPrefix + string.Format(format, args));
        }

        public void LogWarning(string message)
        {
            SD.Trace.TraceWarning(message);
            Console.WriteLine(message);
            SetLastMessage(WarningPrefix + message);
        }

        public void LogWarning(string format, params object[] args)
        {
            SD.Trace.TraceWarning(format, args);
            Console.WriteLine(format, args);
            SetLastMessage(WarningPrefix + string.Format(format, args));
        }

        public void LogError(string message)
        {
            SD.Trace.TraceError(message);
            Console.WriteLine(message);
            SetLastMessage(ErrorPrefix + message);
        }

        public void LogError(string format, params object[] args)
        {
            SD.Trace.TraceError(format, args);
            Console.WriteLine(format, args);
            SetLastMessage(ErrorPrefix + string.Format(format, args));
        }

        public void LogException(Exception ex)
        {
            LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            if (ex.InnerException != null)
            {
                LogException(ex.InnerException);
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
