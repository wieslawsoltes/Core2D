using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.UnitTests.ViewModels.Path
{
    public class PathSizeTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToXamlString_Should_Return_Width_And_Height()
        {
            var target = _factory.CreatePathSize();

            target.Width = 50;
            target.Height = 30;

            var actual = target.ToXamlString();
            Assert.Equal("50,30", actual);
        }

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToSvgString_Should_Return_Width_And_Height()
        {
            var target = _factory.CreatePathSize();

            target.Width = 50;
            target.Height = 30;

            var actual = target.ToSvgString();
            Assert.Equal("50,30", actual);
        }
    }
}
