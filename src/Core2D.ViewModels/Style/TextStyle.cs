// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Text style.
    /// </summary>
    public class TextStyle : ObservableObject, ITextStyle
    {
        private string _fontName;
        private string _fontFile;
        private double _fontSize;
        private IFontStyle _fontStyle;
        private TextHAlignment _textHAlignment;
        private TextVAlignment _textVAlignment;

        /// <inheritdoc/>
        public string FontName
        {
            get => _fontName;
            set => Update(ref _fontName, value);
        }

        /// <inheritdoc/>
        public string FontFile
        {
            get => _fontFile;
            set => Update(ref _fontFile, value);
        }

        /// <inheritdoc/>
        public double FontSize
        {
            get => _fontSize;
            set => Update(ref _fontSize, value);
        }

        /// <inheritdoc/>
        public IFontStyle FontStyle
        {
            get => _fontStyle;
            set => Update(ref _fontStyle, value);
        }

        /// <inheritdoc/>
        public TextHAlignment TextHAlignment
        {
            get => _textHAlignment;
            set => Update(ref _textHAlignment, value);
        }

        /// <inheritdoc/>
        public TextVAlignment TextVAlignment
        {
            get => _textVAlignment;
            set => Update(ref _textVAlignment, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
        public static ITextStyle Create(string name = "", string fontName = "Calibri", string fontFile = @"C:\Windows\Fonts\calibri.ttf", double fontSize = 12.0, IFontStyle fontStyle = null, TextHAlignment textHAlignment = TextHAlignment.Center, TextVAlignment textVAlignment = TextVAlignment.Center)
        {
            return new TextStyle()
            {
                Name = name,
                FontName = fontName,
                FontFile = fontFile,
                FontSize = fontSize,
                FontStyle = fontStyle ?? Style.FontStyle.Create(FontStyleFlags.Regular),
                TextHAlignment = textHAlignment,
                TextVAlignment = textVAlignment
            };
        }

        /// <summary>
        /// Check whether the <see cref="FontName"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFontName() => !string.IsNullOrWhiteSpace(_fontName);

        /// <summary>
        /// Check whether the <see cref="FontFile"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFontFile() => !string.IsNullOrWhiteSpace(_fontFile);

        /// <summary>
        /// Check whether the <see cref="FontSize"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFontSize() => _fontSize != default;

        /// <summary>
        /// Check whether the <see cref="FontStyle"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFontStyle() => _fontStyle != null;

        /// <summary>
        /// Check whether the <see cref="TextHAlignment"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTextHAlignment() => _textHAlignment != default;

        /// <summary>
        /// Check whether the <see cref="TextVAlignment"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTextVAlignment() => _textVAlignment != default;
    }
}
