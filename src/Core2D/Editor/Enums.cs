// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Containers;
using Core2D.Path;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Static enum value arrays.
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// The <see cref="LineCap"/> enum values.
        /// </summary>
        public static LineCap[] LineCapValues { get; } = (LineCap[])Enum.GetValues(typeof(LineCap));

        /// <summary>
        /// The <see cref="CurveOrientation"/> enum values.
        /// </summary>
        public static CurveOrientation[] CurveOrientationValues { get; } = (CurveOrientation[])Enum.GetValues(typeof(CurveOrientation));

        /// <summary>
        /// The <see cref="ArrowType"/> enum values.
        /// </summary>
        public static ArrowType[] ArrowTypeValues { get; } = (ArrowType[])Enum.GetValues(typeof(ArrowType));

        /// <summary>
        /// The <see cref="TextHAlignment"/> enum values.
        /// </summary>
        public static TextHAlignment[] TextHAlignmentValues { get; } = (TextHAlignment[])Enum.GetValues(typeof(TextHAlignment));

        /// <summary>
        /// The <see cref="TextVAlignment"/> enum values.
        /// </summary>
        public static TextVAlignment[] TextVAlignmentValues { get; } = (TextVAlignment[])Enum.GetValues(typeof(TextVAlignment));

        /// <summary>
        /// The <see cref="MoveMode"/> enum values.
        /// </summary>
        public static MoveMode[] MoveModeValues { get; } = (MoveMode[])Enum.GetValues(typeof(MoveMode));

        /// <summary>
        /// The <see cref="FillRule"/> enum values.
        /// </summary>
        public static FillRule[] XFillRuleValues { get; } = (FillRule[])Enum.GetValues(typeof(FillRule));

        /// <summary>
        /// The <see cref="SweepDirection"/> enum values.
        /// </summary>
        public static SweepDirection[] XSweepDirectionValues { get; } = (SweepDirection[])Enum.GetValues(typeof(SweepDirection));

        /// <summary>
        /// The <see cref="PointAlignment"/> enum values.
        /// </summary>
        public static PointAlignment[] PointAlignmentValues { get; } = (PointAlignment[])Enum.GetValues(typeof(PointAlignment));
    }
}
