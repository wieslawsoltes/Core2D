// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Renderer;
using Core2D.Style;
using static System.Math;

namespace Core2D.Shapes
{
    /// <summary>
    /// Line shape extension methods.
    /// </summary>
    public static class LineShapeExtensions
    {
        /// <summary>
        /// Get <see cref="ILineShape"/> maximum length using <see cref="LineFixedLengthFlags"/>.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="x1">The calculated X coordinate for <see cref="ILineShape.Start"/> point.</param>
        /// <param name="y1">The calculated Y coordinate for <see cref="ILineShape.Start"/> point.</param>
        /// <param name="x2">The calculated X coordinate for <see cref="ILineShape.End"/> point.</param>
        /// <param name="y2">The calculated Y coordinate for <see cref="ILineShape.End"/> point.</param>
        public static void GetMaxLength(this ILineShape line, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            var ls = line.Style.LineStyle;

            if (ls.FixedLength.Flags == LineFixedLengthFlags.Disabled)
                return;

            if (ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.All))
            {
                GetMaxLengthAll(line, ref x1, ref y1, ref x2, ref y2);
            }
            else
            {
                if (ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Vertical))
                {
                    bool isVertical = Round(x1, 1) == Round(x2, 1);
                    if (isVertical)
                    {
                        GetMaxLengthVertical(line, ref y1, ref y2);
                    }
                }

                if (ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Horizontal))
                {
                    bool isHorizontal = Round(y1, 1) == Round(y2, 1);
                    if (isHorizontal)
                    {
                        GetMaxLengthHorizontal(line, ref x1, ref x2);
                    }
                }
            }
        }

        /// <summary>
        /// Get <see cref="ILineShape"/> maximum length for <see cref="LineFixedLengthFlags.All"/> mode.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="x1">The calculated X coordinate for <see cref="ILineShape.Start"/> point.</param>
        /// <param name="y1">The calculated Y coordinate for <see cref="ILineShape.Start"/> point.</param>
        /// <param name="x2">The calculated X coordinate for <see cref="ILineShape.End"/> point.</param>
        /// <param name="y2">The calculated Y coordinate for <see cref="ILineShape.End"/> point.</param>
        public static void GetMaxLengthAll(this ILineShape line, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            var ls = line.Style.LineStyle;

            bool shortenStart = ls.FixedLength.StartTrigger.Flags != ShapeStateFlags.Default
                && line.Start.State.Flags.HasFlag(ls.FixedLength.StartTrigger.Flags)
                && ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Start);

            bool shortenEnd = ls.FixedLength.EndTrigger.Flags != ShapeStateFlags.Default
                && line.End.State.Flags.HasFlag(ls.FixedLength.EndTrigger.Flags)
                && ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.End);

            if (shortenStart && !shortenEnd)
            {
                double dx = x1 - x2;
                double dy = y1 - y2;
                double distance = Sqrt(dx * dx + dy * dy);
                x1 = x2 - (x2 - x1) / distance * ls.FixedLength.Length;
                y1 = y2 - (y2 - y1) / distance * ls.FixedLength.Length;
            }

            if (!shortenStart && shortenEnd)
            {
                double dx = x2 - x1;
                double dy = y2 - y1;
                double distance = Sqrt(dx * dx + dy * dy);
                x2 = x1 - (x1 - x2) / distance * ls.FixedLength.Length;
                y2 = y1 - (y1 - y2) / distance * ls.FixedLength.Length;
            }
        }

        /// <summary>
        /// Get <see cref="ILineShape"/> maximum length for <see cref="LineFixedLengthFlags.Horizontal"/> mode.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="x1">The calculated X coordinate for <see cref="ILineShape.Start"/> point.</param>
        /// <param name="x2">The calculated X coordinate for <see cref="ILineShape.End"/> point.</param>
        public static void GetMaxLengthHorizontal(this ILineShape line, ref double x1, ref double x2)
        {
            var ls = line.Style.LineStyle;

            bool shortenStart = ls.FixedLength.StartTrigger.Flags != ShapeStateFlags.Default
                && line.Start.State.Flags.HasFlag(ls.FixedLength.StartTrigger.Flags)
                && ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Start);

            bool shortenEnd = ls.FixedLength.EndTrigger.Flags != ShapeStateFlags.Default
                && line.End.State.Flags.HasFlag(ls.FixedLength.EndTrigger.Flags)
                && ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.End);

            if (shortenStart && !shortenEnd)
            {
                if (x2 > x1)
                    x1 = x2 - ls.FixedLength.Length;
                else
                    x1 = x2 + ls.FixedLength.Length;
            }

            if (!shortenStart && shortenEnd)
            {
                if (x2 > x1)
                    x2 = x1 + ls.FixedLength.Length;
                else
                    x2 = x1 - ls.FixedLength.Length;
            }
        }

        /// <summary>
        /// Get <see cref="ILineShape"/> maximum length for <see cref="LineFixedLengthFlags.Vertical"/> mode.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="y1">The calculated Y coordinate for <see cref="ILineShape.Start"/> point.</param>
        /// <param name="y2">The calculated Y coordinate for <see cref="ILineShape.End"/> point.</param>
        public static void GetMaxLengthVertical(this ILineShape line, ref double y1, ref double y2)
        {
            var ls = line.Style.LineStyle;

            bool shortenStart = ls.FixedLength.StartTrigger.Flags != ShapeStateFlags.Default
                && line.Start.State.Flags.HasFlag(ls.FixedLength.StartTrigger.Flags)
                && ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Start);

            bool shortenEnd = ls.FixedLength.EndTrigger.Flags != ShapeStateFlags.Default
                && line.End.State.Flags.HasFlag(ls.FixedLength.EndTrigger.Flags)
                && ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.End);

            if (shortenStart && !shortenEnd)
            {
                if (y2 > y1)
                    y1 = y2 - ls.FixedLength.Length;
                else
                    y1 = y2 + ls.FixedLength.Length;
            }

            if (!shortenStart && shortenEnd)
            {
                if (y2 > y1)
                    y2 = y1 + ls.FixedLength.Length;
                else
                    y2 = y1 - ls.FixedLength.Length;
            }
        }

        /// <summary>
        /// Get curved line bezier curve control points.
        /// </summary>
        /// <param name="orientation">The curved line orientation.</param>
        /// <param name="offset">The curved line offset.</param>
        /// <param name="p1a">The line start point alignment.</param>
        /// <param name="p2a">The line end point alignment.</param>
        /// <param name="p1x">The adjusted X coordinate for curve start control point.</param>
        /// <param name="p1y">The adjusted Y coordinate for curve start control point.</param>
        /// <param name="p2x">The adjusted X coordinate for curve end control point.</param>
        /// <param name="p2y">The adjusted Y coordinate for curve end control point.</param>
        public static void GetCurvedLineBezierControlPoints(CurveOrientation orientation, double offset, PointAlignment p1a, PointAlignment p2a, ref double p1x, ref double p1y, ref double p2x, ref double p2y)
        {
            if (orientation == CurveOrientation.Auto)
            {
                switch (p1a)
                {
                    case PointAlignment.None:
                        {
                            switch (p2a)
                            {
                                case PointAlignment.None:
                                    break;
                                case PointAlignment.Left:
                                    p2x -= offset;
                                    p1x += offset;
                                    break;
                                case PointAlignment.Right:
                                    p2x += offset;
                                    p1x -= offset;
                                    break;
                                case PointAlignment.Top:
                                    p2y -= offset;
                                    p1y += offset;
                                    break;
                                case PointAlignment.Bottom:
                                    p2y += offset;
                                    p1y -= offset;
                                    break;
                            }
                        }
                        break;
                    case PointAlignment.Left:
                        {
                            switch (p2a)
                            {
                                case PointAlignment.None:
                                    p1x -= offset;
                                    p2x += offset;
                                    break;
                                case PointAlignment.Left:
                                    p1x -= offset;
                                    p2x -= offset;
                                    break;
                                case PointAlignment.Right:
                                    p1x -= offset;
                                    p2x += offset;
                                    break;
                                case PointAlignment.Top:
                                    p1x -= offset;
                                    break;
                                case PointAlignment.Bottom:
                                    p2y += offset;
                                    break;
                            }
                        }
                        break;
                    case PointAlignment.Right:
                        {
                            switch (p2a)
                            {
                                case PointAlignment.None:
                                    p1x += offset;
                                    p2x -= offset;
                                    break;
                                case PointAlignment.Left:
                                    p1x += offset;
                                    p2x -= offset;
                                    break;
                                case PointAlignment.Right:
                                    p1x += offset;
                                    p2x += offset;
                                    break;
                                case PointAlignment.Top:
                                    p1x += offset;
                                    break;
                                case PointAlignment.Bottom:
                                    p2y += offset;
                                    break;
                            }
                        }
                        break;
                    case PointAlignment.Top:
                        {
                            switch (p2a)
                            {
                                case PointAlignment.None:
                                    p1y -= offset;
                                    p2y += offset;
                                    break;
                                case PointAlignment.Left:
                                    p2x -= offset;
                                    break;
                                case PointAlignment.Right:
                                    p2x += offset;
                                    break;
                                case PointAlignment.Top:
                                    p1y -= offset;
                                    p2y -= offset;
                                    break;
                                case PointAlignment.Bottom:
                                    p1y -= offset;
                                    p2y += offset;
                                    break;
                            }
                        }
                        break;
                    case PointAlignment.Bottom:
                        {
                            switch (p2a)
                            {
                                case PointAlignment.None:
                                    p1y += offset;
                                    p2y -= offset;
                                    break;
                                case PointAlignment.Left:
                                    p1y += offset;
                                    break;
                                case PointAlignment.Right:
                                    p1y += offset;
                                    break;
                                case PointAlignment.Top:
                                    p1y += offset;
                                    p2y -= offset;
                                    break;
                                case PointAlignment.Bottom:
                                    p1y += offset;
                                    p2y += offset;
                                    break;
                            }
                        }
                        break;
                }
            }
            else if (orientation == CurveOrientation.Horizontal)
            {
                p1x += offset;
                p2x -= offset;
            }
            else if (orientation == CurveOrientation.Vertical)
            {
                p1y += offset;
                p2y -= offset;
            }
        }
    }
}
