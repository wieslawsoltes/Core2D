using System;

namespace Core2D.ViewModels.Containers
{
    public partial class PageContainerViewModel : BaseContainerViewModel
    {
        [AutoNotify] private TemplateContainerViewModel _template;

        public PageContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void InvalidateLayer()
        {
            base.InvalidateLayer();

            _template?.InvalidateLayer();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_template is { })
            {
                isDirty |= _template.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _template?.Invalidate();
        }
    }
}
