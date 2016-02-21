// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;

namespace Core2D.Style
{
    /// <summary>
    /// Line style.
    /// </summary>
    [RuntimeNameProperty(nameof(Name))]
    public sealed class LineStyle : ObservableObject
    {
        private string _name;
        private LineFixedLength _fixedLength;

        /// <summary>
        /// Gets or sets line style name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets line fixed length.
        /// </summary>
        public LineFixedLength FixedLength
        {
            get { return _fixedLength; }
            set { Update(ref _fixedLength, value); }
        }

        /// <summary>
        /// Creates a new <see cref="LineStyle"/> instance.
        /// </summary>
        /// <param name="name">The line style name.</param>
        /// <param name="fixedLength">The line style fixed length.</param>
        /// <returns>The new instance of the <see cref="LineStyle"/> class.</returns>
        public static LineStyle Create(string name = "", LineFixedLength fixedLength = null)
        {
            return new LineStyle()
            {
                Name = name,
                FixedLength = fixedLength ?? LineFixedLength.Create()
            };
        }
    }
}
