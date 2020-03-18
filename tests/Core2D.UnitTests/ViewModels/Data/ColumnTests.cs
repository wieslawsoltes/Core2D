using Core2D.Interfaces;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class ColumnTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var db = _factory.CreateDatabase("db");
            var target = _factory.CreateColumn(db, "Column");
            Assert.True(target is IObservableObject);
        }
    }
}
