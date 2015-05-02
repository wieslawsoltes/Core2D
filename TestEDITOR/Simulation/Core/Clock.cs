// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSIM
{
    public class Clock : IClock
    {
        private object _sync = new object();

        private long _cycle;
        public long Cycle 
        {
            get { return _cycle; } 
        }

        private int _resolution;
        public int Resolution 
        {
            get { return _resolution; }
        }

        public Clock(long cycle, int resolution)
        {
            this._cycle = cycle;
            this._resolution = resolution;
        }

        public void Tick()
        {
            lock (_sync)
            {
                this._cycle++;
            }
        }

        public void Reset()
        {
            lock (_sync)
            {
                this._cycle = 0;
            }
        }

        public void SetCycle(long cycle)
        {
            lock (_sync)
            {
                this._cycle = cycle; 
            }
        }

        public void SetResolution(int resolution)
        {
            lock (_sync)
            {
                this._resolution = resolution; 
            }
        }
    }
}
