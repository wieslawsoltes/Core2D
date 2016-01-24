// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Linq;
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
            var target = new Project();
            Assert.NotNull(target.Keys);
        }

        [Fact]
        public void AddImageFromFile_Add_Key_And_Notify()
        {
            var target = new Project();
            string actual = null;

            target.PropertyChanged += (sender, e) =>
            {
                actual = e.PropertyName;
            };

            var key = target.AddImageFromFile(@"C:\Images\image.jpg", new byte[] { });

            Assert.Equal(Project.ImageEntryNamePrefix + "image.jpg", key);
            Assert.Equal(1, target.Keys.Count());
            Assert.Equal("Keys", actual);
        }

        [Fact]
        public void AddImageFromFile_Do_Not_Add_Duplicate()
        {
            var target = new Project();
            int count = 0;

            target.PropertyChanged += (sender, e) =>
            {
                count++;
            };

            var key1 = target.AddImageFromFile(@"C:\Images\image.jpg", new byte[] { });
            var key2 = target.AddImageFromFile(@"C:\Images\image.jpg", new byte[] { });

            Assert.Equal(Project.ImageEntryNamePrefix + "image.jpg", key1);
            Assert.Equal(Project.ImageEntryNamePrefix + "image.jpg", key2);
            Assert.Equal(1, target.Keys.Count());
            Assert.Equal(1, count);
        }

        [Fact]
        public void AddImage_Add_Key_And_Notify()
        {
            var target = new Project();
            string actual = null;

            target.PropertyChanged += (sender, e) =>
            {
                actual = e.PropertyName;
            };

            var key = Project.ImageEntryNamePrefix + "image.jpg";

            target.AddImage(key, new byte[] { });

            Assert.Equal(key, target.Keys.First().Key);
            Assert.Equal(1, target.Keys.Count());
            Assert.Equal("Keys", actual);
        }

        [Fact]
        public void AddImage_Do_Not_Add_Duplicate()
        {
            var target = new Project();
            int count = 0;

            target.PropertyChanged += (sender, e) =>
            {
                count++;
            };

            var key = Project.ImageEntryNamePrefix + "image.jpg";

            target.AddImage(key, new byte[] { });
            target.AddImage(key, new byte[] { });

            Assert.Equal(key, target.Keys.First().Key);
            Assert.Equal(1, target.Keys.Count());
            Assert.Equal(1, count);
        }

        [Fact]
        public void GetImage_Returns_Byte_Array()
        {
            var project = new Project();

            var key = Project.ImageEntryNamePrefix + "image.jpg";
            var data = new byte[] { 0x12, 0x34, 0x56, 0x78 };

            project.AddImage(key, data);

            var target = project.GetImage(key);

            Assert.Equal(data, target);
        }

        [Fact]
        public void GetImage_Returns_Null()
        {
            var project = new Project();

            var key = Project.ImageEntryNamePrefix + "image.jpg";

            var target = project.GetImage(key);

            Assert.Null(target);
        }

        [Fact]
        public void RemoveImage_Remove_Key_And_Notify()
        {
            var target = new Project();
            int count = 0;

            target.PropertyChanged += (sender, e) =>
            {
                count++;
            };

            var key = Project.ImageEntryNamePrefix + "image.jpg";

            target.AddImage(key, new byte[] { });
            target.RemoveImage(key);

            Assert.Equal(0, target.Keys.Count());
            Assert.Equal(2, count);
        }

        [Fact]
        public void PurgeUnusedImages_Remove_All_Keys_And_Notify()
        {
            var target = new Project();
            int count = 0;

            target.PropertyChanged += (sender, e) =>
            {
                count++;
            };

            var key1 = Project.ImageEntryNamePrefix + "image1.jpg";
            var key2 = Project.ImageEntryNamePrefix + "image2.jpg";
            var key3 = Project.ImageEntryNamePrefix + "image3.jpg";

            target.AddImage(key1, new byte[] { });
            target.AddImage(key2, new byte[] { });
            target.AddImage(key3, new byte[] { });

            var used = Enumerable.Empty<string>().ToImmutableHashSet();

            target.PurgeUnusedImages(used);

            Assert.Equal(0, target.Keys.Count());
            Assert.Equal(4, count);
        }

        [Fact]
        public void PurgeUnusedImages_Remove_Only_Unused_Keys_And_Notify()
        {
            var target = new Project();
            int count = 0;

            target.PropertyChanged += (sender, e) =>
            {
                count++;
            };

            var key1 = Project.ImageEntryNamePrefix + "image1.jpg";
            var key2 = Project.ImageEntryNamePrefix + "image2.jpg";
            var key3 = Project.ImageEntryNamePrefix + "image3.jpg";

            target.AddImage(key1, new byte[] { });
            target.AddImage(key2, new byte[] { });
            target.AddImage(key3, new byte[] { });

            var used = ImmutableHashSet.Create(key2);

            target.PurgeUnusedImages(used);

            Assert.Equal(key2, target.Keys.First().Key);
            Assert.Equal(1, target.Keys.Count());
            Assert.Equal(4, count);
        }
    }
}
