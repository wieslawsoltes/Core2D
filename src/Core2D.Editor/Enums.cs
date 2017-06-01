// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Path;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Static enum value arrays.
    /// </summary>
    public static class Enums
    {
        private static LineCap[] _lineCapValues = (LineCap[])Enum.GetValues(typeof(LineCap));
        private static CurveOrientation[] _curveOrientationValues = (CurveOrientation[])Enum.GetValues(typeof(CurveOrientation));
        private static ArrowType[] _arrowTypeValues = (ArrowType[])Enum.GetValues(typeof(ArrowType));
        private static TextHAlignment[] _textHAlignmentValues = (TextHAlignment[])Enum.GetValues(typeof(TextHAlignment));
        private static TextVAlignment[] _textVAlignmentValues = (TextVAlignment[])Enum.GetValues(typeof(TextVAlignment));
        private static XMoveMode[] _moveModeValues = (XMoveMode[])Enum.GetValues(typeof(XMoveMode));
        private static XFillRule[] _xFillRuleValues = (XFillRule[])Enum.GetValues(typeof(XFillRule));
        private static XSweepDirection[] _xSweepDirectionValues = (XSweepDirection[])Enum.GetValues(typeof(XSweepDirection));
        private static PointAlignment[] _pointAlignmentValues = (PointAlignment[])Enum.GetValues(typeof(PointAlignment));

        /// <summary>
        /// The <see cref="LineCap"/> enum values.
        /// </summary>
        public static LineCap[] LineCapValues => _lineCapValues;

        /// <summary>
        /// The <see cref="CurveOrientation"/> enum values.
        /// </summary>
        public static CurveOrientation[] CurveOrientationValues => _curveOrientationValues;

        /// <summary>
        /// The <see cref="ArrowType"/> enum values.
        /// </summary>
        public static ArrowType[] ArrowTypeValues => _arrowTypeValues;

        /// <summary>
        /// The <see cref="TextHAlignment"/> enum values.
        /// </summary>
        public static TextHAlignment[] TextHAlignmentValues => _textHAlignmentValues;

        /// <summary>
        /// The <see cref="TextVAlignment"/> enum values.
        /// </summary>
        public static TextVAlignment[] TextVAlignmentValues => _textVAlignmentValues;

        /// <summary>
        /// The <see cref="XMoveMode"/> enum values.
        /// </summary>
        public static XMoveMode[] MoveModeValues => _moveModeValues;

        /// <summary>
        /// The <see cref="XFillRule"/> enum values.
        /// </summary>
        public static XFillRule[] XFillRuleValues => _xFillRuleValues;

        /// <summary>
        /// The <see cref="XSweepDirection"/> enum values.
        /// </summary>
        public static XSweepDirection[] XSweepDirectionValues => _xSweepDirectionValues;

        /// <summary>
        /// The <see cref="PointAlignment"/> enum values.
        /// </summary>
        public static PointAlignment[] PointAlignmentValues => _pointAlignmentValues;
    }
}
