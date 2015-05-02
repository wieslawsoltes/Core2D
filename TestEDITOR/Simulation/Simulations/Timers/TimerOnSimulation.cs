// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace TestSIM
{
    public class TimerOnSimulation : BoolSimulation
    {
        public override string Key
        {
            get { return "TIMER-ON"; }
        }

        //public override Func<XGroup, BoolSimulation> Factory
        //{
        //    get
        //    {
        //        return (group) =>
        //        {
        //            double delay = group.GetDoublePropertyValue("Delay");
        //            string unit = group.GetStringPropertyValue("Unit");
        //            double seconds = delay.ConvertToSeconds(unit);
        //            return new TimerOnSimulation(false, seconds);
        //        };
        //    }
        //}
        public override Func<XGroup, BoolSimulation> Factory
        {
            get
            {
                return (group) =>
                {
                    return new TimerOnSimulation(false, 1.0);
                };
            }
        }

        public double Delay { get; set; }

        public TimerOnSimulation()
            : base()
        {
        }

        public TimerOnSimulation(bool? state, double delay)
            : base()
        {
            base.State = state;
            this.Delay = delay;
        }

        private bool _isEnabled;
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
