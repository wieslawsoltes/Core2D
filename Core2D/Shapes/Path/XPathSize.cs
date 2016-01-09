// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Path size.
    /// </summary>
    public class XPathSize
    {
        /// <summary>
        /// Gets or sets width value.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets height value.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Creates a new <see cref="XPathSize"/> instance.
        /// </summary>
        /// <param name="width">The width value.</param>
        /// <param name="height">The height value.</param>
        /// <returns>The new instance of the <see cref="XPathSize"/> class.</returns>
        public static XPathSize Create(double width = 0.0, double height = 0.0)
        {
            return new XPathSize()
            {
                Width = width,
                Height = height
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0},{1}", Width, Height);
        }
    }
}
