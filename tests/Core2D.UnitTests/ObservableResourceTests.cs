// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Xunit;

namespace Core2D.UnitTests
{
    public class ObservableResourceTests
    {
        [Fact]
        [Trait("Core2D", "Base")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Class1();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Resources_Not_Null()
        {
            var target = new Class1();
            Assert.NotNull(target.Resources);
        }

        [Fact]
        [Trait("Core2D", "Base")]
        public void Resources_Is_ImmutableArray()
        {
            var target = new Class1();
            Assert.IsType<ImmutableArray<ObservableObject>>(target.Resources);
        }

        private class Class1 : ObservableResource
        {
        }
    }
}
