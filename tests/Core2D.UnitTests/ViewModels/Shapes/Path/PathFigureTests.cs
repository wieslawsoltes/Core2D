using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D;
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

            var segment1 = new TestSegmentViewModel() { Point = _factory.CreatePointShape() };
            figure.Segments = figure.Segments.Add(segment1);

            var segment2 = new TestSegmentViewModel() { Point = _factory.CreatePointShape() };
            figure.Segments = figure.Segments.Add(segment2);

            var target = new List<PointShapeViewModel>();
            figure.GetPoints(target);

            Assert.Equal(3, target.Count());

            Assert.Contains(figure.StartPoint, target);
            Assert.Contains(segment1.Point, target);
            Assert.Contains(segment2.Point, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToXamlString_Should_Return_Empty()
        {
            var figure = _factory.CreatePathFigure();

            var target = ImmutableArray.Create<PathSegmentViewModel>();
            var actual = (figure as PathFigureViewModel).ToXamlString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToSvgString_Should_Return_Empty()
        {
            var figure = _factory.CreatePathFigure();

            var target = ImmutableArray.Create<PathSegmentViewModel>();
            var actual = (figure as PathFigureViewModel).ToSvgString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToXamlString_Should_Return_Path_Markup_Empty_Not_Closed()
        {
            var target = _factory.CreatePathFigure();

            var actual = target.ToXamlString();

            Assert.Equal("M0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToSvgString_Should_Return_Path_Markup_Empty_Not_Closed()
        {
            var target = _factory.CreatePathFigure();

            var actual = target.ToSvgString();

            Assert.Equal("M0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToXamlString_Should_Return_Path_Markup_Empty_Closed()
        {
            var target = _factory.CreatePathFigure();

            target.IsClosed = true;

            var actual = target.ToXamlString();

            Assert.Equal("M0,0z", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToSvgString_Should_Return_Path_Markup_Empty_Closed()
        {
            var target = _factory.CreatePathFigure();

            target.IsClosed = true;

            var actual = target.ToSvgString();

            Assert.Equal("M0,0z", actual);
        }

        public class TestSegmentViewModel : PathSegmentViewModel
        {
            public PointShapeViewModel Point { get; set; }

            public override void GetPoints(IList<PointShapeViewModel> points)
            {
                points.Add(Point);
            }

            public override object Copy(IDictionary<object, object> shared)
            {
                throw new NotImplementedException();
            }

            public override string ToXamlString()
            {
                throw new NotImplementedException();
            }

            public override string ToSvgString()
            {
                throw new NotImplementedException();
            }
        }
    }
}
