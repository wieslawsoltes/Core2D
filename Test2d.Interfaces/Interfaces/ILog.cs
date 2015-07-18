// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 
        /// </summary>
        string LastMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        void Initialize(string path);

        /// <summary>
        /// 
        /// </summary>
        void Close();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogInformation(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void LogInformation(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogWarning(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void LogWarning(string format, params object[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void LogError(string format, params object[] args);
    }
}
