using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.ViewModels.UnitTests.ViewModels.Containers
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
