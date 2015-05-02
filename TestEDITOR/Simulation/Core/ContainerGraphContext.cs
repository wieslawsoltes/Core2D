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
    public class Pin
    {
        public XPoint Point { get; set; }
        public bool IsInverted { get; set; }

        public static Pin Create(XPoint point, bool isInverted)
        {
            return new Pin() 
            { 
                Point = point, 
                IsInverted = isInverted 
            };
        }
    }

    public class ContainerGraphContext
    {
        public IDictionary<XPoint, ICollection<Pin>> Connections { get; set; }
        public IDictionary<XPoint, ICollection<Pin>> Dependencies { get; set; }
        public IDictionary<XPoint, ShapeState> PinTypes { get; set; }
        public IList<XGroup> OrderedGroups { get; set; }
    }
}
