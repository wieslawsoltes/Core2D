// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class XArcSegmentTests
    {
        [Fact]
        [Trait("Core2D", "Path")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = new XArcSegment()
            {
                Point = new XPoint(),
                Size = new XPathSize() { Width = 10, Height = 20 },
                RotationAngle = 90,
                IsLargeArc = true,
                SweepDirection = XSweepDirection.Clockwise
            };

            var target = segment.GetPoints();

            Assert.Equal(1, target.Count());

            Assert.Contains(segment.Point, target);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = new XArcSegment()
            {
                Point = new XPoint(),
                Size = new XPathSize() { Width = 10, Height = 20 },
                RotationAngle = 90,
                IsLargeArc = true,
                SweepDirection = XSweepDirection.Clockwise
            };

            var actual = target.ToString();

            Assert.Equal("A10,20 90 1 1 0,0", actual);
        }
    }
}
