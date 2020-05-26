using System.Collections.Generic;
using System.Linq;
using Core2D;
using Core2D.Shapes;
using Xunit;

namespace Core2D.UnitTests
{
    public class QuadraticBezierSegmentTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreateQuadraticBezierSegment(_factory.CreatePointShape(), _factory.CreatePointShape(), true, true);

            var target = new List<IPointShape>();
            segment.GetPoints(target);

            Assert.Equal(2, target.Count());

            Assert.Contains(segment.Point1, target);
            Assert.Contains(segment.Point2, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToXamlString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateQuadraticBezierSegment(_factory.CreatePointShape(), _factory.CreatePointShape(), true, true);

            var actual = target.ToXamlString();

            Assert.Equal("Q0,0 0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToSvgString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateQuadraticBezierSegment(_factory.CreatePointShape(), _factory.CreatePointShape(), true, true);

            var actual = target.ToSvgString();

            Assert.Equal("Q0,0 0,0", actual);
        }
    }
}
