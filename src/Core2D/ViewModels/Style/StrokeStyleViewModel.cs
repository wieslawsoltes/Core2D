using System;
using System.Collections.Generic;
using Core2D.Model.Style;

namespace Core2D.ViewModels.Style
{
    public partial class StrokeStyleViewModel : ViewModelBase
    {
        public static LineCap[] LineCapValues { get; } = (LineCap[])Enum.GetValues(typeof(LineCap));

        public static ArrowType[] ArrowTypeValues { get; } = (ArrowType[])Enum.GetValues(typeof(ArrowType));

        [AutoNotify] private BaseColorViewModel _color;
        [AutoNotify] private double _thickness;
        [AutoNotify] private LineCap _lineCap;
        [AutoNotify] private string _dashes;
        [AutoNotify] private double _dashOffset;
        [AutoNotify] private ArrowStyleViewModel _startArrow;
        [AutoNotify] private ArrowStyleViewModel _endArrow;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new StrokeStyleViewModel()
            {
                Name = this.Name,
                Color = (BaseColorViewModel)this._color.Copy(shared),
                Thickness = this._thickness,
                LineCap = this._lineCap,
                Dashes = this._dashes,
                DashOffset = this._dashOffset,
                StartArrow = (ArrowStyleViewModel)this._startArrow.Copy(shared),
                EndArrow = (ArrowStyleViewModel)this._endArrow.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _color.IsDirty();
            isDirty |= _startArrow.IsDirty();
            isDirty |= _endArrow.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _color.Invalidate();
            _startArrow.Invalidate();
            _endArrow.Invalidate();
        }
    }
}
