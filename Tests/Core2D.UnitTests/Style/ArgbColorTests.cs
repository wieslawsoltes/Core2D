// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using Xunit;

namespace Core2D.UnitTests
{
    public class ArgbColorTests
    {
        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ObservableObject()
        {
            var target = new ArgbColor();
            Assert.True(target is ObservableObject);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void FromUInt32_Should_Convert_From_UInit32()
        {
            var target = ArgbColor.FromUInt32(0x07ABCDEF);

            Assert.Equal(0x07, target.A);
            Assert.Equal(0xAB, target.R);
            Assert.Equal(0xCD, target.G);
            Assert.Equal(0xEF, target.B);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Parse_String_Statring_With_Hash_And_Alpha_Channel()
        {
            var target = ArgbColor.Parse("#07ABCDEF");

            Assert.Equal(0x07, target.A);
            Assert.Equal(0xAB, target.R);
            Assert.Equal(0xCD, target.G);
            Assert.Equal(0xEF, target.B);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Parse_String_Statring_With_Hash_And_No_Alpha_Channel()
        {
            var target = ArgbColor.Parse("#ABCDEF");

            Assert.Equal(0xFF, target.A);
            Assert.Equal(0xAB, target.R);
            Assert.Equal(0xCD, target.G);
            Assert.Equal(0xEF, target.B);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Parse_String_Using_Predefined_Color_Names()
        {
            var target = ArgbColor.Parse("Magenta");

            Assert.Equal(0xFF, target.A);
            Assert.Equal(0xFF, target.R);
            Assert.Equal(0x00, target.G);
            Assert.Equal(0xFF, target.B);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ToHtml_Should_Return_Color_String_Statring_With_Hash()
        {
            var target = ArgbColor.Create(0xFF, 0x7F, 0x5A, 0x45);

            Assert.Equal("#FF7F5A45", ArgbColor.ToHtml(target));
        }
    }
}
