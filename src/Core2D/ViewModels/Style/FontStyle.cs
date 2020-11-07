using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class FontStyle : ViewModelBase
    {
        private FontStyleFlags _flags;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public FontStyleFlags Flags
        {
            get => _flags;
            set
            {
                RaiseAndSetIfChanged(ref _flags, value);
                NotifyAll();
            }
        }

        private void NotifyAll()
        {
            RaisePropertyChanged(nameof(Regular));
            RaisePropertyChanged(nameof(Bold));
            RaisePropertyChanged(nameof(Italic));
        }

        [IgnoreDataMember]
        public bool Regular
        {
            get => _flags == FontStyleFlags.Regular;
            set => Flags = value ? _flags | FontStyleFlags.Regular : _flags & ~FontStyleFlags.Regular;
        }

        [IgnoreDataMember]
        public bool Bold
        {
            get => _flags.HasFlag(FontStyleFlags.Bold);
            set => Flags = value ? _flags | FontStyleFlags.Bold : _flags & ~FontStyleFlags.Bold;
        }

        [IgnoreDataMember]
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
    }
}
