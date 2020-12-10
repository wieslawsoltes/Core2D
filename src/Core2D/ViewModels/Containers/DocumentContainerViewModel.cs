using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.ViewModels.Containers
{
    public partial class DocumentContainerViewModel : BaseContainerViewModel
    {
        [AutoNotify] private bool _isExpanded = true;
        [AutoNotify] private ImmutableArray<PageContainerViewModel> _pages;

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
