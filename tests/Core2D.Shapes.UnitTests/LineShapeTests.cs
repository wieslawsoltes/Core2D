// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class LineShapeTests
    {
        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var target = new LineShape();
            Assert.True(target is BaseShape);
        }
    }
}
