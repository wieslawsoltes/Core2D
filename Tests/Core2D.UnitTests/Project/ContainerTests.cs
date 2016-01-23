// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class ContainerTests
    {
        [Fact]
        public void Inherits_From_ObservableResource()
        {
            var target = new Class1();
            Assert.True(target is ObservableResource);
        }

        [Fact]
        public void Layers_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Layers);
        }

        public class Class1 : Container
        {
        }
    }
}
