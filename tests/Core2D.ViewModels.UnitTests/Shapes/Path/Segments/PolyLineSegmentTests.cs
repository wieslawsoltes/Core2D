// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path.Segments;
using Core2D.Shapes;
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class PolyLineSegmentTests
    {
        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void Points_Not_Null()
        {
            var target = new PolyLineSegment();
            Assert.False(target.Points.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = new PolyLineSegment();
            segment.Points = segment.Points.Add(new PointShape());
            segment.Points = segment.Points.Add(new PointShape());
            segment.Points = segment.Points.Add(new PointShape());
            segment.Points = segment.Points.Add(new PointShape());
            segment.Points = segment.Points.Add(new PointShape());

            var target = segment.GetPoints();
            var count = target.Count();

            Assert.Equal(5, count);

            Assert.Equal(segment.Points, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = new PolyLineSegment();
            target.Points = target.Points.Add(new PointShape());
            target.Points = target.Points.Add(new PointShape());
            target.Points = target.Points.Add(new PointShape());
            target.Points = target.Points.Add(new PointShape());
            target.Points = target.Points.Add(new PointShape());

            var actual = target.ToString();

            Assert.Equal("L0,0 0,0 0,0 0,0 0,0", actual);
        }
    }
}
