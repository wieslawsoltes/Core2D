// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path.Segments;
using Core2D.Shapes;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class LineSegmentTests
    {
        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = new LineSegment()
            {
                Point = new PointShape()
            };

            var target = segment.GetPoints();
            var count = target.Count();

            Assert.Equal(1, count);
            Assert.Contains(segment.Point, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = new LineSegment()
            {
                Point = new PointShape()
            };

            var actual = target.ToString();

            Assert.Equal("L0,0", actual);
        }
    }
}
