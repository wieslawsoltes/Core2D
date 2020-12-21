#nullable disable
using Core2D.Model;
using Xunit;

namespace Core2D.ViewModels.UnitTests.Data
{
    public class RecordTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ViewModelBase()
        {
            var db = _factory.CreateDatabase("db");
            var target = _factory.CreateRecord(db, "<empty>");
            Assert.True(target is ViewModelBase);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Values_Not_Null()
        {
            var db = _factory.CreateDatabase("db");
            var target = _factory.CreateRecord(db, "<empty>");
            Assert.False(target.Values.IsDefault);
        }
    }
}
