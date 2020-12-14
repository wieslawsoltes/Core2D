using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UnitTests.ViewModels.Path.Segments
{
    public class ArcSegmentTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = _factory.CreateArcSegment(_factory.CreatePointShape(), _factory.CreatePathSize(), 180, true, SweepDirection.Clockwise);

            var target = new List<PointShapeViewModel>();
            segment.GetPoints(target);
            var count = target.Count();

            Assert.Equal(1, count);
            Assert.Contains(segment.Point, target);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToXamlString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateArcSegment(_factory.CreatePointShape(), _factory.CreatePathSize(), 90, true, SweepDirection.Clockwise);

            target.Size.Width = 10;
            target.Size.Height = 20;

            var actual = target.ToXamlString();

            Assert.Equal("A10,20 90 1 1 0,0", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Segments")]
        public void ToSvgString_Should_Return_Path_Markup()
        {
            var target = _factory.CreateArcSegment(_factory.CreatePointShape(), _factory.CreatePathSize(), 90, true, SweepDirection.Clockwise);

            target.Size.Width = 10;
            target.Size.Height = 20;

            var actual = target.ToSvgString();

            Assert.Equal("A10,20 90 1 1 0,0", actual);
        }
    }
}
