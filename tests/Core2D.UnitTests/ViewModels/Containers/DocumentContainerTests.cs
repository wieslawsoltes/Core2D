using Core2D;
using Xunit;

namespace Core2D.UnitTests
{
    public class DocumentContainerTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateDocumentContainer();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Pages_Not_Null()
        {
            var target = _factory.CreateDocumentContainer();
            Assert.False(target.Pages.IsDefault);
        }
    }
}
