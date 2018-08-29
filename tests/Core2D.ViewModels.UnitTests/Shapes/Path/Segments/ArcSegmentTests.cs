// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class ArcSegmentTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreateArcSegment();

            segment.Point = _factory.CreatePointShape();
            segment.Size = _factory.CreatePathSize();
            segment.Size.Width = 10;
            segment.Size.Height = 20;
            segment.RotationAngle = 90;
            segment.IsLargeArc = true;
            segment.SweepDirection = SweepDirection.Clockwise;

            var target = segment.GetPoints();
            var count = target.Count();

            Assert.Equal(1, count);
            Assert.Contains(segment.Point, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateArcSegment();

            target.Point = _factory.CreatePointShape();
            target.Size = _factory.CreatePathSize();
            target.Size.Width = 10;
            target.Size.Height = 20;
            target.RotationAngle = 90;
            target.IsLargeArc = true;
            target.SweepDirection = SweepDirection.Clockwise;

            var actual = target.ToString();

            Assert.Equal("A10,20 90 1 1 0,0", actual);
        }
    }
}
