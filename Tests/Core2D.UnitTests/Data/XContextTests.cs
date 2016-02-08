// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Xunit;

namespace Core2D.UnitTests
{
    public class XContextTests
    {
        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var target = new XContext();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Properties_Not_Null()
        {
            var target = new XContext();
            Assert.NotNull(target.Properties);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void this_Operator_Returns_Null()
        {
            var target = new XContext();
            Assert.Equal(null, target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void this_Operator_Returns_Property_Value()
        {
            var target = new XContext();
            target.Properties = target.Properties.Add(XProperty.Create(target, "Name1", "Value1"));

            Assert.Equal("Value1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void this_Operator_Sets_Property_Value()
        {
            var target = new XContext();
            target.Properties = target.Properties.Add(XProperty.Create(target, "Name1", "Value1"));

            target["Name1"] = "NewValue1";
            Assert.Equal("NewValue1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void this_Operator_Creates_Property()
        {
            var target = new XContext();
            Assert.Equal(0, target.Properties.Length);

            target["Name1"] = "Value1";
            Assert.Equal(1, target.Properties.Length);

            Assert.Equal(target, target.Properties[0].Owner);
        }
    }
}
