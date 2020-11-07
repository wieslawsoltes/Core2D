using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Renderer
{
    [DataContract(IsReference = true)]
    public class ShapeState : ViewModelBase
    {
        private ShapeStateFlags _flags;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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

        [IgnoreDataMember]
        public bool Default
        {
            get => _flags == ShapeStateFlags.Default;
            set => Flags = value ? _flags | ShapeStateFlags.Default : _flags & ~ShapeStateFlags.Default;
        }

        [IgnoreDataMember]
        public bool Visible
        {
            get => _flags.HasFlag(ShapeStateFlags.Visible);
            set => Flags = value ? _flags | ShapeStateFlags.Visible : _flags & ~ShapeStateFlags.Visible;
        }

        [IgnoreDataMember]
        public bool Printable
        {
            get => _flags.HasFlag(ShapeStateFlags.Printable);
            set => Flags = value ? _flags | ShapeStateFlags.Printable : _flags & ~ShapeStateFlags.Printable;
        }

        [IgnoreDataMember]
        public bool Locked
        {
            get => _flags.HasFlag(ShapeStateFlags.Locked);
            set => Flags = value ? _flags | ShapeStateFlags.Locked : _flags & ~ShapeStateFlags.Locked;
        }

        [IgnoreDataMember]
        public bool Size
        {
            get => _flags.HasFlag(ShapeStateFlags.Size);
            set => Flags = value ? _flags | ShapeStateFlags.Size : _flags & ~ShapeStateFlags.Size;
        }

        [IgnoreDataMember]
        public bool Thickness
        {
            get => _flags.HasFlag(ShapeStateFlags.Thickness);
            set => Flags = value ? _flags | ShapeStateFlags.Thickness : _flags & ~ShapeStateFlags.Thickness;
        }

        [IgnoreDataMember]
        public bool Connector
        {
            get => _flags.HasFlag(ShapeStateFlags.Connector);
            set => Flags = value ? _flags | ShapeStateFlags.Connector : _flags & ~ShapeStateFlags.Connector;
        }

        [IgnoreDataMember]
        public bool None
        {
            get => _flags.HasFlag(ShapeStateFlags.None);
            set => Flags = value ? _flags | ShapeStateFlags.None : _flags & ~ShapeStateFlags.None;
        }

        [IgnoreDataMember]
        public bool Standalone
        {
            get => _flags.HasFlag(ShapeStateFlags.Standalone);
            set => Flags = value ? _flags | ShapeStateFlags.Standalone : _flags & ~ShapeStateFlags.Standalone;
        }

        [IgnoreDataMember]
        public bool Input
        {
            get => _flags.HasFlag(ShapeStateFlags.Input);
            set => Flags = value ? _flags | ShapeStateFlags.Input : _flags & ~ShapeStateFlags.Input;
        }

        [IgnoreDataMember]
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
    }
}
