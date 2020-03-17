// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Xunit;

namespace Core2D.UnitTests
{
    public class DocumentContainerTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateDocumentContainer();
            Assert.True(target is IObservableObject);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Pages_Not_Null()
        {
            var target = _factory.CreateDocumentContainer();
            Assert.False(target.Pages.IsDefault);
        }
    }
}
