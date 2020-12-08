using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class ShapeStyle : ViewModelBase
    {
        [AutoNotify] private StrokeStyle _stroke;
        [AutoNotify] private FillStyle _fill;
        [AutoNotify] private TextStyle _textStyle;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Stroke = (StrokeStyle)this.Stroke.Copy(shared),
                Fill = (FillStyle)this.Fill.Copy(shared),
                TextStyle = (TextStyle)this.TextStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Stroke.IsDirty();
            isDirty |= Fill.IsDirty();
            isDirty |= TextStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Stroke.Invalidate();
            Fill.Invalidate();
            TextStyle.Invalidate();
        }
    }
}
