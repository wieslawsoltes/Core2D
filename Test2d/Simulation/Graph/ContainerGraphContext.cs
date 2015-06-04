// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestSIM
{
    /// <summary>
    /// 
    /// </summary>
    public class ContainerGraphContext
    {
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<XPoint, ICollection<Pin>> Connections { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<XPoint, ICollection<Pin>> Dependencies { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<XPoint, ShapeState> PinTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<XGroup> OrderedGroups { get; set; }
    }
}
