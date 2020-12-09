using System.Collections.Generic;

namespace Core2D.Style
{
    public partial class FillStyleViewModel : ViewModelBase
    {
        [AutoNotify] private BaseColorViewModel _colorViewModel;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new FillStyleViewModel()
            {
                Name = this.Name,
                ColorViewModel = (BaseColorViewModel)this.ColorViewModel.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= ColorViewModel.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            ColorViewModel.Invalidate();
        }
    }
}
