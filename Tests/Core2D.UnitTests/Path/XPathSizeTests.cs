// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class XPathSizeTests
    {
        [Fact]
        [Trait("Core2D", "Path")]
        public void ToString_Should_Return_Width_And_Height()
        {
            var target = new XPathSize() { Width = 50, Height = 30 };
            var actual = target.ToString();
            Assert.Equal("50,30", actual);
        }
    }
}
