// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// The static enum value properties.
    /// </summary>
    public static class Enums
    {
        private static Tool[] _toolValues = (Tool[])Enum.GetValues(typeof(Tool));
        private static PathTool[] _pathToolValues = (PathTool[])Enum.GetValues(typeof(PathTool));
        private static LineCap[] _lineCapValues = (LineCap[])Enum.GetValues(typeof(LineCap));
        private static ArrowType[] _arrowTypeValues = (ArrowType[])Enum.GetValues(typeof(ArrowType));
        private static TextHAlignment[] _textHAlignmentValues = (TextHAlignment[])Enum.GetValues(typeof(TextHAlignment));
        private static TextVAlignment[] _textVAlignmentValues = (TextVAlignment[])Enum.GetValues(typeof(TextVAlignment));
        private static MoveMode[] _moveModeValues = (MoveMode[])Enum.GetValues(typeof(MoveMode));
        private static XFillRule[] _xFillRuleValues = (XFillRule[])Enum.GetValues(typeof(XFillRule));
        private static XSweepDirection[] _xSweepDirectionValues = (XSweepDirection[])Enum.GetValues(typeof(XSweepDirection));

        /// <summary>
        /// The <see cref="Tool"/> enum values.
        /// </summary>
        public static Tool[] ToolValues
        {
            get { return _toolValues; }
        }

        /// <summary>
        /// The <see cref="PathTool"/> enum values.
        /// </summary>
        public static PathTool[] PathToolValues
        {
            get { return _pathToolValues; }
        }

        /// <summary>
        /// The <see cref="LineCap"/> enum values.
        /// </summary>
        public static LineCap[] LineCapValues
        {
            get { return _lineCapValues; }
        }

        /// <summary>
        /// The <see cref="ArrowType"/> enum values.
        /// </summary>
        public static ArrowType[] ArrowTypeValues
        {
            get { return _arrowTypeValues; }
        }

        /// <summary>
        /// The <see cref="TextHAlignment"/> enum values.
        /// </summary>
        public static TextHAlignment[] TextHAlignmentValues
        {
            get { return _textHAlignmentValues; }
        }

        /// <summary>
        /// The <see cref="TextVAlignment"/> enum values.
        /// </summary>
        public static TextVAlignment[] TextVAlignmentValues
        {
            get { return _textVAlignmentValues; }
        }

        /// <summary>
        /// The <see cref="MoveMode"/> enum values.
        /// </summary>
        public static MoveMode[] MoveModeValues
        {
            get { return _moveModeValues; }
        }

        /// <summary>
        /// The <see cref="XFillRule"/> enum values.
        /// </summary>
        public static XFillRule[] XFillRuleValues
        {
            get { return _xFillRuleValues; }
        }

        /// <summary>
        /// The <see cref="XSweepDirection"/> enum values.
        /// </summary>
        public static XSweepDirection[] XSweepDirectionValues
        {
            get { return _xSweepDirectionValues; }
        }
    }
}
