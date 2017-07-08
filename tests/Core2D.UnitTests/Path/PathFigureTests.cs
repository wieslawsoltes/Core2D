// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Path;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathFigureTests
    {
        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void StartPoint_Not_Null()
        {
            var target = new PathFigure();
            Assert.NotNull(target.StartPoint);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Segments_Not_Null()
        {
            var target = new PathFigure();
            Assert.NotNull(target.Segments);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void IsFilled_By_Default_Is_False()
        {
            var target = new PathFigure();
            Assert.False(target.IsFilled);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void IsClosed_By_Default_Is_False()
        {
            var target = new PathFigure();
            Assert.False(target.IsClosed);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var figure = new PathFigure();

            var segment1 = new Class1() { Point = new PointShape() };
            figure.Segments = figure.Segments.Add(segment1);

            var segment2 = new Class1() { Point = new PointShape() };
            figure.Segments = figure.Segments.Add(segment2);

            var target = figure.GetPoints();

            Assert.Equal(3, target.Count());

            Assert.Contains(figure.StartPoint, target);
            Assert.Contains(segment1.Point, target);
            Assert.Contains(segment2.Point, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Empty()
        {
            var figure = new PathFigure();

            var target = ImmutableArray.Create<PathSegment>();
            var actual = figure.ToString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_Not_Closed()
        {
            var target = new PathFigure();

            var actual = target.ToString();

            Assert.Equal("M0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_Closed()
        {
            var target = new PathFigure() { IsClosed = true };

            var actual = target.ToString();

            Assert.Equal("M0,0z", actual);
        }

        public class Class1 : PathSegment
        {
            public PointShape Point { get; set; }

            public override IEnumerable<PointShape> GetPoints()
            {
                yield return Point;
            }
        }
    }
}
