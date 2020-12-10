using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class FillStyleViewModel : ViewModelBase
    {
        [AutoNotify] private BaseColorViewModel _color;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new FillStyleViewModel()
            {
                Name = this.Name,
                Color = (BaseColorViewModel)this._color.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _color.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _color.Invalidate();
        }
    }
}
