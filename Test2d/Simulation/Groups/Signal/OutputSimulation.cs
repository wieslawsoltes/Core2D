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
    public class OutputSimulation : BoolSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Key
        {
            get { return "OUTPUT"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Func<XGroup, BoolSimulation> Factory
        {
            get { return (group) => { return new OutputSimulation(false); }; }
        }

        /// <summary>
        /// 
        /// </summary>
        public OutputSimulation()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public OutputSimulation(bool? state)
            : base()
        {
            base.State = state;
        }

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
                base.State = input.IsInverted ? !(input.Simulation.State) : input.Simulation.State;
            }
            else
            {
                throw new Exception("Output simulation can only have one input State.");
            }
        }
    }
}
