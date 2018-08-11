// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes.Interfaces;

namespace Core2D.UnitTests
{
    public class TestPointShape : TestBaseShape, IPointShape
    {
        public double X { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double Y { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public PointAlignment Alignment { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IBaseShape Shape { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
