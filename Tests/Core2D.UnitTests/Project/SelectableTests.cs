// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class SelectableTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_ObservableResource()
        {
            var target = new Class1();
            Assert.True(target is ObservableResource);
        }
        
        private class Class1 : Selectable
        {
        }
    }
}
