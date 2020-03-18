using Core2D.Interfaces;
using Xunit;

namespace Core2D.UnitTests
{
    public class OptionsTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateOptions();
            Assert.True(target is IObservableObject);
        }
    }
}
