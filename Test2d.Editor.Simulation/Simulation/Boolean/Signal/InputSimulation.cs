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
    public class InputSimulation : BoolSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Key
        {
            get { return "INPUT"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Func<XGroup, BoolSimulation> Factory
        {
            get { return (group) => { return new InputSimulation(false); }; }
        }

        /// <summary>
        /// 
        /// </summary>
        public InputSimulation()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public InputSimulation(bool? state)
            : base()
        {
            base.State = state;
        }

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
            else
            {
                throw new Exception("Input simulation can not have any inputs connected.");
            }
        }
    }
}
