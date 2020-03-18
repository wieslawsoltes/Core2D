using System;
using System.IO;
using System.Linq;
using System.Text;
using Core2D.Common;
using Core2D.Interfaces;
using Xunit;

namespace Core2D.FileSystem.DotNet.UnitTests
{
    public class DotNetFileSystemTests
    {
        private IServiceProvider _serviceProvider = new TestServiceProvider();

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void Implements_IFileSystem_Interface()
        {
            var target = new DotNetFileSystem(_serviceProvider);
            Assert.True(target is IFileSystem);
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void Open_Path_Throws_FileNotFoundException()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);
            Assert.Throws<FileNotFoundException>(
                () =>
                {
                    using (var stream = target.Open("existing1.txt"))
                    { }
                });
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void Create_Path_Creates_New_File()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);

            using (var stream = target.Create("new1.txt"))
            { }
            Assert.True(File.Exists("new1.txt"));

            using (var stream = target.Open("new1.txt"))
            { }
            File.Delete("new1.txt");
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void ReadBinary_Read_All_Bytes_From_Stream()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);
            var expceted = new byte[] { 0x12, 0x34, 0x56, 0x67 };
            byte[] actual;

            using (var stream = new MemoryStream(expceted))
            {
                actual = target.ReadBinary(stream);
            }

            Assert.Equal(expceted, actual);
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void WriteBinary_Write_All_Bytes_To_Stream()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);
            var expceted = new byte[] { 0x12, 0x34, 0x56, 0x67 };
            byte[] actual;

            using (var stream = new MemoryStream())
            {
                target.WriteBinary(stream, expceted);
                actual = stream.ToArray();
            }

            Assert.Equal(expceted, actual);
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void ReadUtf8Text_Read_String_From_Stream()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);
            var expceted = "κόσμε";
            string actual;

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(expceted)))
            {
                actual = target.ReadUtf8Text(stream);
            }

            Assert.Equal(expceted, actual);
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void WriteUtf8Text_Write_String_To_Stream()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);
            var expceted = "κόσμε";

            byte[] actual;
            using (var stream = new MemoryStream())
            {
                target.WriteUtf8Text(stream, expceted);
                actual = stream.ToArray();
            }

            var excpetedBytes = Encoding.UTF8.GetBytes(expceted);
            var actualBytesWithoutPreamble = actual.Skip(3).ToArray();
            Assert.Equal(excpetedBytes, actualBytesWithoutPreamble);
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void ReadUtf8Text_Read_String_From_Path()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);
            Assert.Throws<FileNotFoundException>(
                () =>
                {
                    target.ReadUtf8Text("existing2.txt");
                });
        }

        [Fact]
        [Trait("FileSystem.DotNet", "Util")]
        public void WriteUtf8Text_Write_String_To_Path()
        {
            IFileSystem target = new DotNetFileSystem(_serviceProvider);
            var expceted = "κόσμε";

            target.WriteUtf8Text("new2.txt", expceted);
            Assert.True(File.Exists("new2.txt"));

            var actual = target.ReadUtf8Text("new2.txt");
            Assert.Equal(expceted, actual);

            File.Delete("new2.txt");
        }
    }
}
