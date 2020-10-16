using System.Collections.Generic;

namespace Core2D.Style
{
    public class ShapeStyle : BaseStyle
    {
        private LineStyle _lineStyle;
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;
        private TextStyle _textStyle;

        public LineStyle LineStyle
        {
            get => _lineStyle;
            set => RaiseAndSetIfChanged(ref _lineStyle, value);
        }

        public ArrowStyle StartArrowStyle
        {
            get => _startArrowStyle;
            set => RaiseAndSetIfChanged(ref _startArrowStyle, value);
        }

        public ArrowStyle EndArrowStyle
        {
            get => _endArrowStyle;
            set => RaiseAndSetIfChanged(ref _endArrowStyle, value);
        }

        public TextStyle TextStyle
        {
            get => _textStyle;
            set => RaiseAndSetIfChanged(ref _textStyle, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Stroke = (BaseColor)this.Stroke.Copy(shared),
                Fill = (BaseColor)this.Fill.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = this.DashOffset,
                LineStyle = (LineStyle)this.LineStyle.Copy(shared),
                TextStyle = (TextStyle)this.TextStyle.Copy(shared),
                StartArrowStyle = (ArrowStyle)this.StartArrowStyle.Copy(shared),
                EndArrowStyle = (ArrowStyle)this.EndArrowStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= LineStyle.IsDirty();
            isDirty |= StartArrowStyle.IsDirty();
            isDirty |= EndArrowStyle.IsDirty();
            isDirty |= TextStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            LineStyle.Invalidate();
            StartArrowStyle.Invalidate();
            EndArrowStyle.Invalidate();
            TextStyle.Invalidate();
        }

        public virtual bool ShouldSerializeLineStyle() => _lineStyle != null;

        public virtual bool ShouldSerializeStartArrowStyle() => _startArrowStyle != null;

        public virtual bool ShouldSerializeEndArrowStyle() => _endArrowStyle != null;

        public virtual bool ShouldSerializeTextStyle() => _textStyle != null;
    }
}
