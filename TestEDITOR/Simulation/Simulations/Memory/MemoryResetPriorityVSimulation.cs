// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace TestSIM
{
    public class MemoryResetPriorityVSimulation : BoolSimulation
    {
        public override string Key
        {
            get { return "SR-RESET-V"; }
        }

        public override Func<XGroup, BoolSimulation> Factory
        {
            get { return (group) => { return new MemoryResetPriorityVSimulation(); }; }
        }

        public override void Run(IClock clock)
        {
            // TODO: Implement simulation
        }
    }
}
