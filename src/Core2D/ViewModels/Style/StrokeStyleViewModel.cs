using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class StrokeStyleViewModel : ViewModelBase
    {
        public static LineCap[] LineCapValues { get; } = (LineCap[])Enum.GetValues(typeof(LineCap));

        public static ArrowType[] ArrowTypeValues { get; } = (ArrowType[])Enum.GetValues(typeof(ArrowType));

        [AutoNotify] private BaseColorViewModel _colorViewModel;
        [AutoNotify] private double _thickness;
        [AutoNotify] private LineCap _lineCap;
        [AutoNotify] private string _dashes;
        [AutoNotify] private double _dashOffset;
        [AutoNotify] private ArrowStyleViewModel _startArrowStyleViewModel;
        [AutoNotify] private ArrowStyleViewModel _endArrowStyleViewModel;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new StrokeStyleViewModel()
            {
                Name = this.Name,
                ColorViewModel = (BaseColorViewModel)this.ColorViewModel.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = this.DashOffset,
                StartArrowStyleViewModel = (ArrowStyleViewModel)this.StartArrowStyleViewModel.Copy(shared),
                EndArrowStyleViewModel = (ArrowStyleViewModel)this.EndArrowStyleViewModel.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= ColorViewModel.IsDirty();
            isDirty |= StartArrowStyleViewModel.IsDirty();
            isDirty |= EndArrowStyleViewModel.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            ColorViewModel.Invalidate();
            StartArrowStyleViewModel.Invalidate();
            EndArrowStyleViewModel.Invalidate();
        }
    }
}
