// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Eto.Forms;
using System;
using Test2d;

namespace TestEtoForms
{
    /// <summary>
    /// Wrapper class for Eto.Forms.UITimer timer class.
    /// </summary>
    internal class SimulationTimer : ISimulationTimer
    {
        private UITimer _timer;

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
            _timer = new UITimer { Interval = period / 1000.0 };
            _timer.Elapsed += delegate { callback(); };
            _timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = default(UITimer);
        }
    }
}
