// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Project;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPageTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_Container()
        {
            var target = new XPage();
            Assert.True(target is XContainer);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Data_Not_Null()
        {
            var target = new XPage();
            Assert.NotNull(target.Data);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Returns_Null()
        {
            var target = new XPage();
            Assert.Equal(null, target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Returns_Property_Value()
        {
            var target = new XPage();
            target.Data.Properties = target.Data.Properties.Add(XProperty.Create(target.Data, "Name1", "Value1"));

            Assert.Equal("Value1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Sets_Property_Value()
        {
            var target = new XPage();
            target.Data.Properties = target.Data.Properties.Add(XProperty.Create(target.Data, "Name1", "Value1"));

            target["Name1"] = "NewValue1";
            Assert.Equal("NewValue1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void this_Operator_Creates_Property()
        {
            var target = new XPage();
            Assert.Equal(0, target.Data.Properties.Length);

            target["Name1"] = "Value1";
            Assert.Equal(1, target.Data.Properties.Length);

            Assert.Equal(target.Data, target.Data.Properties[0].Owner);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Invalidate_Should_Invalidate_Template()
        {
            var target = new XPage()
            {
                Template = new XTemplate()
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
    }
}
