// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class XImageTests
    {
        [Fact]
        [Trait("Core2D", "Shapes")]
        public void Inherits_From_XText()
        {
            var target = new XImage();
            Assert.True(target is XText);
        }
    }
}
