// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Common;
using Core2D.Interfaces;
using Core2D.Style;
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
            var target = new FontStyle();
            Assert.True(target is IObservableObject);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Flags_On_Set_Notify_Events_Are_Raised()
        {
            var style = new FontStyle();
            var target = new PropertyChangedObserver(style);

            style.Flags = 
                FontStyleFlags.Regular 
                | FontStyleFlags.Bold 
                | FontStyleFlags.Italic
                | FontStyleFlags.Underline
                | FontStyleFlags.Strikeout;

            Assert.Equal(FontStyleFlags.Regular
                | FontStyleFlags.Bold
                | FontStyleFlags.Italic
                | FontStyleFlags.Underline
                | FontStyleFlags.Strikeout, style.Flags);
            Assert.Equal(6, target.PropertyNames.Count);

            var propertyNames = new string[]
            {
                nameof(IFontStyle.Flags),
                nameof(IFontStyle.Regular),
                nameof(IFontStyle.Bold),
                nameof(IFontStyle.Italic),
                nameof(IFontStyle.Underline),
                nameof(IFontStyle.Strikeout)
            };

            Assert.Equal(propertyNames, target.PropertyNames);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Regular_Property()
        {
            var target = new FontStyle();

            target.Regular = true;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);

            target.Regular = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Bold_Property()
        {
            var target = new FontStyle();

            target.Bold = true;
            Assert.Equal(FontStyleFlags.Bold, target.Flags);

            target.Bold = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Italic_Property()
        {
            var target = new FontStyle();

            target.Italic = true;
            Assert.Equal(FontStyleFlags.Italic, target.Flags);

            target.Italic = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Underline_Property()
        {
            var target = new FontStyle();

            target.Underline = true;
            Assert.Equal(FontStyleFlags.Underline, target.Flags);

            target.Underline = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Strikeout_Property()
        {
            var target = new FontStyle();

            target.Strikeout = true;
            Assert.Equal(FontStyleFlags.Strikeout, target.Flags);

            target.Strikeout = false;
            Assert.Equal(FontStyleFlags.Regular, target.Flags);
        }
        
        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void Parse_FontStyleFlags_String()
        {
            var target = FontStyle.Parse("Bold, Italic, Strikeout");

            Assert.Equal(
                FontStyleFlags.Bold 
                | FontStyleFlags.Italic 
                | FontStyleFlags.Strikeout, target.Flags);
        }

        [Fact]
        [Trait("Core2D.Style", "Style")]
        public void ToString_Should_Return_Flags_String()
        {
            var target = _factory.CreateFontStyle(
                FontStyleFlags.Bold
                | FontStyleFlags.Italic
                | FontStyleFlags.Strikeout);

            var actual = target.ToString();

            Assert.Equal("Bold, Italic, Strikeout", actual);
        }
    }
}
