using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.Style.UnitTests
{
    public class TextStyleTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateTextStyle();
            Assert.True(target is ViewModelBase);
        }
    }
}
