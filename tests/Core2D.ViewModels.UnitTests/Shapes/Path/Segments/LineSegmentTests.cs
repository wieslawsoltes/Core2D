// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Interfaces;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class LineSegmentTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreateLineSegment(_factory.CreatePointShape(), true, true);

            var target = segment.GetPoints();
            var count = target.Count();

            Assert.Equal(1, count);
            Assert.Contains(segment.Point, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateLineSegment(_factory.CreatePointShape(), true, true);

            var actual = target.ToString();

            Assert.Equal("L0,0", actual);
        }
    }
}
