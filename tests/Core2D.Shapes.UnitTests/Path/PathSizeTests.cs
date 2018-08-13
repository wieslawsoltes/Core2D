// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Xunit;

namespace Core2D.UnitTests
{
    public class PathSizeTests
    {
        [Fact]
        [Trait("Core2D.Path", "Geometry")]
        public void ToString_Should_Return_Width_And_Height()
        {
            var target = new PathSize() { Width = 50, Height = 30 };
            var actual = target.ToString();
            Assert.Equal("50,30", actual);
        }
    }
}
