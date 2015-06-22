// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISimulationTimer
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="period"></param>
        void Start(Action callback, int period);

        /// <summary>
        /// 
        /// </summary>
        void Stop();
    }
}
