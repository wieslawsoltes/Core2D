// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Project;
using Core2D.Style;
using Xunit;

namespace Core2D.UnitTests
{
    public class XContainerTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_Selectable()
        {
            var target = new Class1();
            Assert.True(target is XSelectable);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Layers_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Layers);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetCurrentLayer_Sets_CurrentLayer()
        {
            var target = new Class1();

            var layer = XLayer.Create("Layer1", target);
            target.Layers = target.Layers.Add(layer);

            target.SetCurrentLayer(layer);

            Assert.Equal(layer, target.CurrentLayer);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Invalidate_Should_Invalidate_All_Layers()
        {
            var target = new Class1();

            var layer1 = XLayer.Create("Layer1", target);
            var layer2 = XLayer.Create("Layer2", target);
            target.Layers = target.Layers.Add(layer1);
            target.Layers = target.Layers.Add(layer2);

            var workingLayer = XLayer.Create("WorkingLayer", target);
            var helperLayer = XLayer.Create("HelperLayer", target);
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
        [Trait("Core2D.Project", "Project")]
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
        [Trait("Core2D.Project", "Project")]
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
        [Trait("Core2D.Project", "Project")]
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

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Data_Not_Null()
        {
            var target = new XContainer();
            Assert.NotNull(target.Data);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Returns_Null()
        {
            var target = new XContainer();
            Assert.Equal(null, target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Returns_Property_Value()
        {
            var target = new XContainer();
            target.Data.Properties = target.Data.Properties.Add(XProperty.Create(target.Data, "Name1", "Value1"));

            Assert.Equal("Value1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Sets_Property_Value()
        {
            var target = new XContainer();
            target.Data.Properties = target.Data.Properties.Add(XProperty.Create(target.Data, "Name1", "Value1"));

            target["Name1"] = "NewValue1";
            Assert.Equal("NewValue1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Creates_Property()
        {
            var target = new XContainer();
            Assert.Equal(0, target.Data.Properties.Length);

            target["Name1"] = "Value1";
            Assert.Equal(1, target.Data.Properties.Length);

            Assert.Equal(target.Data, target.Data.Properties[0].Owner);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Invalidate_Should_Invalidate_Template()
        {
            var target = new XContainer()
            {
                Template = new XContainer()
            };

            var layer = XLayer.Create("Layer1", target);
            target.Template.Layers = target.Template.Layers.Add(layer);

            bool raised = false;

            layer.InvalidateLayer += (sender, e) =>
            {
                raised = true;
            };

            target.Invalidate();

            Assert.True(raised);
        }

        public class Class1 : XContainer
        {
        }
    }
}
