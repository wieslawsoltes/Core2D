// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using System;
using Xunit;

namespace Core2D.UnitTests
{
    public class ShapeStateFlagsTests
    {
        [Fact]
        [Trait("Core2D.Shape", "Shape")]
        public void Flags_Attribute_Not_Null()
        {
            Assert.NotNull(Attribute.GetCustomAttribute(typeof(ShapeStateFlags), typeof(FlagsAttribute)));
        }
    }
}
