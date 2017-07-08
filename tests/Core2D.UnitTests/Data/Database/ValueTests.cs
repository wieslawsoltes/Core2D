// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Xunit;

namespace Core2D.UnitTests
{
    public class ValueTests
    {
        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var target = new Value();
            Assert.True(target is ObservableObject);
        }
    }
}
