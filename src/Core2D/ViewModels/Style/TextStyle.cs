using System.Collections.Generic;

namespace Core2D.Style
{
    public class TextStyle : ObservableObject
    {
        private string _fontName;
        private string _fontFile;
        private double _fontSize;
        private FontStyle _fontStyle;
        private TextHAlignment _textHAlignment;
        private TextVAlignment _textVAlignment;

        public string FontName
        {
            get => _fontName;
            set => RaiseAndSetIfChanged(ref _fontName, value);
        }

        public string FontFile
        {
            get => _fontFile;
            set => RaiseAndSetIfChanged(ref _fontFile, value);
        }

        public double FontSize
        {
            get => _fontSize;
            set => RaiseAndSetIfChanged(ref _fontSize, value);
        }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => RaiseAndSetIfChanged(ref _fontStyle, value);
        }

        public TextHAlignment TextHAlignment
        {
            get => _textHAlignment;
            set => RaiseAndSetIfChanged(ref _textHAlignment, value);
        }

        public TextVAlignment TextVAlignment
        {
            get => _textVAlignment;
            set => RaiseAndSetIfChanged(ref _textVAlignment, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new TextStyle()
            {
                Name = this.Name,
                FontName = this.FontName,
                FontFile = this.FontFile,
                FontSize = this.FontSize,
                FontStyle = (FontStyle)this.FontStyle.Copy(shared),
                TextHAlignment = this.TextHAlignment,
                TextVAlignment = this.TextVAlignment
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= FontStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            FontStyle.Invalidate();
        }

        public virtual bool ShouldSerializeFontName() => !string.IsNullOrWhiteSpace(_fontName);

        public virtual bool ShouldSerializeFontFile() => !string.IsNullOrWhiteSpace(_fontFile);

        public virtual bool ShouldSerializeFontSize() => _fontSize != default;

        public virtual bool ShouldSerializeFontStyle() => _fontStyle != null;

        public virtual bool ShouldSerializeTextHAlignment() => _textHAlignment != default;

        public virtual bool ShouldSerializeTextVAlignment() => _textVAlignment != default;
    }
}
