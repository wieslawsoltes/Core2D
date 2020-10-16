using System;
using System.Collections.Generic;

namespace Core2D.Renderer
{
    public class ShapeState : ObservableObject
    {
        private ShapeStateFlags _flags;

        public ShapeStateFlags Flags
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
            RaisePropertyChanged(nameof(Default));
            RaisePropertyChanged(nameof(Visible));
            RaisePropertyChanged(nameof(Printable));
            RaisePropertyChanged(nameof(Locked));
            RaisePropertyChanged(nameof(Size));
            RaisePropertyChanged(nameof(Thickness));
            RaisePropertyChanged(nameof(Connector));
            RaisePropertyChanged(nameof(None));
            RaisePropertyChanged(nameof(Standalone));
            RaisePropertyChanged(nameof(Input));
            RaisePropertyChanged(nameof(Output));
        }

        public bool Default
        {
            get => _flags == ShapeStateFlags.Default;
            set => Flags = value ? _flags | ShapeStateFlags.Default : _flags & ~ShapeStateFlags.Default;
        }

        public bool Visible
        {
            get => _flags.HasFlag(ShapeStateFlags.Visible);
            set => Flags = value ? _flags | ShapeStateFlags.Visible : _flags & ~ShapeStateFlags.Visible;
        }

        public bool Printable
        {
            get => _flags.HasFlag(ShapeStateFlags.Printable);
            set => Flags = value ? _flags | ShapeStateFlags.Printable : _flags & ~ShapeStateFlags.Printable;
        }

        public bool Locked
        {
            get => _flags.HasFlag(ShapeStateFlags.Locked);
            set => Flags = value ? _flags | ShapeStateFlags.Locked : _flags & ~ShapeStateFlags.Locked;
        }

        public bool Size
        {
            get => _flags.HasFlag(ShapeStateFlags.Size);
            set => Flags = value ? _flags | ShapeStateFlags.Size : _flags & ~ShapeStateFlags.Size;
        }

        public bool Thickness
        {
            get => _flags.HasFlag(ShapeStateFlags.Thickness);
            set => Flags = value ? _flags | ShapeStateFlags.Thickness : _flags & ~ShapeStateFlags.Thickness;
        }

        public bool Connector
        {
            get => _flags.HasFlag(ShapeStateFlags.Connector);
            set => Flags = value ? _flags | ShapeStateFlags.Connector : _flags & ~ShapeStateFlags.Connector;
        }

        public bool None
        {
            get => _flags.HasFlag(ShapeStateFlags.None);
            set => Flags = value ? _flags | ShapeStateFlags.None : _flags & ~ShapeStateFlags.None;
        }

        public bool Standalone
        {
            get => _flags.HasFlag(ShapeStateFlags.Standalone);
            set => Flags = value ? _flags | ShapeStateFlags.Standalone : _flags & ~ShapeStateFlags.Standalone;
        }

        public bool Input
        {
            get => _flags.HasFlag(ShapeStateFlags.Input);
            set => Flags = value ? _flags | ShapeStateFlags.Input : _flags & ~ShapeStateFlags.Input;
        }

        public bool Output
        {
            get => _flags.HasFlag(ShapeStateFlags.Output);
            set => Flags = value ? _flags | ShapeStateFlags.Output : _flags & ~ShapeStateFlags.Output;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeState()
            {
                Flags = this._flags
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

        public static ShapeState Parse(string s)
        {
            var flags = (ShapeStateFlags)Enum.Parse(typeof(ShapeStateFlags), s, true);
            return new ShapeState()
            {
                Flags = flags
            };
        }

        public override string ToString()
        {
            return _flags.ToString();
        }

        public virtual bool ShouldSerializeFlags() => _flags != default;

        public virtual bool ShouldSerializeDefault() => false;

        public virtual bool ShouldSerializeVisible() => false;

        public virtual bool ShouldSerializePrintable() => false;

        public virtual bool ShouldSerializeLocked() => false;

        public virtual bool ShouldSerializeSize() => false;

        public virtual bool ShouldSerializeThickness() => false;

        public virtual bool ShouldSerializeConnector() => false;

        public virtual bool ShouldSerializeNone() => false;

        public virtual bool ShouldSerializeStandalone() => false;

        public virtual bool ShouldSerializeInput() => false;

        public virtual bool ShouldSerializeOutput() => false;
    }
}
