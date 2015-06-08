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
    public class OrSimulation : BoolSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Key
        {
            get { return "OR"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Func<XGroup, BoolSimulation> Factory
        {
            get { return (group) => { return new OrSimulation(null, group.GetIntPropertyValue("Counter")); }; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrSimulation()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="counter"></param>
        public OrSimulation(bool? state, int counter)
            : base()
        {
            base.State = state;
            this.Counter = counter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clock"></param>
        public override void Run(Clock clock)
        {
            int length = Inputs.Length;
            if (length == 1)
            {
                base.State = default(bool?);
                return;
            }

            if (Counter <= 0)
            {
                throw new Exception("Or counter must greater than or equal 1.");
            }
            else
            {
                int counter = 0;
                bool? result = default(bool?);
                for (int i = 0; i < length; i++)
                {
                    var input = Inputs[i];
                    if (i == 0)
                    {
                        result = input.IsInverted ? !(input.Simulation.State) : input.Simulation.State;
                        if (result == true)
                        {
                            counter += 1;
                        }
                    }
                    else
                    {
                        bool? value = input.IsInverted ? !(input.Simulation.State) : input.Simulation.State;
                        result |= value;
                        if (value == true)
                        {
                            counter += 1;
                        }
                    }
                }

                if (counter >= Counter)
                {
                    base.State = true;
                }
                else if (result == true)
                {
                    base.State = false;
                }
                else
                {
                    base.State = result;
                }
            }
        }
    }
}
