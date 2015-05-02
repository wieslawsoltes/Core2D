// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace TestSIM
{
    public class TimerPulseSimulation : BoolSimulation
    {
        public override string Key 
        {
            get { return "TIMER-PULSE"; } 
        }

        //public override Func<XGroup, BoolSimulation> Factory
        //{
        //    get 
        //    {
        //        return (block) =>
        //        {
        //            double delay = block.GetDoublePropertyValue("Delay");
        //            string unit = block.GetStringPropertyValue("Unit");
        //            double seconds = delay.ConvertToSeconds(unit);
        //            return new TimerPulseSimulation(false, seconds);
        //        };
        //    }
        //}
        public override Func<XGroup, BoolSimulation> Factory
        {
            get
            {
                return (block) =>
                {
                    return new TimerPulseSimulation(false, 1.0);
                };
            }
        }

        public double Delay { get; set; }

        public TimerPulseSimulation()
            : base()
        {
        }

        public TimerPulseSimulation(bool? state, double delay)
            : base()
        {
            base.State = state;
            this.Delay = delay;
        }

        private bool _isEnabled;
        private bool _isReset;
        private long _endCycle;

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
                            base.State = null;
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
