using Core2D.Interfaces;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class PathShapeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var style = _factory.CreateShapeStyle();
            var geometry = _factory.CreatePathGeometry();
            var target = _factory.CreatePathShape(style, geometry);
            Assert.True(target is BaseShape);
        }
    }
}
