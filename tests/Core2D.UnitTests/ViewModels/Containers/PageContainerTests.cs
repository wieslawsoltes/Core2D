using Core2D;
using Xunit;

namespace Core2D.UnitTests
{
    public class PageContainerTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreatePageContainer();
            Assert.True(target is ViewModelBase);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Layers_Not_Null()
        {
            var target = _factory.CreatePageContainer();
            Assert.False(target.Layers.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentLayer_Sets_CurrentLayer()
        {
            var target = _factory.CreatePageContainer();

            var layer = _factory.CreateLayerContainer("Layer1", target);
            target.Layers = target.Layers.Add(layer);

            target.SetCurrentLayer(layer);

            Assert.Equal(layer, target.CurrentLayer);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Invalidate_Should_Invalidate_All_Layers()
        {
            var target = _factory.CreatePageContainer();

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
        public void Template_Not_Null_Width_Returns_Template_Width()
        {
            var target = _factory.CreatePageContainer();

            target.Width = 300;
            target.Template = _factory.CreateTemplateContainer();
            target.Template.Width = 400;

            Assert.Equal(400, target.Width);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Template_Not_Null_Width_Returns_Template_Height()
        {
            var target = _factory.CreatePageContainer();

            target.Height = 300;
            target.Template = _factory.CreateTemplateContainer();
            target.Template.Height = 400;

            Assert.Equal(400, target.Height);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Template_Not_Null_Background_Returns_Template_Background()
        {
            var target = _factory.CreatePageContainer();

            target.Background = _factory.CreateArgbColor();
            target.Template = _factory.CreateTemplateContainer();
            target.Template.Background = _factory.CreateArgbColor();

            Assert.Equal(target.Template.Background, target.Background);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Invalidate_Should_Invalidate_Template()
        {
            var target = _factory.CreatePageContainer();

            target.Template = _factory.CreateTemplateContainer();

            var layer = _factory.CreateLayerContainer("Layer1", target);
            target.Template.Layers = target.Template.Layers.Add(layer);

            bool raised = false;

            layer.InvalidateLayerHandler += (sender, e) =>
            {
                raised = true;
            };

            target.InvalidateLayer();

            Assert.True(raised);
        }
    }
}
