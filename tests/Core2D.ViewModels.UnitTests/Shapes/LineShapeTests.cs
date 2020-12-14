using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.ViewModels.UnitTests.ViewModels.Shapes
{
    public class LineShapeTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var style = _factory.CreateShapeStyle();
            var target = _factory.CreateLineShape(0, 0, 0, 0, style);
            Assert.True(target is BaseShapeViewModel);
        }
    }
}
