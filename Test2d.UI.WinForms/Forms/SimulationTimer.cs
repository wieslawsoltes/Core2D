// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading;
using Test2d;

namespace TestWinForms
{
    /// <summary>
    /// Wrapper class for System.Threading.Timer timer class.
    /// </summary>
    internal class SimulationTimer : ISimulationTimer
    {
        private Timer _timer = default(Timer);

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get { return _timer != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="period"></param>
        public void Start(Action callback, int period)
        {
            _timer = new Timer((_) => callback(), null, 0, period);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _timer.Dispose();
            _timer = default(Timer);
        }
    }
}
