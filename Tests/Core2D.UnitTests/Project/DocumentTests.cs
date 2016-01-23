// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class DocumentTests
    {
        [Fact]
        public void Inherits_From_ObservableResource()
        {
            var target = new Document();
            Assert.True(target is ObservableResource);
        }

        [Fact]
        public void Pages_Not_Null()
        {
            var target = new Document();
            Assert.NotNull(target.Pages);
        }
    }
}
