using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UnitTests.ViewModels.Shapes
{
    public class PathShapeTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var style = _factory.CreateShapeStyle();
            var geometry = _factory.CreatePathGeometry();
            var target = _factory.CreatePathShape(style, geometry);
            Assert.True(target is BaseShapeViewModel);
        }
    }
}
