// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Attributes;

namespace Core2D.Style
{
    /// <summary>
    /// Text style.
    /// </summary>
    public class TextStyle : ObservableObject
    {
        private string _fontName;
        private string _fontFile;
        private double _fontSize;
        private FontStyle _fontStyle;
        private TextHAlignment _textHAlignment;
        private TextVAlignment _textVAlignment;

        /// <summary>
        /// Gets or sets font name.
        /// </summary>
        public string FontName
        {
            get => _fontName;
            set => Update(ref _fontName, value);
        }

        /// <summary>
        /// Gets or sets font file path.
        /// </summary>
        public string FontFile
        {
            get => _fontFile;
            set => Update(ref _fontFile, value);
        }

        /// <summary>
        /// Gets or sets font size.
        /// </summary>
        public double FontSize
        {
            get => _fontSize;
            set => Update(ref _fontSize, value);
        }

        /// <summary>
        /// Gets or sets font style.
        /// </summary>
        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => Update(ref _fontStyle, value);
        }

        /// <summary>
        /// Gets or sets text horizontal alignment.
        /// </summary>
        public TextHAlignment TextHAlignment
        {
            get => _textHAlignment;
            set => Update(ref _textHAlignment, value);
        }

        /// <summary>
        /// Gets or sets text vertical alignment.
        /// </summary>
        public TextVAlignment TextVAlignment
        {
            get => _textVAlignment;
            set => Update(ref _textVAlignment, value);
        }

        /// <summary>
        /// Creates a new <see cref="TextStyle"/> instance.
        /// </summary>
        /// <param name="name">The text style name.</param>
        /// <param name="fontName">The font name.</param>
        /// <param name="fontFile">The font file path.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="textHAlignment">The text horizontal alignment.</param>
        /// <param name="textVAlignment">The text vertical alignment.</param>
        /// <returns>The new instance of the <see cref="TextStyle"/> class.</returns>
        public static TextStyle Create(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, FontStyle fontStyle = null, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center)
        {
            return new TextStyle()
            {
                Name = name,
                FontName = fontName,
                FontFile = fontFile,
                FontSize = fontSize,
                FontStyle = fontStyle ?? FontStyle.Create(FontStyleFlags.Regular),
                TextHAlignment = textHAlignment,
                TextVAlignment = textVAlignment
            };
        }

        /// <summary>
        /// Clones text style.
        /// </summary>
        /// <returns>The new instance of the <see cref="TextStyle"/> class.</returns>
        public TextStyle Clone()
        {
            return new TextStyle()
            {
                Name = Name,
                FontName = _fontName,
                FontFile = _fontFile,
                FontSize = _fontSize,
                FontStyle = _fontStyle.Clone(),
                TextHAlignment = _textHAlignment,
                TextVAlignment = _textVAlignment
            };
        }

        /// <summary>
        /// Check whether the <see cref="FontName"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeFontName() => !String.IsNullOrWhiteSpace(_fontName);

        /// <summary>
        /// Check whether the <see cref="FontFile"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeFontFile() => !String.IsNullOrWhiteSpace(_fontFile);

        /// <summary>
        /// Check whether the <see cref="FontSize"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeFontSize() => _fontSize != default(double);

        /// <summary>
        /// Check whether the <see cref="FontStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeFontStyle() => _fontStyle != null;

        /// <summary>
        /// Check whether the <see cref="TextHAlignment"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeTextHAlignment() => _textHAlignment != default(TextHAlignment);

        /// <summary>
        /// Check whether the <see cref="TextVAlignment"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeTextVAlignment() => _textVAlignment != default(TextVAlignment);
    }
}
