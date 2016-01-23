// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class TextStyleTests
    {
        [Fact]
        public void Inherits_From_ObservableObject()
        {
            var target = new TextStyle();
            Assert.True(target is ObservableObject);
        }
    }
}
