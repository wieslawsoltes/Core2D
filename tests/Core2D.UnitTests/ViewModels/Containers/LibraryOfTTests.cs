// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;
using Core2D.Interfaces;
using Xunit;

namespace Core2D.UnitTests
{
    public class LibraryOfTTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateLibrary<IPageContainer>("Test");
            Assert.True(target is IObservableObject);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Items_Not_Null()
        {
            var target = _factory.CreateLibrary<IPageContainer>("Test");
            Assert.False(target.Items.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Selected_Is_Null()
        {
            var target = _factory.CreateLibrary<IPageContainer>("Test");
            Assert.Null(target.Selected);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetSelected_Sets_Selected()
        {
            var target = _factory.CreateLibrary<IPageContainer>("Test");

            var item = _factory.CreateTemplateContainer();
            target.Items = target.Items.Add(item);

            target.SetSelected(item);

            Assert.Equal(item, target.Selected);
        }
    }
}
