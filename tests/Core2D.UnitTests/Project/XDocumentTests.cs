// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Xunit;

namespace Core2D.UnitTests
{
    public class XDocumentTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_Selectable()
        {
            var target = new XDocument();
            Assert.True(target is XSelectable);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Pages_Not_Null()
        {
            var target = new XDocument();
            Assert.NotNull(target.Pages);
        }
    }
}
