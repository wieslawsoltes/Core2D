using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Path;

namespace Core2D.Containers
{
    [DataContract(IsReference = true)]
    public class Options : ViewModelBase
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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool SnapToGrid
        {
            get => _snapToGrid;
            set => RaiseAndSetIfChanged(ref _snapToGrid, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double SnapX
        {
            get => _snapX;
            set => RaiseAndSetIfChanged(ref _snapX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double SnapY
        {
            get => _snapY;
            set => RaiseAndSetIfChanged(ref _snapY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double HitThreshold
        {
            get => _hitThreshold;
            set => RaiseAndSetIfChanged(ref _hitThreshold, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public MoveMode MoveMode
        {
            get => _moveMode;
            set => RaiseAndSetIfChanged(ref _moveMode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DefaultIsStroked
        {
            get => _defaultIsStroked;
            set => RaiseAndSetIfChanged(ref _defaultIsStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DefaultIsFilled
        {
            get => _defaultIsFilled;
            set => RaiseAndSetIfChanged(ref _defaultIsFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DefaultIsClosed
        {
            get => _defaultIsClosed;
            set => RaiseAndSetIfChanged(ref _defaultIsClosed, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public FillRule DefaultFillRule
        {
            get => _defaultFillRule;
            set => RaiseAndSetIfChanged(ref _defaultFillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
    }
}
