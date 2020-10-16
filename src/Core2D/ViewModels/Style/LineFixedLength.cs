using System;
using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Style
{
    public class LineFixedLength : ObservableObject
    {
        private LineFixedLengthFlags _flags;
        private ShapeState _startTrigger;
        private ShapeState _endTrigger;
        private double _length;

        public LineFixedLengthFlags Flags
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
            RaisePropertyChanged(nameof(Disabled));
            RaisePropertyChanged(nameof(Start));
            RaisePropertyChanged(nameof(End));
            RaisePropertyChanged(nameof(Vertical));
            RaisePropertyChanged(nameof(Horizontal));
            RaisePropertyChanged(nameof(All));
        }

        public bool Disabled
        {
            get => _flags == LineFixedLengthFlags.Disabled;
            set => Flags = value ? _flags | LineFixedLengthFlags.Disabled : _flags & ~LineFixedLengthFlags.Disabled;
        }

        public bool Start
        {
            get => _flags.HasFlag(LineFixedLengthFlags.Start);
            set => Flags = value ? _flags | LineFixedLengthFlags.Start : _flags & ~LineFixedLengthFlags.Start;
        }

        public bool End
        {
            get => _flags.HasFlag(LineFixedLengthFlags.End);
            set => Flags = value ? _flags | LineFixedLengthFlags.End : _flags & ~LineFixedLengthFlags.End;
        }

        public bool Vertical
        {
            get => _flags.HasFlag(LineFixedLengthFlags.Vertical);
            set => Flags = value ? _flags | LineFixedLengthFlags.Vertical : _flags & ~LineFixedLengthFlags.Vertical;
        }

        public bool Horizontal
        {
            get => _flags.HasFlag(LineFixedLengthFlags.Horizontal);
            set => Flags = value ? _flags | LineFixedLengthFlags.Horizontal : _flags & ~LineFixedLengthFlags.Horizontal;
        }

        public bool All
        {
            get => _flags.HasFlag(LineFixedLengthFlags.All);
            set => Flags = value ? _flags | LineFixedLengthFlags.All : _flags & ~LineFixedLengthFlags.All;
        }

        public ShapeState StartTrigger
        {
            get => _startTrigger;
            set => RaiseAndSetIfChanged(ref _startTrigger, value);
        }

        public ShapeState EndTrigger
        {
            get => _endTrigger;
            set => RaiseAndSetIfChanged(ref _endTrigger, value);
        }

        public double Length
        {
            get => _length;
            set => RaiseAndSetIfChanged(ref _length, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new LineFixedLength()
            {
                Flags = this.Flags,
                StartTrigger = (ShapeState)this.StartTrigger.Copy(shared),
                EndTrigger = (ShapeState)this.EndTrigger.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= StartTrigger.IsDirty();
            isDirty |= EndTrigger.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            StartTrigger.Invalidate();
            EndTrigger.Invalidate();
        }

        public static LineFixedLength Parse(string s)
        {
            var flags = (LineFixedLengthFlags)Enum.Parse(typeof(LineFixedLengthFlags), s, true);
            return new LineFixedLength()
            {
                Flags = flags,
                StartTrigger = new ShapeState() { Flags = ShapeStateFlags.Connector | ShapeStateFlags.Output },
                EndTrigger = new ShapeState() { Flags = ShapeStateFlags.Connector | ShapeStateFlags.Input },
                Length = 15.0
            };
        }

        public override string ToString()
        {
            return _flags.ToString();
        }

        public virtual bool ShouldSerializeFlags() => _flags != default;

        public virtual bool ShouldSerializeDisabled() => false;

        public virtual bool ShouldSerializeStart() => false;

        public virtual bool ShouldSerializeEnd() => false;

        public virtual bool ShouldSerializeVertical() => false;

        public virtual bool ShouldSerializeHorizontal() => false;

        public virtual bool ShouldSerializeAll() => false;

        public virtual bool ShouldSerializeStartTrigger() => _startTrigger != null;

        public virtual bool ShouldSerializeEndTrigger() => _endTrigger != null;

        public virtual bool ShouldSerializeLength() => _length != default;
    }
}
