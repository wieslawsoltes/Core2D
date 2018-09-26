// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Attributes;

namespace Core2D.Containers
{
    /// <summary>
    /// Document container.
    /// </summary>
    public class DocumentContainer : ObservableObject, IDocumentContainer
    {
        private IProjectContainer _owner;
        private bool _isExpanded = true;
        private ImmutableArray<IPageContainer> _pages;

        /// <inheritdoc/>
        public IProjectContainer Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <inheritdoc/>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Update(ref _isExpanded, value);
        }

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<IPageContainer> Pages
        {
            get => _pages;
            set => Update(ref _pages, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOwner() => _owner != null;

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
