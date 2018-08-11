// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Path.Segments;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class ArcSegmentTests
    {
        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = new ArcSegment()
            {
                Point = new TestPointShape(),
                Size = new PathSize() { Width = 10, Height = 20 },
                RotationAngle = 90,
                IsLargeArc = true,
                SweepDirection = SweepDirection.Clockwise
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
            var target = new ArcSegment()
            {
                Point = new TestPointShape(),
                Size = new PathSize() { Width = 10, Height = 20 },
                RotationAngle = 90,
                IsLargeArc = true,
                SweepDirection = SweepDirection.Clockwise
            };

            var actual = target.ToString();

            Assert.Equal("A10,20 90 1 1 0,0", actual);
        }
    }
}
