using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class TextStyle : ViewModelBase
    {
        public static TextHAlignment[] TextHAlignmentValues { get; } = (TextHAlignment[])Enum.GetValues(typeof(TextHAlignment));

        public static TextVAlignment[] TextVAlignmentValues { get; } = (TextVAlignment[])Enum.GetValues(typeof(TextVAlignment));

        private string _fontName;
        private string _fontFile;
        private double _fontSize;
        private FontStyle _fontStyle;
        private TextHAlignment _textHAlignment;
        private TextVAlignment _textVAlignment;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string FontName
        {
            get => _fontName;
            set => RaiseAndSetIfChanged(ref _fontName, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string FontFile
        {
            get => _fontFile;
            set => RaiseAndSetIfChanged(ref _fontFile, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double FontSize
        {
            get => _fontSize;
            set => RaiseAndSetIfChanged(ref _fontSize, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => RaiseAndSetIfChanged(ref _fontStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public TextHAlignment TextHAlignment
        {
            get => _textHAlignment;
            set => RaiseAndSetIfChanged(ref _textHAlignment, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
    }
}
