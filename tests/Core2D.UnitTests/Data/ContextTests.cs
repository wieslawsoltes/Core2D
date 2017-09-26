// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Xunit;

namespace Core2D.UnitTests
{
    public class ContextTests
    {
        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Context();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Properties_Not_Null()
        {
            var target = new Context();
            Assert.False(target.Properties.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void This_Operator_Returns_Null()
        {
            var target = new Context();
            Assert.Null(target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void This_Operator_Returns_Property_Value()
        {
            var target = new Context();
            target.Properties = target.Properties.Add(Property.Create(target, "Name1", "Value1"));

            Assert.Equal("Value1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void This_Operator_Sets_Property_Value()
        {
            var target = new Context();
            target.Properties = target.Properties.Add(Property.Create(target, "Name1", "Value1"));

            target["Name1"] = "NewValue1";
            Assert.Equal("NewValue1", target["Name1"]);
        }

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void This_Operator_Creates_Property()
        {
            var target = new Context();
            Assert.Empty(target.Properties);

            target["Name1"] = "Value1";
            Assert.Contains("Value1", target.Properties);

            Assert.Equal(target, target.Properties[0].Owner);
        }
    }
}
