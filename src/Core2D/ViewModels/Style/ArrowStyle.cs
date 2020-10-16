using System.Collections.Generic;

namespace Core2D.Style
{
    public class ArrowStyle : BaseStyle
    {
        private ArrowType _arrowType;
        private bool _isStroked;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;

        public ArrowType ArrowType
        {
            get => _arrowType;
            set => RaiseAndSetIfChanged(ref _arrowType, value);
        }

        public bool IsStroked
        {
            get => _isStroked;
            set => RaiseAndSetIfChanged(ref _isStroked, value);
        }

        public bool IsFilled
        {
            get => _isFilled;
            set => RaiseAndSetIfChanged(ref _isFilled, value);
        }

        public double RadiusX
        {
            get => _radiusX;
            set => RaiseAndSetIfChanged(ref _radiusX, value);
        }

        public double RadiusY
        {
            get => _radiusY;
            set => RaiseAndSetIfChanged(ref _radiusY, value);
        }

        public ArrowStyle() : base()
        {
        }

        public ArrowStyle(BaseStyle source) : this()
        {
            Stroke = (BaseColor)source.Stroke.Copy(null);
            Fill = (BaseColor)source.Fill.Copy(null);
            Thickness = source.Thickness;
            LineCap = source.LineCap;
            Dashes = source.Dashes ?? (default);
            DashOffset = source.DashOffset;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ArrowStyle()
            {
                Name = this.Name,
                Stroke = (BaseColor)this.Stroke.Copy(shared),
                Fill = (BaseColor)this.Fill.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = this.DashOffset,
                ArrowType = this.ArrowType,
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled,
                RadiusX = this.RadiusX,
                RadiusY = this.RadiusY
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

        public virtual bool ShouldSerializeArrowType() => _arrowType != default;

        public virtual bool ShouldSerializeIsStroked() => _isStroked != default;

        public virtual bool ShouldSerializeIsFilled() => _isFilled != default;

        public virtual bool ShouldSerializeRadiusX() => _radiusX != default;

        public virtual bool ShouldSerializeRadiusY() => _radiusY != default;
    }
}
