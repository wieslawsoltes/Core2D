using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class StrokeStyle : ViewModelBase
    {
        public static LineCap[] LineCapValues { get; } = (LineCap[])Enum.GetValues(typeof(LineCap));

        public static ArrowType[] ArrowTypeValues { get; } = (ArrowType[])Enum.GetValues(typeof(ArrowType));

        [AutoNotify] private BaseColor _color;
        [AutoNotify] private double _thickness;
        [AutoNotify] private LineCap _lineCap;
        [AutoNotify] private string _dashes;
        [AutoNotify] private double _dashOffset;
        [AutoNotify] private ArrowStyle _startArrowStyle;
        [AutoNotify] private ArrowStyle _endArrowStyle;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new StrokeStyle()
            {
                Name = this.Name,
                Color = (BaseColor)this.Color.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = this.DashOffset,
                StartArrowStyle = (ArrowStyle)this.StartArrowStyle.Copy(shared),
                EndArrowStyle = (ArrowStyle)this.EndArrowStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Color.IsDirty();
            isDirty |= StartArrowStyle.IsDirty();
            isDirty |= EndArrowStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Color.Invalidate();
            StartArrowStyle.Invalidate();
            EndArrowStyle.Invalidate();
        }
    }
}
