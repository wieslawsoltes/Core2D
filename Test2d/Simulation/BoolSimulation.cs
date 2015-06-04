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
    public abstract class BoolSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string Key { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract Func<XGroup, BoolSimulation> Factory { get; }
        /// <summary>
        /// 
        /// </summary>
        public BoolInput[] Inputs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clock"></param>
        public abstract void Run(IClock clock);
    }
}
