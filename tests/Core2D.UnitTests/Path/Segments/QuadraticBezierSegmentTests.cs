// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path.Segments;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class QuadraticBezierSegmentTests
    {
        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = new QuadraticBezierSegment()
            {
                Point1 = new TestPointShape(),
                Point2 = new TestPointShape()
            };

            var target = segment.GetPoints();

            Assert.Equal(2, target.Count());

            Assert.Contains(segment.Point1, target);
            Assert.Contains(segment.Point2, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = new QuadraticBezierSegment()
            {
                Point1 = new TestPointShape(),
                Point2 = new TestPointShape()
            };

            var actual = target.ToString();

            Assert.Equal("Q0,0 0,0", actual);
        }
    }
}
