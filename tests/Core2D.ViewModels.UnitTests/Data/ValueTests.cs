// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Interfaces;
using Xunit;

namespace Core2D.Data.UnitTests
{
    public class ValueTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Data", "Database")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateValue();
            Assert.True(target is IObservableObject);
        }
    }
}
