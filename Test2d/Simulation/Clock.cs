// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Clock
    {
        private object _sync = new object();
        private long _cycle;
        private int _resolution;

        /// <summary>
        /// 
        /// </summary>
        public long Cycle 
        {
            get { return _cycle; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public int Resolution 
        {
            get { return _resolution; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cycle"></param>
        /// <param name="resolution"></param>
        public Clock(long cycle, int resolution)
        {
            this._cycle = cycle;
            this._resolution = resolution;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Tick()
        {
            lock (_sync)
            {
                this._cycle++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            lock (_sync)
            {
                this._cycle = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cycle"></param>
        public void SetCycle(long cycle)
        {
            lock (_sync)
            {
                this._cycle = cycle; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        public void SetResolution(int resolution)
        {
            lock (_sync)
            {
                this._resolution = resolution; 
            }
        }
    }
}
