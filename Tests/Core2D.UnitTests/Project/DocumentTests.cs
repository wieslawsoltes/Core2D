// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class DocumentTests
    {
        [Fact]
        [Trait("Core2D", "Project")]
        public void Inherits_From_Selectable()
        {
            var target = new Document();
            Assert.True(target is Selectable);
        }

        [Fact]
        [Trait("Core2D", "Project")]
        public void Pages_Not_Null()
        {
            var target = new Document();
            Assert.NotNull(target.Pages);
        }
    }
}
