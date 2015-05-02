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
    public class ContainerGraphContext
    {
        public IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> Connections { get; set; }
        public IDictionary<XPoint, ICollection<Tuple<XPoint, bool>>> Dependencies { get; set; }
        public IDictionary<XPoint, ShapeState> PinTypes { get; set; }
        public IList<XGroup> OrderedGroups { get; set; }
    }
}
