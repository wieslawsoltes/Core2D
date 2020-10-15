using Core2D;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class ValueTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateValue("<empty>");
            Assert.True(target is ObservableObject);
        }
    }
}
