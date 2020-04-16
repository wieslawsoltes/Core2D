using Core2D;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class ContextTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateContext();
            Assert.True(target is IObservableObject);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Properties_Not_Null()
        {
            var target = _factory.CreateContext();
            Assert.False(target.Properties.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Properties_Property_Add()
        {
            var target = _factory.CreateContext();
            target.Properties = target.Properties.Add(_factory.CreateProperty(target, "Name1", "Value1"));

            Assert.Equal("Value1", target.Properties[0].Value);
        }
    }
}
