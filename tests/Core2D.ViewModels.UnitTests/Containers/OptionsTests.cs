#nullable disable
using Core2D.Model;
using Xunit;

namespace Core2D.ViewModels.UnitTests.Containers
{
    public class OptionsTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateOptions();
            Assert.True(target is ViewModelBase);
        }
    }
}
