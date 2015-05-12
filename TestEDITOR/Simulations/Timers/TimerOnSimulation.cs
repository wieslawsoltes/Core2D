// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace TestSIM
{
    /// <summary>
    /// 
    /// </summary>
    public class TimerOnSimulation : BoolSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Key
        {
            get { return "TIMER-ON"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Func<XGroup, BoolSimulation> Factory
        {
            get
            {
                return (group) =>
                {
                    double delay = group.GetDoublePropertyValue("Delay");
                    string unit = group.GetStringPropertyValue("Unit");
                    double seconds = delay.ConvertToSeconds(unit);
                    return new TimerOnSimulation(false, seconds);
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Delay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimerOnSimulation()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="delay"></param>
        public TimerOnSimulation(bool? state, double delay)
            : base()
        {
            base.State = state;
            this.Delay = delay;
        }

        private bool _isEnabled;
        private long _endCycle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clock"></param>
        public override void Run(IClock clock)
        {
            int length = Inputs.Length;
            if (length == 0)
            {
                // Do nothing.
            }
            else if (length == 1)
            {
                var input = Inputs[0];
                bool? enableState = input.IsInverted ? !(input.Simulation.State) : input.Simulation.State;
                switch (enableState)
                {
                    case true:
                        {
                            if (_isEnabled)
                            {
                                if (clock.Cycle >= _endCycle && base.State != true)
                                {
                                    base.State = true;
                                }
                            }
                            else
                            {
                                // Delay -> in seconds
                                // Clock.Cycle
                                // Clock.Resolution -> in milliseconds
                                long cyclesDelay = (long)(Delay * 1000.0) / clock.Resolution;
                                _endCycle = clock.Cycle + cyclesDelay;
                                _isEnabled = true;
                                if (clock.Cycle >= _endCycle)
                                {
                                    base.State = true;
                                }
                            }
                        }
                        break;
                    case false:
                        {
                            _isEnabled = false;
                            base.State = false;
                        }
                        break;
                    case null:
                        {
                            _isEnabled = false;
                            base.State = default(bool?);
                        }
                        break;
                }
            }
            else
            {
                throw new Exception("TimerOn simulation can only have one input State.");
            }
        }
    }
}
