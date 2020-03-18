// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Attributes;

namespace Core2D.Data
{
    /// <summary>
    /// Record value.
    /// </summary>
    public class Value : ObservableObject, IValue
    {
        private string _content;

        /// <inheritdoc/>
        [Content]
        public string Content
        {
            get => _content;
            set => Update(ref _content, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new Value()
            {
                Name = this.Name,
                Content = this.Content
            };
        }

        /// <summary>
        /// Check whether the <see cref="Content"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeContent() => !string.IsNullOrWhiteSpace(_content);
    }
}
