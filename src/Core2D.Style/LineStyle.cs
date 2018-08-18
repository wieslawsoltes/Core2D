// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Line style.
    /// </summary>
    public class LineStyle : ObservableObject, ILineStyle
    {
        private bool _isCurved;
        private double _curvature;
        private CurveOrientation _curveOrientation;
        private ILineFixedLength _fixedLength;

        /// <inheritdoc/>
        public bool IsCurved
        {
            get => _isCurved;
            set => Update(ref _isCurved, value);
        }

        /// <inheritdoc/>
        public double Curvature
        {
            get => _curvature;
            set => Update(ref _curvature, value);
        }

        /// <inheritdoc/>
        public CurveOrientation CurveOrientation
        {
            get => _curveOrientation;
            set => Update(ref _curveOrientation, value);
        }

        /// <inheritdoc/>
        public ILineFixedLength FixedLength
        {
            get => _fixedLength;
            set => Update(ref _fixedLength, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="LineStyle"/> instance.
        /// </summary>
        /// <param name="name">The line style name.</param>
        /// <param name="isCurved">The flag indicating whether line is curved.</param>
        /// <param name="curvature">The line curvature.</param>
        /// <param name="curveOrientation">The curve orientation.</param>
        /// <param name="fixedLength">The line style fixed length.</param>
        /// <returns>The new instance of the <see cref="LineStyle"/> class.</returns>
        public static ILineStyle Create(string name = "", bool isCurved = false, double curvature = 50.0, CurveOrientation curveOrientation = CurveOrientation.Auto, ILineFixedLength fixedLength = null)
        {
            return new LineStyle()
            {
                Name = name,
                IsCurved = isCurved,
                Curvature = curvature,
                CurveOrientation = curveOrientation,
                FixedLength = fixedLength ?? LineFixedLength.Create()
            };
        }

        /// <summary>
        /// Check whether the <see cref="IsCurved"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsCurved() => _isCurved != default;

        /// <summary>
        /// Check whether the <see cref="Curvature"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurvature() => _curvature != default;

        /// <summary>
        /// Check whether the <see cref="CurveOrientation"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurveOrientation() => _curveOrientation != default;

        /// <summary>
        /// Check whether the <see cref="FixedLength"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFixedLength() => _fixedLength != null;
    }
}
