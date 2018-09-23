// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Linq;
using Core2D.Interfaces;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class PolyQuadraticBezierSegmentTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void Points_Not_Null()
        {
            var target = _factory.CreatePolyQuadraticBezierSegment(ImmutableArray.Create<IPointShape>(), true, true);
            Assert.False(target.Points.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreatePolyQuadraticBezierSegment(ImmutableArray.Create<IPointShape>(), true, true);

            segment.Points = segment.Points.Add(_factory.CreatePointShape());
            segment.Points = segment.Points.Add(_factory.CreatePointShape());
            segment.Points = segment.Points.Add(_factory.CreatePointShape());
            segment.Points = segment.Points.Add(_factory.CreatePointShape());
            segment.Points = segment.Points.Add(_factory.CreatePointShape());

            var target = segment.GetPoints();
            var count = target.Count();

            Assert.Equal(5, count);

            Assert.Equal(segment.Points, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = _factory.CreatePolyQuadraticBezierSegment(ImmutableArray.Create<IPointShape>(), true, true);

            target.Points = target.Points.Add(_factory.CreatePointShape());
            target.Points = target.Points.Add(_factory.CreatePointShape());
            target.Points = target.Points.Add(_factory.CreatePointShape());
            target.Points = target.Points.Add(_factory.CreatePointShape());
            target.Points = target.Points.Add(_factory.CreatePointShape());

            var actual = target.ToString();

            Assert.Equal("Q0,0 0,0 0,0 0,0 0,0", actual);
        }
    }
}
