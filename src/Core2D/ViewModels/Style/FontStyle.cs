using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    public class FontStyle : ObservableObject
    {
        private FontStyleFlags _flags;

        public FontStyleFlags Flags
        {
            get => _flags;
            set
            {
                RaiseAndSetIfChanged(ref _flags, value);
                NatifyAll();
            }
        }

        private void NatifyAll()
        {
            RaisePropertyChanged(nameof(Regular));
            RaisePropertyChanged(nameof(Bold));
            RaisePropertyChanged(nameof(Italic));
        }

        public bool Regular
        {
            get => _flags == FontStyleFlags.Regular;
            set => Flags = value ? _flags | FontStyleFlags.Regular : _flags & ~FontStyleFlags.Regular;
        }

        public bool Bold
        {
            get => _flags.HasFlag(FontStyleFlags.Bold);
            set => Flags = value ? _flags | FontStyleFlags.Bold : _flags & ~FontStyleFlags.Bold;
        }

        public bool Italic
        {
            get => _flags.HasFlag(FontStyleFlags.Italic);
            set => Flags = value ? _flags | FontStyleFlags.Italic : _flags & ~FontStyleFlags.Italic;
        }

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

        public virtual bool ShouldSerializeFlags() => _flags != default;

        public virtual bool ShouldSerializeRegular() => false;

        public virtual bool ShouldSerializeBold() => false;

        public virtual bool ShouldSerializeItalic() => false;
    }
}
