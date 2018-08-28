// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;
using Xunit;

namespace Core2D.UnitTests
{
    public class OptionsTests
    {
        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Options();
            Assert.True(target is IObservableObject);
        }
    }
}
