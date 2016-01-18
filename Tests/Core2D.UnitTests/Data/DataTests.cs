// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class DataTests
    {
        [Fact]
        public void Inherits_From_ObservableObject()
        {
            var target = new Data();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        public void Properties_Not_Null()
        {
            var target = new Data();
            Assert.NotNull(target.Properties);
        }

        [Fact]
        public void this_Operator_Returns_Null()
        {
            var target = new Data();
            Assert.Equal(null, target["Name1"]);
        }

        [Fact]
        public void this_Operator_Returns_Property_Value()
        {
            var target = new Data();
            target.Properties = target.Properties.Add(Property.Create(target, "Name1", "Value1"));

            Assert.Equal("Value1", target["Name1"]);
        }

        [Fact]
        public void this_Operator_Sets_Property_Value()
        {
            var target = new Data();
            target.Properties = target.Properties.Add(Property.Create(target, "Name1", "Value1"));

            target["Name1"] = "NewValue1";
            Assert.Equal("NewValue1", target["Name1"]);
        }

        [Fact]
        public void this_Operator_Creates_Property()
        {
            var target = new Data();
            Assert.Equal(0, target.Properties.Length);

            target["Name1"] = "Value1";
            Assert.Equal(1, target.Properties.Length);

            Assert.Equal(target, target.Properties[0].Owner);
        }
    }
}
