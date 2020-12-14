using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UnitTests.ViewModels.Path.Segments
{
    public class CubicBezierSegmentTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreateCubicBezierSegment(_factory.CreatePointShape(), _factory.CreatePointShape(), _factory.CreatePointShape());

            var target = new List<PointShapeViewModel>();
            segment.GetPoints(target);

            Assert.Equal(3, target.Count());

            Assert.Contains(segment.Point1, target);
            Assert.Contains(segment.Point2, target);
            Assert.Contains(segment.Point3, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToXamlString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateCubicBezierSegment(_factory.CreatePointShape(), _factory.CreatePointShape(), _factory.CreatePointShape());

            var actual = target.ToXamlString();

            Assert.Equal("C0,0 0,0 0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToSvgString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateCubicBezierSegment(_factory.CreatePointShape(), _factory.CreatePointShape(), _factory.CreatePointShape());

            var actual = target.ToSvgString();

            Assert.Equal("C0,0 0,0 0,0", actual);
        }
    }
}
