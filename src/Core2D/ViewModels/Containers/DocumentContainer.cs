using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Containers
{
    public class DocumentContainer : BaseContainer
    {
        private bool _isExpanded = true;
        private ImmutableArray<PageContainer> _pages;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        public ImmutableArray<PageContainer> Pages
        {
            get => _pages;
            set => RaiseAndSetIfChanged(ref _pages, value);
        }

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

        public virtual bool ShouldSerializeIsExpanded() => _isExpanded != default;

        public virtual bool ShouldSerializePages() => true;
    }
}
