using System.Collections.Immutable;
using Core2D;
using Core2D.Path;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathGeometryTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void Figures_Not_Null()
        {
            var target = _factory.CreatePathGeometry();
            Assert.False(target.Figures.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void FillRule_Set_To_Nonzero_By_Default()
        {
            var target = _factory.CreatePathGeometry();
            Assert.Equal(FillRule.Nonzero, target.FillRule);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToXamlString_Should_Return_Empty()
        {
            var geometry = _factory.CreatePathGeometry();

            var target = ImmutableArray.Create<PathFigure>();
            var actual = (geometry as PathGeometry).ToXamlString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToSvgString_Should_Return_Empty()
        {
            var geometry = _factory.CreatePathGeometry();

            var target = ImmutableArray.Create<PathFigure>();
            var actual = (geometry as PathGeometry).ToSvgString(target);

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToXamlString_Should_Return_Path_Markup_Empty_Nonzero()
        {
            var target = _factory.CreatePathGeometry();

            var actual = target.ToXamlString();

            Assert.Equal("F1", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToSvgString_Should_Return_Path_Markup_Empty_Nonzero()
        {
            var target = _factory.CreatePathGeometry();

            var actual = target.ToSvgString();

            Assert.Equal("", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToXamlString_Should_Return_Path_Markup_Empty_EvenOdd()
        {
            var target = _factory.CreatePathGeometry();

            target.FillRule = FillRule.EvenOdd;

            var actual = target.ToXamlString();

            Assert.Equal("", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToSvgString_Should_Return_Path_Markup_Empty_EvenOdd()
        {
            var target = _factory.CreatePathGeometry();

            target.FillRule = FillRule.EvenOdd;

            var actual = target.ToSvgString();

            Assert.Equal("", actual);
        }
    }
}
