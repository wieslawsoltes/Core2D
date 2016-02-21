// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Xunit;

namespace Core2D.UnitTests
{
    public class XPropertyTests
    {
        [Fact]
        [Trait("Core2D.Data", "Data")]
        public void Inherits_From_ObservableObject()
        {
            var target = new XProperty();
            Assert.True(target is ObservableObject);
        }
        
        [Fact]
        [Trait("Core2D.Data", "Data")]
        public void ToString_Should_Return_Value_String()
        {
            var target = XProperty.Create(null, "Property1", "Value1");

            Assert.Equal("Value1", target.ToString());
        }
    }
}
