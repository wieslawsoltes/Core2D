// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Path;
using Core2D.Containers;
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
        private static MoveMode[] _moveModeValues = (MoveMode[])Enum.GetValues(typeof(MoveMode));
        private static FillRule[] _xFillRuleValues = (FillRule[])Enum.GetValues(typeof(FillRule));
        private static SweepDirection[] _xSweepDirectionValues = (SweepDirection[])Enum.GetValues(typeof(SweepDirection));
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
        /// The <see cref="MoveMode"/> enum values.
        /// </summary>
        public static MoveMode[] MoveModeValues => _moveModeValues;

        /// <summary>
        /// The <see cref="FillRule"/> enum values.
        /// </summary>
        public static FillRule[] XFillRuleValues => _xFillRuleValues;

        /// <summary>
        /// The <see cref="SweepDirection"/> enum values.
        /// </summary>
        public static SweepDirection[] XSweepDirectionValues => _xSweepDirectionValues;

        /// <summary>
        /// The <see cref="PointAlignment"/> enum values.
        /// </summary>
        public static PointAlignment[] PointAlignmentValues => _pointAlignmentValues;
    }
}
