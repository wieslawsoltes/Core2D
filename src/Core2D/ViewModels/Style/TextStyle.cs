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
            return new TextStyle()
            {
                Name = this.Name,
                FontName = this.FontName,
                FontFile = this.FontFile,
                FontSize = this.FontSize,
                FontStyle = (IFontStyle)this.FontStyle.Copy(shared),
                TextHAlignment = this.TextHAlignment,
                TextVAlignment = this.TextVAlignment
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
