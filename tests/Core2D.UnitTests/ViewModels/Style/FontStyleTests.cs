using Core2D;
using Core2D.Common.UnitTests;
using Xunit;

namespace Core2D.Style.UnitTests
{
    public class FontStyleTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Inherits_From_ViewModelBase()
        {
            var target = _factory.CreateFontStyle();
            Assert.True(target is ViewModelBase);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ToString_Should_Return_Flags_String()
        {
            var target = _factory.CreateFontStyle(
                FontStyleFlags.Bold
                | FontStyleFlags.Italic);

            var actual = target.ToString();

            Assert.Equal("Bold, Italic", actual);
        }
    }
}
