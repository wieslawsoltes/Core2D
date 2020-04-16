using Core2D;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class PointShapeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var target = _factory.CreatePointShape();
            Assert.True(target is BaseShape);
        }
    }
}
