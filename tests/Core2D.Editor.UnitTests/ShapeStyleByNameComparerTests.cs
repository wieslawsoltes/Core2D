// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using System.Collections.Generic;
using Xunit;

namespace Core2D.Editor.UnitTests
{
    public class ShapeStyleByNameComparerTests
    {
        [Fact]
        [Trait("Core2D.Editor", "Style")]
        public void Implements_IEqualityComparer_Interface()
        {
            var target = new ShapeStyleByNameComparer();
            Assert.True(target is IEqualityComparer<IShapeStyle>);
        }

        [Fact]
        [Trait("Core2D.Editor", "Style")]
        public void Equals_Same_Object_Return_True()
        {
            var x = ShapeStyle.Create();
            Assert.Equal(x, x, new ShapeStyleByNameComparer());
        }

        [Fact]
        [Trait("Core2D.Editor", "Style")]
        public void Equals_First_Object_Null_Return_False()
        {
            var x = ShapeStyle.Create();
            var c = new ShapeStyleByNameComparer();
            Assert.False(c.Equals(null, x));
        }

        [Fact]
        [Trait("Core2D.Editor", "Style")]
        public void Equals_Second_Object_Null_Return_False()
        {
            var y = ShapeStyle.Create();
            var c = new ShapeStyleByNameComparer();
            Assert.False(c.Equals(y, null));
        }

        [Fact]
        [Trait("Core2D.Editor", "Style")]
        public void Equals_Same_Name_Return_True()
        {
            var x = ShapeStyle.Create("Style1");
            var y = ShapeStyle.Create("Style1");
            Assert.Equal(x, y, new ShapeStyleByNameComparer());
        }

        [Fact]
        [Trait("Core2D.Editor", "Style")]
        public void Equals_Different_Name_Return_False()
        {
            var x = ShapeStyle.Create("Style1");
            var y = ShapeStyle.Create("Style2");
            Assert.NotEqual(x, y, new ShapeStyleByNameComparer());
        }
    }
}
