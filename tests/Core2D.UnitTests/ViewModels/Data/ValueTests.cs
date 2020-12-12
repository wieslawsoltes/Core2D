using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class ValueTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateValue("<empty>");
            Assert.True(target is ViewModelBase);
        }
    }
}
