// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class ProjectTests_IImageCache
    {
        [Fact]
        public void Implements_IImageCache_Interface()
        {
            var target = new Project();
            Assert.True(target is IImageCache);
        }

        [Fact]
        public void Inherits_From_ObservableResource()
        {
            var target = new Project();
            Assert.True(target is ObservableResource);
        }

        [Fact]
        public void Keys_Not_Null()
        {
            IImageCache target = new Project();
            Assert.NotNull(target.Keys);
        }

        [Fact(Skip = "Need to write test.")]
        public void AddImageFromFile_Add_Key_And_Notify()
        {

        }

        [Fact(Skip = "Need to write test.")]
        public void AddImageFromFile_Do_Not_Add_Duplicate()
        {

        }

        [Fact(Skip = "Need to write test.")]
        public void AddImage_Add_Key_And_Notify()
        {

        }

        [Fact(Skip = "Need to write test.")]
        public void AddImage_Do_Not_Add_Duplicate()
        {

        }

        [Fact(Skip = "Need to write test.")]
        public void GetImage_Returns_Byte_Array()
        {

        }

        [Fact(Skip = "Need to write test.")]
        public void GetImage_Returns_Null()
        {

        }

        [Fact(Skip = "Need to write test.")]
        public void RemoveImage_Remove_Key_And_Notify()
        {

        }

        [Fact(Skip = "Need to write test.")]
        public void PurgeUnusedImages_Remove_Unused_Keys_And_Notify()
        {

        }
    }
}
