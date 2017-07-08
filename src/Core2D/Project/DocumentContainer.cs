// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Attributes;

namespace Core2D.Project
{
    /// <summary>
    /// Document model.
    /// </summary>
    public class DocumentContainer : SelectableObject, ICopyable
    {
        private bool _isExpanded = true;
        private ImmutableArray<PageContainer> _pages;

        /// <summary>
        /// Gets or sets flag indicating whether document is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Update(ref _isExpanded, value);
        }

        /// <summary>
        /// Gets or sets document pages.
        /// </summary>
        [Content]
        public ImmutableArray<PageContainer> Pages
        {
            get => _pages;
            set => Update(ref _pages, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContainer"/> class.
        /// </summary>
        public DocumentContainer() : base() => _pages = ImmutableArray.Create<PageContainer>();

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="DocumentContainer"/> instance.
        /// </summary>
        /// <param name="name">The document name.</param>
        /// <returns>The new instance of the <see cref="DocumentContainer"/> class.</returns>
        public static DocumentContainer Create(string name = "Document") => new DocumentContainer() { Name = name };

        /// <summary>
        /// Check whether the <see cref="IsExpanded"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsExpanded() => _isExpanded != default(bool);

        /// <summary>
        /// Check whether the <see cref="Pages"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePages() => _pages.IsEmpty == false;
    }
}
