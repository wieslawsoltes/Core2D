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
    public class TimerPulseSimulation : BoolSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Key 
        {
            get { return "TIMER-PULSE"; } 
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
                    return new TimerPulseSimulation(false, seconds);
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
        public TimerPulseSimulation()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="delay"></param>
        public TimerPulseSimulation(bool? state, double delay)
            : base()
        {
            base.State = state;
            this.Delay = delay;
        }

        private bool _isEnabled;
        private bool _isReset;
        private long _endCycle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clock"></param>
        public override void Run(Clock clock)
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
                                if (clock.Cycle >= _endCycle && base.State != false)
                                {
                                    _isEnabled = false;
                                    base.State = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (_isReset == true)
                                {
                                    // Delay -> in seconds
                                    // Clock.Cycle
                                    // Clock.Resolution -> in milliseconds
                                    long cyclesDelay = (long)(Delay * 1000.0) / clock.Resolution;
                                    _endCycle = clock.Cycle + cyclesDelay;
                                    _isReset = false;
                                    if (clock.Cycle >= _endCycle)
                                    {
                                        _isEnabled = false;
                                        base.State = false;
                                    }
                                    else
                                    {
                                        _isEnabled = true;
                                        base.State = true;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    case false:
                        {
                            _isReset = true;
                            if (_isEnabled)
                            {
                                if (clock.Cycle >= _endCycle && base.State != false)
                                {
                                    base.State = false;
                                    _isEnabled = false;
                                    break;
                                }
                            }
                        }
                        break;
                    case null:
                        {
                            _isReset = true;
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
