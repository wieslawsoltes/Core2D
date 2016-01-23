// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class PageTests
    {
        [Fact]
        public void Inherits_From_Container()
        {
            var target = new Page();
            Assert.True(target is Container);
        }

        [Fact]
        public void Data_NotNull()
        {
            var target = new Page();
            Assert.NotNull(target.Data);
        }

        [Fact]
        public void this_Operator_Returns_Null()
        {
            var target = new Page();
            Assert.Equal(null, target["Name1"]);
        }

        [Fact]
        public void this_Operator_Returns_Property_Value()
        {
            var target = new Page();
            target.Data.Properties = target.Data.Properties.Add(Property.Create(target.Data, "Name1", "Value1"));

            Assert.Equal("Value1", target["Name1"]);
        }

        [Fact]
        public void this_Operator_Sets_Property_Value()
        {
            var target = new Page();
            target.Data.Properties = target.Data.Properties.Add(Property.Create(target.Data, "Name1", "Value1"));

            target["Name1"] = "NewValue1";
            Assert.Equal("NewValue1", target["Name1"]);
        }

        [Fact]
        public void this_Operator_Creates_Property()
        {
            var target = new Page();
            Assert.Equal(0, target.Data.Properties.Length);

            target["Name1"] = "Value1";
            Assert.Equal(1, target.Data.Properties.Length);

            Assert.Equal(target.Data, target.Data.Properties[0].Owner);
        }

        [Fact]
        public void Invalidate_Should_Invalidate_Template()
        {
            var target = new Page()
            {
                Template = new Template()
            };

            var layer = Layer.Create("Layer1", target);
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
