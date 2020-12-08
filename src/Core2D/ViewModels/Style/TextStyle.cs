using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class TextStyle : ViewModelBase
    {
        public static TextHAlignment[] TextHAlignmentValues { get; } = (TextHAlignment[])Enum.GetValues(typeof(TextHAlignment));

        public static TextVAlignment[] TextVAlignmentValues { get; } = (TextVAlignment[])Enum.GetValues(typeof(TextVAlignment));

        [AutoNotify] private string _fontName;
        [AutoNotify] private string _fontFile;
        [AutoNotify] private double _fontSize;
        [AutoNotify] private FontStyle _fontStyle;
        [AutoNotify] private TextHAlignment _textHAlignment;
        [AutoNotify] private TextVAlignment _textVAlignment;

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
