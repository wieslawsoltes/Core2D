// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Attributes;

namespace Core2D.Project
{
    /// <summary>
    /// Document model.
    /// </summary>
    public class XDocument : XSelectable
    {
        private string _name;
        private bool _isExpanded = true;
        private ImmutableArray<XContainer> _pages;

        /// <summary>
        /// Gets or sets document name.
        /// </summary>
        [Name]
        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

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
        public ImmutableArray<XContainer> Pages
        {
            get => _pages;
            set => Update(ref _pages, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XDocument"/> class.
        /// </summary>
        public XDocument() : base() => _pages = ImmutableArray.Create<XContainer>();

        /// <summary>
        /// Creates a new <see cref="XDocument"/> instance.
        /// </summary>
        /// <param name="name">The document name.</param>
        /// <returns>The new instance of the <see cref="XDocument"/> class.</returns>
        public static XDocument Create(string name = "Document") => new XDocument() { Name = name };
    }
}
