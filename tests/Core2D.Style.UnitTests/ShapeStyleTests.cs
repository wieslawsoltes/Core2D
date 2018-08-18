// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using Xunit;

namespace Core2D.Style.UnitTests
{
    public class ShapeStyleTests
    {
        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ObservableObject()
        {
            var target = new ShapeStyle();
            Assert.True(target is ObservableObject);
        }
    }
}
