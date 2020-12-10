using Core2D.Model;
using Core2D.ViewModels;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class RecordTests
    {
        private readonly IFactory _factory = new Factory();

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
