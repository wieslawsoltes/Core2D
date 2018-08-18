// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;
using Xunit;

namespace Core2D.UnitTests
{
    public class DocumentContainerTests
    {
        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = new DocumentContainer();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Pages_Not_Null()
        {
            var target = new DocumentContainer();
            Assert.False(target.Pages.IsDefault);
        }
    }
}
