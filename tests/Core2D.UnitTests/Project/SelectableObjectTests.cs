// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Xunit;

namespace Core2D.UnitTests
{
    public class SelectableObjectTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Class1();
            Assert.True(target is ObservableObject);
        }
        
        private class Class1 : SelectableObject
        {
        }
    }
}
