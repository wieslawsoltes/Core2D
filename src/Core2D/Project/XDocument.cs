// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Attributes;

namespace Core2D.Project
{
    /// <summary>
    /// Document model.
    /// </summary>
    public sealed class XDocument : XSelectable
    {
        private string _name;
        private bool _isExpanded = true;
        private ImmutableArray<XPage> _pages;

        /// <summary>
        /// Gets or sets document name.
        /// </summary>
        [Name]
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating whether document is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Update(ref _isExpanded, value); }
        }

        /// <summary>
        /// Gets or sets document pages.
        /// </summary>
        [Content]
        public ImmutableArray<XPage> Pages
        {
            get { return _pages; }
            set { Update(ref _pages, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XDocument"/> class.
        /// </summary>
        public XDocument()
            : base()
        {
            _pages = ImmutableArray.Create<XPage>();
        }

        /// <summary>
        /// Creates a new <see cref="XDocument"/> instance.
        /// </summary>
        /// <param name="name">The document name.</param>
        /// <returns>The new instance of the <see cref="XDocument"/> class.</returns>
        public static XDocument Create(string name = "Document")
        {
            return new XDocument()
            {
                Name = name
            };
        }
    }
}
