using Core2D;
using Xunit;

namespace Core2D.Style.UnitTests
{
    public class ShapeStyleTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateShapeStyle();
            Assert.True(target is IObservableObject);
        }
    }
}
