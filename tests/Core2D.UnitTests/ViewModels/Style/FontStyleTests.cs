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
        public void Inherits_From_ObservableObject()
        {
            var target = _factory.CreateFontStyle();
            Assert.True(target is IObservableObject);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Flags_On_Set_Notify_Events_Are_Raised()
        {
            var style = _factory.CreateFontStyle();
            var target = new PropertyChangedObserver(style);

            style.Flags =
                FontStyleFlags.Regular
                | FontStyleFlags.Bold
                | FontStyleFlags.Italic;

            Assert.Equal(FontStyleFlags.Regular
                | FontStyleFlags.Bold
                | FontStyleFlags.Italic, style.Flags);
            Assert.Equal(4, target.PropertyNames.Count);

            var propertyNames = new string[]
            {
                nameof(IFontStyle.Flags),
                nameof(IFontStyle.Regular),
                nameof(IFontStyle.Bold),
                nameof(IFontStyle.Italic)
            };

            Assert.Equal(propertyNames, target.PropertyNames);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Regular_Property()
        {
            var target = _factory.CreateFontStyle();

            target.Regular = true;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);

            target.Regular = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Bold_Property()
        {
            var target = _factory.CreateFontStyle();

            target.Bold = true;
            Assert.Equal(FontStyleFlags.Bold, target.Flags);

            target.Bold = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Italic_Property()
        {
            var target = _factory.CreateFontStyle();

            target.Italic = true;
            Assert.Equal(FontStyleFlags.Italic, target.Flags);

            target.Italic = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
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
