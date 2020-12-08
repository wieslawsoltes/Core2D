using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class FillStyle : ViewModelBase
    {
        [AutoNotify] private BaseColor _color;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new FillStyle()
            {
                Name = this.Name,
                Color = (BaseColor)this.Color.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Color.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Color.Invalidate();
        }
    }
}
