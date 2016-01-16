// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Document model.
    /// </summary>
    [ContentProperty(nameof(Pages))]
    [RuntimeNameProperty(nameof(Name))]
    public class Document : ObservableResource
    {
        private string _name;
        private bool _isExpanded = true;
        private ImmutableArray<Page> _pages;

        /// <summary>
        /// Gets or sets document name.
        /// </summary>
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
        public ImmutableArray<Page> Pages
        {
            get { return _pages; }
            set { Update(ref _pages, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document()
            : base()
        {
            _pages = ImmutableArray.Create<Page>();
        }

        /// <summary>
        /// Creates a new <see cref="Document"/> instance.
        /// </summary>
        /// <param name="name">The document name.</param>
        /// <returns>The new instance of the <see cref="Document"/> class.</returns>
        public static Document Create(string name = "Document")
        {
            return new Document()
            {
                Name = name
            };
        }
    }
}
