// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Interfaces;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class CubicBezierSegmentTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreateCubicBezierSegment();

            segment.Point1 = _factory.CreatePointShape();
            segment.Point2 = _factory.CreatePointShape();
            segment.Point3 = _factory.CreatePointShape();

            var target = segment.GetPoints();

            Assert.Equal(3, target.Count());

            Assert.Contains(segment.Point1, target);
            Assert.Contains(segment.Point2, target);
            Assert.Contains(segment.Point3, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateCubicBezierSegment()

            target.Point1 = _factory.CreatePointShape();
            target.Point2 = _factory.CreatePointShape();
            target.Point3 = _factory.CreatePointShape();

            var actual = target.ToString();

            Assert.Equal("C0,0 0,0 0,0", actual);
        }
    }
}
