// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class ContainerTests
    {
        [Fact]
        public void Inherits_From_ObservableResource()
        {
            var target = new Class1();
            Assert.True(target is ObservableResource);
        }

        [Fact]
        public void Layers_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Layers);
        }

        [Fact]
        public void SetCurrentLayer_Sets_CurrentLayer()
        {
            var target = new Class1();

            var layer = Layer.Create("Layer1", target);
            target.Layers = target.Layers.Add(layer);

            target.SetCurrentLayer(layer);

            Assert.Equal(layer, target.CurrentLayer);
        }

        [Fact]
        public void Invalidate_Should_Invalidate_All_Layers()
        {
            var target = new Class1();

            var layer1 = Layer.Create("Layer1", target);
            var layer2 = Layer.Create("Layer2", target);
            target.Layers = target.Layers.Add(layer1);
            target.Layers = target.Layers.Add(layer2);

            var workingLayer = Layer.Create("WorkingLayer", target);
            var helperLayer = Layer.Create("HelperLayer", target);
            target.WorkingLayer = workingLayer;
            target.HelperLayer = helperLayer;

            int count = 0;

            layer1.InvalidateLayer += (sender, e) => count++;
            layer2.InvalidateLayer += (sender, e) => count++;
            workingLayer.InvalidateLayer += (sender, e) => count++;
            helperLayer.InvalidateLayer += (sender, e) => count++;

            target.Invalidate();

            Assert.Equal(4, count);
        }

        [Fact]
        public void Template_Not_Null_Width_Returns_Template_Width()
        {
            var target = new Class1()
            {
                Width = 300,
                Template = new Class1()
                {
                    Width = 400
                }
            };

            Assert.Equal(400, target.Width);
        }

        [Fact]
        public void Template_Not_Null_Width_Returns_Template_Height()
        {
            var target = new Class1()
            {
                Height = 300,
                Template = new Class1()
                {
                    Height = 400
                }
            };

            Assert.Equal(400, target.Height);
        }

        [Fact]
        public void Template_Not_Null_Background_Returns_Template_Background()
        {
            var target = new Class1()
            {
                Background = ArgbColor.Create(),
                Template = new Class1()
                {
                    Background = ArgbColor.Create()
                }
            };

            Assert.Equal(target.Template.Background, target.Background);
        }

        public class Class1 : Container
        {
        }
    }
}
