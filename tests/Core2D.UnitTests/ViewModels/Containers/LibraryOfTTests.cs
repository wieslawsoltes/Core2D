using Core2D;
using Core2D.Containers;
using Xunit;

namespace Core2D.UnitTests
{
    public class LibraryOfTTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateLibrary<PageContainer>("Test");
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Items_Not_Null()
        {
            var target = _factory.CreateLibrary<PageContainer>("Test");
            Assert.False(target.Items.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Selected_Is_Null()
        {
            var target = _factory.CreateLibrary<PageContainer>("Test");
            Assert.Null(target.Selected);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetSelected_Sets_Selected()
        {
            var target = _factory.CreateLibrary<PageContainer>("Test");

            var item = _factory.CreateTemplateContainer();
            target.Items = target.Items.Add(item);

            target.SetSelected(item);

            Assert.Equal(item, target.Selected);
        }
    }
}
