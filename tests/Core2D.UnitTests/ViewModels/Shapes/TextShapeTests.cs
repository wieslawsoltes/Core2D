using Core2D.Interfaces;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class TextShapeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var style = _factory.CreateShapeStyle();
            var target = _factory.CreateTextShape(0, 0, style, "Text");
            Assert.True(target is BaseShape);
        }
    }
}
