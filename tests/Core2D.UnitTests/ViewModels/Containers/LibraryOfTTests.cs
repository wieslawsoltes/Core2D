using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Xunit;

namespace Core2D.UnitTests.ViewModels.Containers
{
    public class LibraryOfTTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateLibrary<PageContainerViewModel>("Test");
            Assert.True(target is ViewModelBase);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Items_Not_Null()
        {
            var target = _factory.CreateLibrary<PageContainerViewModel>("Test");
            Assert.False(target.Items.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Selected_Is_Null()
        {
            var target = _factory.CreateLibrary<PageContainerViewModel>("Test");
            Assert.Null(target.Selected);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetSelected_Sets_Selected()
        {
            var target = _factory.CreateLibrary<TemplateContainerViewModel>("Test");

            var item = _factory.CreateTemplateContainer();
            target.Items = target.Items.Add(item);

            target.SetSelected(item);

            Assert.Equal(item, target.Selected);
        }
    }
}
