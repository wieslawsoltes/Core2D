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
    public class TimerOffSimulation : BoolSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Key
        {
            get { return "TIMER-OFF"; }
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
                    return new TimerOffSimulation(false, seconds);
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
        public TimerOffSimulation()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="delay"></param>
        public TimerOffSimulation(bool? state, double delay)
            : base()
        {
            base.State = state;
            this.Delay = delay;
        }

        private bool _isEnabled;
        private bool _isLowEnabled;
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
                            if (_isEnabled == false && _isLowEnabled == false)
                            {
                                base.State = true;
                                _isEnabled = true;
                                _isLowEnabled = false;
                            }
                            else if (_isEnabled == true && _isLowEnabled == true && base.State != false)
                            {
                                if (clock.Cycle >= _endCycle)
                                {
                                    base.State = false;
                                    _isEnabled = false;
                                    _isLowEnabled = false;
                                    break;
                                }
                            }
                        }
                        break;
                    case false:
                        {
                            if (_isEnabled == true && _isLowEnabled == false)
                            {
                                // Delay -> in seconds
                                // Clock.Cycle
                                // Clock.Resolution -> in milliseconds
                                long cyclesDelay = (long)(Delay * 1000.0) / clock.Resolution;
                                _endCycle = clock.Cycle + cyclesDelay;
                                _isLowEnabled = true;
                                break;
                            }
                            else if (_isEnabled == true && _isLowEnabled == true && base.State != false)
                            {
                                if (clock.Cycle >= _endCycle)
                                {
                                    base.State = false;
                                    _isEnabled = false;
                                    _isLowEnabled = false;
                                    break;
                                }
                            }
                        }
                        break;
                    case null:
                        {
                            _isEnabled = false;
                            _isLowEnabled = false;
                            base.State = default(bool?);
                        }
                        break;
                }
            }
            else
            {
                throw new Exception("TimerOff simulation can only have one input State.");
            }
        }
    }
}
