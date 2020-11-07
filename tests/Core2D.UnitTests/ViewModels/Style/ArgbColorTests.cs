using Core2D;
using Xunit;

namespace Core2D.Style.UnitTests
{
    public class ArgbColorTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateArgbColor();
            Assert.True(target is ViewModelBase);
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
        public void ToXamlHex_Should_Return_Color_String_Statring_With_Hash()
        {
            var target = _factory.CreateArgbColor(0xFF, 0x7F, 0x5A, 0x45);

            Assert.Equal("#FF7F5A45", ArgbColor.ToXamlHex(target));
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ToSvgHex_Should_Return_Color_String_Statring_With_Hash()
        {
            var target = _factory.CreateArgbColor(0xFF, 0x7F, 0x5A, 0x45);

            Assert.Equal("#7F5A45", ArgbColor.ToSvgHex(target)); // NOTE: 0xFF Alpha value is not used in Svg
        }
    }
}
