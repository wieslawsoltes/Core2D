// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using Core2D.Interfaces;
using Xunit;

namespace Core2D.Style.UnitTests
{
    public class ShapeStyleTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateShapeStyle();
            Assert.True(target is IObservableObject);
        }
    }
}
