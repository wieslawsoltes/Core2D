using System;
using System.Collections.Generic;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public class Options : ObservableObject
    {
        private bool _snapToGrid = true;
        private double _snapX = 15.0;
        private double _snapY = 15.0;
        private double _hitThreshold = 7.0;
        private MoveMode _moveMode = MoveMode.Point;
        private bool _defaultIsStroked = true;
        private bool _defaultIsFilled = false;
        private bool _defaultIsClosed = true;
        private FillRule _defaultFillRule = FillRule.EvenOdd;
        private bool _tryToConnect = false;

        public bool SnapToGrid
        {
            get => _snapToGrid;
            set => RaiseAndSetIfChanged(ref _snapToGrid, value);
        }

        public double SnapX
        {
            get => _snapX;
            set => RaiseAndSetIfChanged(ref _snapX, value);
        }

        public double SnapY
        {
            get => _snapY;
            set => RaiseAndSetIfChanged(ref _snapY, value);
        }

        public double HitThreshold
        {
            get => _hitThreshold;
            set => RaiseAndSetIfChanged(ref _hitThreshold, value);
        }

        public MoveMode MoveMode
        {
            get => _moveMode;
            set => RaiseAndSetIfChanged(ref _moveMode, value);
        }

        public bool DefaultIsStroked
        {
            get => _defaultIsStroked;
            set => RaiseAndSetIfChanged(ref _defaultIsStroked, value);
        }

        public bool DefaultIsFilled
        {
            get => _defaultIsFilled;
            set => RaiseAndSetIfChanged(ref _defaultIsFilled, value);
        }

        public bool DefaultIsClosed
        {
            get => _defaultIsClosed;
            set => RaiseAndSetIfChanged(ref _defaultIsClosed, value);
        }

        public FillRule DefaultFillRule
        {
            get => _defaultFillRule;
            set => RaiseAndSetIfChanged(ref _defaultFillRule, value);
        }

        public bool TryToConnect
        {
            get => _tryToConnect;
            set => RaiseAndSetIfChanged(ref _tryToConnect, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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

        public virtual bool ShouldSerializeSnapToGrid() => _snapToGrid != default;

        public virtual bool ShouldSerializeSnapX() => _snapX != default;

        public virtual bool ShouldSerializeSnapY() => _snapY != default;

        public virtual bool ShouldSerializeHitThreshold() => _hitThreshold != default;

        public virtual bool ShouldSerializeMoveMode() => _moveMode != default;

        public virtual bool ShouldSerializeDefaultIsStroked() => _defaultIsStroked != default;

        public virtual bool ShouldSerializeDefaultIsFilled() => _defaultIsFilled != default;

        public virtual bool ShouldSerializeDefaultIsClosed() => _defaultIsClosed != default;

        public virtual bool ShouldSerializeDefaultFillRule() => _defaultFillRule != default;

        public virtual bool ShouldSerializeTryToConnect() => _tryToConnect != default;
    }
}
