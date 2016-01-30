// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Xunit;

namespace Core2D.UnitTests
{
    public class FileSystemTests
    {
        [Fact]
        [Trait("Core2D", "IO")]
        public void Implements_IFileSystem_Interface()
        {
            var target = new FileSystem();
            Assert.True(target is IFileSystem);
        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void Open_Path()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void Create_Path()
        {

        }
        
        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void ReadBinary_Read_All_Bytes_From_Stream()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void WriteBinary_Write_All_Bytes_To_Stream()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void ReadUtf8Text_Read_String_From_Stream()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void WriteUtf8Text_Write_String_To_Stream()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void ReadUtf8Text_Read_String_From_Path()
        {

        }

        [Fact(Skip = "Need to write test.")]
        [Trait("Core2D", "IO")]
        public void WriteUtf8Text_Write_String_To_Path()
        {

        }
    }
}
