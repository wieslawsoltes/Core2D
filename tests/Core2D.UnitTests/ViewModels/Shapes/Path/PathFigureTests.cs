using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathFigureTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void StartPoint_Not_Null()
        {
            var target = _factory.CreatePathFigure();
            Assert.NotNull(target.StartPoint);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Segments_Not_Null()
        {
            var target = _factory.CreatePathFigure();
            Assert.False(target.Segments.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void IsFilled_By_Default_Is_False()
        {
            var target = _factory.CreatePathFigure();
            Assert.False(target.IsFilled);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void IsClosed_By_Default_Is_True()
        {
            var target = _factory.CreatePathFigure();
            Assert.False(target.IsClosed);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var figure = _factory.CreatePathFigure();

            var segment1 = new TestSegment() { Point = _factory.CreatePointShape() };
            figure.Segments = figure.Segments.Add(segment1);

            var segment2 = new TestSegment() { Point = _factory.CreatePointShape() };
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
            var figure = _factory.CreatePathFigure();

            var target = ImmutableArray.Create<IPathSegment>();
            var actual = (figure as PathFigure).ToString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_Not_Closed()
        {
            var target = _factory.CreatePathFigure();

            var actual = target.ToString();

            Assert.Equal("M0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Path_Markup_Empty_Closed()
        {
            var target = _factory.CreatePathFigure();

            target.IsClosed = true;

            var actual = target.ToString();

            Assert.Equal("M0,0z", actual);
        }

        public class TestSegment : PathSegment
        {
            public IPointShape Point { get; set; }

            public override IEnumerable<IPointShape> GetPoints()
            {
                yield return Point;
            }

            /// <inheritdoc/>
            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }
        }
    }
}
