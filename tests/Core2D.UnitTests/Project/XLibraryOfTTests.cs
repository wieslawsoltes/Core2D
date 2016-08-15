// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Xunit;

namespace Core2D.UnitTests
{
    public class XLibraryOfTTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_ObservableObject()
        {
            var target = new XLibrary<XContainer>();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Items_Not_Null()
        {
            var target = new XLibrary<XContainer>();
            Assert.NotNull(target.Items);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Selected_Is_Null()
        {
            var target = new XLibrary<XContainer>();
            Assert.Null(target.Selected);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetSelected_Sets_Selected()
        {
            var target = new XLibrary<XContainer>();

            var item = XContainer.CreateTemplate();
            target.Items = target.Items.Add(item);

            target.SetSelected(item);

            Assert.Equal(item, target.Selected);
        }
    }
}
