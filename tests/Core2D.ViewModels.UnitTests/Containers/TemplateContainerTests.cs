using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Xunit;

namespace Core2D.ViewModels.UnitTests.ViewModels.Containers
{
    public class TemplateContainerTests
    {
        private readonly IFactory _factory = new Factory(null);

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_BaseContainerViewModel()
        {
            var target = _factory.CreateTemplateContainer();
            Assert.True(target is ViewModelBase);
            Assert.True(target is FrameContainerViewModel);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Layers_Not_Null()
        {
            var target = _factory.CreateTemplateContainer();
            Assert.False(target.Layers.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentLayer_Sets_CurrentLayer()
        {
            var target = _factory.CreateTemplateContainer();

            var layer = _factory.CreateLayerContainer("Layer1", target);
            target.Layers = target.Layers.Add(layer);

            target.SetCurrentLayer(layer);

            Assert.Equal(layer, target.CurrentLayer);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Invalidate_Should_Invalidate_All_Layers()
        {
            var target = _factory.CreateTemplateContainer();

            var layer1 = _factory.CreateLayerContainer("Layer1", target);
            var layer2 = _factory.CreateLayerContainer("Layer2", target);
            target.Layers = target.Layers.Add(layer1);
            target.Layers = target.Layers.Add(layer2);

            var workingLayer = _factory.CreateLayerContainer("WorkingLayer", target);
            var helperLayer = _factory.CreateLayerContainer("HelperLayer", target);
            target.WorkingLayer = workingLayer;
            target.HelperLayer = helperLayer;

            int count = 0;

            layer1.InvalidateLayerHandler += (sender, e) => count++;
            layer2.InvalidateLayerHandler += (sender, e) => count++;
            workingLayer.InvalidateLayerHandler += (sender, e) => count++;
            helperLayer.InvalidateLayerHandler += (sender, e) => count++;

            target.InvalidateLayer();

            Assert.Equal(4, count);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Template_Width_Returns_Template_Width()
        {
            var target = _factory.CreateTemplateContainer();

            target.Width = 300;

            Assert.Equal(300, target.Width);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Template_Width_Returns_Template_Height()
        {
            var target = _factory.CreateTemplateContainer();

            target.Height = 300;

            Assert.Equal(300, target.Height);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Template_Not_Null_Background_Returns_Template_Background()
        {
            var target = _factory.CreateTemplateContainer();
            var argbColor = _factory.CreateArgbColor();
            
            target.Background = argbColor;

            Assert.Equal(argbColor, target.Background);
        }
    }
}
