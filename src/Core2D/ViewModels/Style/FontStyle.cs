using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class FontStyle : ViewModelBase
    {
        // TODO:
        [AutoNotify] private FontStyleFlags _flags;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new FontStyle()
            {
                Flags = this.Flags
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }

        public static FontStyle Parse(string s)
        {
            var flags = (FontStyleFlags)Enum.Parse(typeof(FontStyleFlags), s, true);
            return new FontStyle()
            {
                Flags = flags
            };
        }

        public override string ToString()
        {
            return _flags.ToString();
        }
    }
}
