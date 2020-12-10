using System.Collections.Generic;

namespace Core2D.ViewModels.Style
{
    public partial class ShapeStyleViewModel : ViewModelBase
    {
        [AutoNotify] private StrokeStyleViewModel _stroke;
        [AutoNotify] private FillStyleViewModel _fill;
        [AutoNotify] private TextStyleViewModel _textStyle;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyleViewModel()
            {
                Name = this.Name,
                Stroke = (StrokeStyleViewModel)this._stroke.Copy(shared),
                Fill = (FillStyleViewModel)this._fill.Copy(shared),
                TextStyle = (TextStyleViewModel)this._textStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _stroke.IsDirty();
            isDirty |= _fill.IsDirty();
            isDirty |= _textStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _stroke.Invalidate();
            _fill.Invalidate();
            _textStyle.Invalidate();
        }
    }
}
