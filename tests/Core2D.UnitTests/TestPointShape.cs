// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;

namespace Core2D.UnitTests
{
    public class TestPointShape : TestBaseShape, IPointShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        public PointAlignment Alignment { get; set; }
        public IBaseShape Shape { get; set; }
    }
}
