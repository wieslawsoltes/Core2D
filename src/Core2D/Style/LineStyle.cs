// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Attributes;

namespace Core2D.Style
{
    /// <summary>
    /// Line style.
    /// </summary>
    public class LineStyle : ObservableObject
    {
        private bool _isCurved;
        private double _curvature;
        private CurveOrientation _curveOrientation;
        private LineFixedLength _fixedLength;

        /// <summary>
        /// Gets or sets value indicating whether line is curved.
        /// </summary>
        public bool IsCurved
        {
            get => _isCurved;
            set => Update(ref _isCurved, value);
        }

        /// <summary>
        /// Gets or sets line curvature.
        /// </summary>
        public double Curvature
        {
            get => _curvature;
            set => Update(ref _curvature, value);
        }

        /// <summary>
        /// Gets or sets curve orientation.
        /// </summary>
        public CurveOrientation CurveOrientation
        {
            get => _curveOrientation;
            set => Update(ref _curveOrientation, value);
        }

        /// <summary>
        /// Gets or sets line fixed length.
        /// </summary>
        public LineFixedLength FixedLength
        {
            get => _fixedLength;
            set => Update(ref _fixedLength, value);
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
        public static LineStyle Create(
            string name = "",
            bool isCurved = false,
            double curvature = 50.0,
            CurveOrientation curveOrientation = CurveOrientation.Auto,
            LineFixedLength fixedLength = null)
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
        /// Clones line style.
        /// </summary>
        /// <returns>The new instance of the <see cref="LineStyle"/> class.</returns>
        public LineStyle Clone()
        {
            return new LineStyle()
            {
                Name = Name,
                IsCurved = _isCurved,
                Curvature = _curvature,
                CurveOrientation = _curveOrientation,
                FixedLength = _fixedLength.Clone()
            };
        }

        /// <summary>
        /// Check whether the <see cref="IsCurved"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeIsCurved() => _isCurved != default(bool);

        /// <summary>
        /// Check whether the <see cref="Curvature"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeCurvature() => _curvature != default(double);

        /// <summary>
        /// Check whether the <see cref="CurveOrientation"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeCurveOrientation() => _curveOrientation != default(CurveOrientation);

        /// <summary>
        /// Check whether the <see cref="FixedLength"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeFixedLength() => _fixedLength != null;
    }
}
