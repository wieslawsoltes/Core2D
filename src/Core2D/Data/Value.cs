// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Attributes;

namespace Core2D.Data
{
    /// <summary>
    /// Record value.
    /// </summary>
    public class Value : ObservableObject
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

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Value"/> instance.
        /// </summary>
        /// <param name="content">The value content.</param>
        /// <returns>The new instance of the <see cref="Value"/> class.</returns>
        public static Value Create(string content) => new Value() { Content = content };

        /// <summary>
        /// Check whether the <see cref="Content"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeContent() => !string.IsNullOrWhiteSpace(_content);
    }
}
