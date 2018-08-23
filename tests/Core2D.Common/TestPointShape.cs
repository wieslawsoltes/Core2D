// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Shapes;

namespace Core2D.Common
{
    public class TestPointShape : TestBaseShape, IPointShape
    {
        public override Type TargetType => typeof(TestPointShape);
        public double X { get; set; }
        public double Y { get; set; }
        public PointAlignment Alignment { get; set; }
        public IBaseShape Shape { get; set; }
    }
}
