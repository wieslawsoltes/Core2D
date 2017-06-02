// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Attributes;

namespace Core2D.Data.Database
{
    /// <summary>
    /// Record value.
    /// </summary>
    public class XValue : ObservableObject
    {
        private string _content;

        /// <summary>
        /// Gets or sets value content.
        /// </summary>
        [Content]
        public string Content
        {
            get => _content;
            set => Update(ref _content, value);
        }

        /// <summary>
        /// Creates a new <see cref="XValue"/> instance.
        /// </summary>
        /// <param name="content">The value content.</param>
        /// <returns>The new instance of the <see cref="XValue"/> class.</returns>
        public static XValue Create(string content) => new XValue() { Content = content };

        /// <summary>
        /// Check whether the <see cref="Content"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeContent() => !String.IsNullOrWhiteSpace(_content);
    }
}
