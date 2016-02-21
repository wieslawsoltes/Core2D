// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path.Segments;
using Core2D.Shapes;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPolyCubicBezierSegmentTests
    {
        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void Points_Not_Null()
        {
            var target = new XPolyCubicBezierSegment();
            Assert.NotNull(target.Points);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = new XPolyCubicBezierSegment();
            segment.Points.Add(new XPoint());
            segment.Points.Add(new XPoint());
            segment.Points.Add(new XPoint());
            segment.Points.Add(new XPoint());
            segment.Points.Add(new XPoint());

            var target = segment.GetPoints();

            Assert.Equal(5, target.Count());

            Assert.Equal(segment.Points, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = new XPolyCubicBezierSegment();
            target.Points.Add(new XPoint());
            target.Points.Add(new XPoint());
            target.Points.Add(new XPoint());
            target.Points.Add(new XPoint());
            target.Points.Add(new XPoint());

            var actual = target.ToString();

            Assert.Equal("C0,0 0,0 0,0 0,0 0,0", actual);
        }
    }
}
