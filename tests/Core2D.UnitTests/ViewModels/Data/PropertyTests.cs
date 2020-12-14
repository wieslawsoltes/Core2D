using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.UnitTests.ViewModels.Data
{
    public class PropertyTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Data", "Data")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateProperty(null, "", "");
            Assert.True(target is ViewModelBase);
        }

        [Fact]
        [Trait("Core2D.Data", "Data")]
        public void ToString_Should_Return_Value_String()
        {
            var target = _factory.CreateProperty(null, "Property1", "Value1");

            Assert.Equal("Value1", target.ToString());
        }
    }
}
