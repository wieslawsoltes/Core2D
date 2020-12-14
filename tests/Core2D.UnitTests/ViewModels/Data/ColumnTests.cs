using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.UnitTests.ViewModels.Data
{
    public class ColumnTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ViewModelBase()
        {
            var db = _factory.CreateDatabase("db");
            var target = _factory.CreateColumn(db, "Column");
            Assert.True(target is ViewModelBase);
        }
    }
}
