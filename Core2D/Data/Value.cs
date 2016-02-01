// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Record value.
    /// </summary>
    [ContentProperty(nameof(Content))]
    public sealed class Value : ObservableObject
    {
        private string _content;

        /// <summary>
        /// Gets or sets value content.
        /// </summary>
        public string Content
        {
            get { return _content; }
            set { Update(ref _content, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Value"/> instance.
        /// </summary>
        /// <param name="content">The value content.</param>
        /// <returns>The new instance of the <see cref="Value"/> class.</returns>
        public static Value Create(string content)
        {
            return new Value()
            {
                Content = content,
            };
        }
    }
}
