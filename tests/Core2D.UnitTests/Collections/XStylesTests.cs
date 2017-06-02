// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Collections;
using Xunit;

namespace Core2D.UnitTests
{
    public class XStylesTests
    {
        [Fact]
        [Trait("Core2D", "Collections")]
        public void Inherits_From_ObservableObject()
        {
            var target = new XStyles();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D", "Collections")]
        public void Children_Not_Null()
        {
            var target = new XStyles();
            Assert.NotNull(target.Children);
        }
    }
}
