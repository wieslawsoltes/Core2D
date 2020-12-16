using System;
using System.Collections.Immutable;

namespace Core2D.ViewModels.Containers
{
    public partial class DocumentContainerViewModel : BaseContainerViewModel
    {
        [AutoNotify] private ImmutableArray<PageContainerViewModel> _pages;

        public DocumentContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var page in Pages)
            {
                isDirty |= page.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var page in Pages)
            {
                page.Invalidate();
            }
        }
    }
}
