namespace Core2D.Style
{
    public abstract class BaseStyle : ObservableObject
    {
        private BaseColor _stroke;
        private BaseColor _fill;
        private double _thickness;
        private LineCap _lineCap;
        private string _dashes;
        private double _dashOffset;

        public BaseColor Stroke
        {
            get => _stroke;
            set => RaiseAndSetIfChanged(ref _stroke, value);
        }

        public BaseColor Fill
        {
            get => _fill;
            set => RaiseAndSetIfChanged(ref _fill, value);
        }

        public double Thickness
        {
            get => _thickness;
            set => RaiseAndSetIfChanged(ref _thickness, value);
        }

        public LineCap LineCap
        {
            get => _lineCap;
            set => RaiseAndSetIfChanged(ref _lineCap, value);
        }

        public string Dashes
        {
            get => _dashes;
            set => RaiseAndSetIfChanged(ref _dashes, value);
        }

        public double DashOffset
        {
            get => _dashOffset;
            set => RaiseAndSetIfChanged(ref _dashOffset, value);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Stroke.IsDirty();
            isDirty |= Fill.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Stroke.Invalidate();
            Fill.Invalidate();
        }

        public virtual bool ShouldSerializeStroke() => _stroke != null;

        public virtual bool ShouldSerializeFill() => _fill != null;

        public virtual bool ShouldSerializeThickness() => _thickness != default;

        public virtual bool ShouldSerializeLineCap() => _lineCap != default;

        public virtual bool ShouldSerializeDashes() => !string.IsNullOrWhiteSpace(_dashes);

        public virtual bool ShouldSerializeDashOffset() => _dashOffset != default;
    }
}
