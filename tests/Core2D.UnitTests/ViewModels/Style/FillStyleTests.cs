using Core2D;
using Xunit;

namespace Core2D.Style.UnitTests
{
    public class FillStyleTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateFillStyle();
            Assert.True(target is ViewModelBase);
        }
    }
}
