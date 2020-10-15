using Core2D;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class RecordTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var db = _factory.CreateDatabase("db");
            var target = _factory.CreateRecord(db, "<empty>");
            Assert.True(target is ObservableObject);
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
