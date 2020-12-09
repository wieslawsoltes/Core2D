using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class ShapeStyleViewModel : ViewModelBase
    {
        [AutoNotify] private StrokeStyleViewModel _stroke;
        [AutoNotify] private FillStyleViewModel _fill;
        [AutoNotify] private TextStyleViewModel _textStyleViewModel;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyleViewModel()
            {
                Name = this.Name,
                Stroke = (StrokeStyleViewModel)this.Stroke.Copy(shared),
                Fill = (FillStyleViewModel)this.Fill.Copy(shared),
                TextStyleViewModel = (TextStyleViewModel)this.TextStyleViewModel.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Stroke.IsDirty();
            isDirty |= Fill.IsDirty();
            isDirty |= TextStyleViewModel.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Stroke.Invalidate();
            Fill.Invalidate();
            TextStyleViewModel.Invalidate();
        }
    }
}
