using Core2D.Interfaces;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathSizeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Width_And_Height()
        {
            var target = _factory.CreatePathSize();

            target.Width = 50;
            target.Height = 30;

            var actual = target.ToString();
            Assert.Equal("50,30", actual);
        }
    }
}
