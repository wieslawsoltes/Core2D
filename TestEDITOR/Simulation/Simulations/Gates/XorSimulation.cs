// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace TestSIM
{
    public class XorSimulation : BoolSimulation
    {
        public override string Key 
        { 
            get { return "XOR"; } 
        }

        public override Func<XGroup, BoolSimulation> Factory
        {
            get { return (group) => { return new XorSimulation(null); }; }
        }

        public XorSimulation()
            : base()
        {
        }

        public XorSimulation(bool? state)
            : base()
        {
            base.State = state;
        }

        public override void Run(IClock clock)
        {
            int length = Inputs.Length;
            if (length == 1)
            {
                base.State = default(bool?);
                return;
            }

            bool? result = default(bool?);
            for (int i = 0; i < length; i++)
            {
                var input = Inputs[i];
                if (i == 0)
                {
                    result = input.IsInverted ? !(input.Simulation.State) : input.Simulation.State;
                }
                else
                {
                    result ^= input.IsInverted ? !(input.Simulation.State) : input.Simulation.State;
                }
            }
            base.State = result;
        }
    }
}
