// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Core2D.Path
{
    /// <summary>
    /// Path size.
    /// </summary>
    public class XPathSize : ObservableObject, ICopyable
    {
        private double _width;
        private double _height;

        /// <summary>
        /// Gets or sets width value.
        /// </summary>
        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        /// <summary>
        /// Gets or sets height value.
        /// </summary>
        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

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
        public override string ToString() => $"{Width},{Height}";

        /// <summary>
        /// Check whether the <see cref="Width"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeWidth() => _width != default(double);

        /// <summary>
        /// Check whether the <see cref="Height"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeHeight() => _height != default(double);
    }
}
