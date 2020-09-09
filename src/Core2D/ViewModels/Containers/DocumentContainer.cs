using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Containers
{
    /// <summary>
    /// Document container.
    /// </summary>
    public class DocumentContainer : ObservableObject, IDocumentContainer
    {
        private bool _isExpanded = true;
        private ImmutableArray<IPageContainer> _pages;

        /// <inheritdoc/>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Update(ref _isExpanded, value);
        }

        /// <inheritdoc/>
        public ImmutableArray<IPageContainer> Pages
        {
            get => _pages;
            set => Update(ref _pages, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var page in Pages)
            {
                isDirty |= page.IsDirty();
            }

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var page in Pages)
            {
                page.Invalidate();
            }
        }

        /// <summary>
        /// Check whether the <see cref="IsExpanded"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsExpanded() => _isExpanded != default;

        /// <summary>
        /// Check whether the <see cref="Pages"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePages() => true;
    }
}
