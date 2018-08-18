// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core2D.Renderer
{
    /// <summary>
    /// Transformation matrix.
    /// </summary>
    public class MatrixObject : ObservableObject
    {
        private double _m11;
        private double _m12;
        private double _m21;
        private double _m22;
        private double _offsetX;
        private double _offsetY;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixObject"/>.
        /// </summary>
        public MatrixObject()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixObject"/>.
        /// </summary>
        /// <param name="m11">The value of the first row and first column.</param>
        /// <param name="m12">The value of the first row and second column.</param>
        /// <param name="m21">The value of the second row and first column.</param>
        /// <param name="m22">The value of the second row and second column.</param>
        /// <param name="offsetX">The value of the third row and first column.</param>
        /// <param name="offsetY">The value of the third row and second column.</param>
        public MatrixObject(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
            : base()
        {
            _m11 = m11;
            _m12 = m12;
            _m21 = m21;
            _m22 = m22;
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        /// <summary>
        /// Gets or sets the value of the first row and first column of this <see cref="MatrixObject"/>.
        /// </summary>
        public double M11
        {
            get => _m11;
            set => Update(ref _m11, value);
        }

        /// <summary>
        /// Gets or sets the value of the first row and second column of this <see cref="MatrixObject"/>.
        /// </summary>
        public double M12
        {
            get => _m12;
            set => Update(ref _m12, value);
        }

        /// <summary>
        /// Gets or sets the value of the second row and first column of this <see cref="MatrixObject"/>.
        /// </summary>
        public double M21
        {
            get => _m21;
            set => Update(ref _m21, value);
        }

        /// <summary>
        /// Gets or sets the value of the second row and second column of this <see cref="MatrixObject"/>.
        /// </summary>
        public double M22
        {
            get => _m22;
            set => Update(ref _m22, value);
        }

        /// <summary>
        /// Gets or sets the value of the third row and first column of this <see cref="MatrixObject"/>.
        /// </summary>
        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        /// <summary>
        /// Gets or sets the value of the third row and second column of this <see cref="MatrixObject"/>.
        /// </summary>
        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an identity <see cref="MatrixObject"/>.
        /// </summary>
        public static readonly MatrixObject Identity = new MatrixObject(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

        /// <summary>
        /// Creates a new <see cref="MatrixObject"/> instance.
        /// </summary>
        /// <param name="m11">The value of the first row and first column.</param>
        /// <param name="m12">The value of the first row and second column.</param>
        /// <param name="m21">The value of the second row and first column.</param>
        /// <param name="m22">The value of the second row and second column.</param>
        /// <param name="offsetX">The value of the third row and first column.</param>
        /// <param name="offsetY">The value of the third row and second column.</param>
        /// <returns>The new instance of the <see cref="MatrixObject"/> class.</returns>
        public static MatrixObject Create(double m11 = 1.0, double m12 = 0.0, double m21 = 0.0, double m22 = 1.0, double offsetX = 0.0, double offsetY = 0.0)
        {
            return new MatrixObject(m11, m12, m21, m22, offsetX, offsetY);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", _m11, _m12, _m21, _m22, _offsetX, _offsetY);
        }

        /// <summary>
        /// Parses a <see cref="MatrixObject"/> string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The parsed <see cref="MatrixObject"/>.</returns>
        public static MatrixObject Parse(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            if (parts.Count == 6)
            {
                return MatrixObject.Create(
                    double.Parse(parts[0]),
                    double.Parse(parts[1]),
                    double.Parse(parts[2]),
                    double.Parse(parts[3]),
                    double.Parse(parts[4]),
                    double.Parse(parts[5]));
            }
            else
            {
                throw new FormatException("Invalid Matrix.");
            }
        }

        /// <summary>
        /// Check whether the <see cref="M11"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeM11() => _m11 != default;

        /// <summary>
        /// Check whether the <see cref="M12"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeM12() => _m12 != default;

        /// <summary>
        /// Check whether the <see cref="M21"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeM21() => _m21 != default;

        /// <summary>
        /// Check whether the <see cref="M22"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeM22() => _m22 != default;

        /// <summary>
        /// Check whether the <see cref="OffsetX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOffsetX() => _offsetX != default;

        /// <summary>
        /// Check whether the <see cref="OffsetY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOffsetY() => _offsetY != default;
    }
}
