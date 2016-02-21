// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Xunit;

namespace Core2D.UnitTests
{
    public class XTemplateTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_Container()
        {
            var target = new XTemplate();
            Assert.True(target is XContainer);
        }
    }
}
