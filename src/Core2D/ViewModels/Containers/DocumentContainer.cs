using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Containers
{
    public partial class DocumentContainer : BaseContainer
    {
        [AutoNotify] private bool _isExpanded = true;
        [AutoNotify] private ImmutableArray<PageContainer> _pages;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
