using System;
using System.Collections.Generic;
using Core2D.Input;

namespace Core2D.Editor
{
    public class ModifierViewModel : ViewModelBase
    {
        private ModifierFlags _flags;

        public ModifierFlags Flags
        {
            get => _flags;
            set
            {
                RaiseAndSetIfChanged(ref _flags, value);
                RaisePropertyChanged(nameof(None));
                RaisePropertyChanged(nameof(Alt));
                RaisePropertyChanged(nameof(Control));
                RaisePropertyChanged(nameof(Shift));
            }
        }

        public bool None
        {
            get => _flags.HasFlag(ModifierFlags.None);
            set => Flags = value ? _flags | ModifierFlags.None : _flags & ~ModifierFlags.None;
        }

        public bool Alt
        {
            get => _flags.HasFlag(ModifierFlags.Alt);
            set => Flags = value ? _flags | ModifierFlags.Alt : _flags & ~ModifierFlags.Alt;
        }

        public bool Control
        {
            get => _flags.HasFlag(ModifierFlags.Control);
            set => Flags = value ? _flags | ModifierFlags.Control : _flags & ~ModifierFlags.Control;
        }

        public bool Shift
        {
            get => _flags.HasFlag(ModifierFlags.Shift);
            set => Flags = value ? _flags | ModifierFlags.Shift : _flags & ~ModifierFlags.Shift;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public static ModifierViewModel Create(ModifierFlags flags = ModifierFlags.None) => new ModifierViewModel() { Flags = flags };

        public static ModifierViewModel Parse(string s) => Create((ModifierFlags)Enum.Parse(typeof(ModifierFlags), s, true));

        public override string ToString() => _flags.ToString();

        public ModifierViewModel Clone() => Create(_flags);
    }
}
