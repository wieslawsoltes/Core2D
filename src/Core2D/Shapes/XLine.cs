// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Style;
using System.Collections.Generic;
using System.Collections.Immutable;
using static System.Math;

namespace Core2D.Shapes
{
    /// <summary>
    /// Line shape.
    /// </summary>
    public class XLine : BaseShape
    {
        private XPoint _start;
        private XPoint _end;

        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        public XPoint Start
        {
            get { return _start; }
            set { Update(ref _start, value); }
        }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint End
        {
            get { return _end; }
            set { Update(ref _end, value); }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var record = this.Data.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy, db, record);
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_start == renderer.State.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_end == renderer.State.SelectedShape)
                {
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _start.Draw(dc, renderer, dx, dy, db, record);
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
            }
        }

        /// <inheritdoc/>
        public override void Move(double dx, double dy)
        {
            if (!Start.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Start.Move(dx, dy);
            }

            if (!End.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                End.Move(dx, dy);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            yield return Start;
            yield return End;
        }

        /// <summary>
        /// Creates a new <see cref="XLine"/> instance.
        /// </summary>
        /// <param name="start">The <see cref="XLine.Start"/> point.</param>
        /// <param name="end">The <see cref="XLine.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XLine"/> class.</returns>
        public static XLine Create(XPoint start, XPoint end, ShapeStyle style, BaseShape point, bool isStroked = true, string name = "")
        {
            return new XLine()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Start = start,
                End = end
            };
        }

        /// <summary>
        /// Creates a new <see cref="XLine"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XLine.Start"/> point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XLine.Start"/> point.</param>
        /// <param name="x2">The X coordinate of <see cref="XLine.End"/> point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XLine.End"/> point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XLine"/> class.</returns>
        public static XLine Create(double x1, double y1, double x2, double y2, ShapeStyle style, BaseShape point, bool isStroked = true, string name = "")
        {
            return new XLine()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Start = XPoint.Create(x1, y1, point),
                End = XPoint.Create(x2, y2, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="XLine"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="XLine.Start"/> and <see cref="XLine.End"/> points.</param>
        /// <param name="y">The Y coordinate of <see cref="XLine.Start"/> and <see cref="XLine.End"/> points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XLine"/> class.</returns>
        public static XLine Create(double x, double y, ShapeStyle style, BaseShape point, bool isStroked = true, string name = "")
        {
            return Create(x, y, x, y, style, point, isStroked, name);
        }

        /// <summary>
        /// Set <see cref="XLine"/> maximum length using <see cref="LineFixedLengthFlags"/>.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="x1">The calculated X coordinate for <see cref="XLine.Start"/> point.</param>
        /// <param name="y1">The calculated Y coordinate for <see cref="XLine.Start"/> point.</param>
        /// <param name="x2">The calculated X coordinate for <see cref="XLine.End"/> point.</param>
        /// <param name="y2">The calculated Y coordinate for <see cref="XLine.End"/> point.</param>
        public static void SetMaxLength(XLine line, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            var ls = line.Style.LineStyle;

            if (ls.FixedLength.Flags == LineFixedLengthFlags.Disabled)
                return;

            if (ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.All))
            {
                SetMaxLengthAll(line, ref x1, ref y1, ref x2, ref y2);
            }
            else
            {
                if (ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Vertical))
                {
                    bool isVertical = Round(x1, 1) == Round(x2, 1);
                    if (isVertical)
                    {
                        SetMaxLengthVertical(line, ref y1, ref y2);
                    }
                }

                if (ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Horizontal))
                {
                    bool isHorizontal = Round(y1, 1) == Round(y2, 1);
                    if (isHorizontal)
                    {
                        SetMaxLengthHorizontal(line, ref x1, ref x2);
                    }
                }
            }
        }

        /// <summary>
        /// Set <see cref="XLine"/> maximum length for <see cref="LineFixedLengthFlags.All"/> mode.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="x1">The calculated X coordinate for <see cref="XLine.Start"/> point.</param>
        /// <param name="y1">The calculated Y coordinate for <see cref="XLine.Start"/> point.</param>
        /// <param name="x2">The calculated X coordinate for <see cref="XLine.End"/> point.</param>
        /// <param name="y2">The calculated Y coordinate for <see cref="XLine.End"/> point.</param>
        public static void SetMaxLengthAll(XLine line, ref double x1, ref double y1, ref double x2, ref double y2)
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

            if (shortenStart && shortenEnd)
            {
                // TODO: Implement shorten start and end case.
            }
        }

        /// <summary>
        /// Set <see cref="XLine"/> maximum length for <see cref="LineFixedLengthFlags.Horizontal"/> mode.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="x1">The calculated X coordinate for <see cref="XLine.Start"/> point.</param>
        /// <param name="x2">The calculated X coordinate for <see cref="XLine.End"/> point.</param>
        public static void SetMaxLengthHorizontal(XLine line, ref double x1, ref double x2)
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

            if (shortenStart && shortenEnd)
            {
                // TODO: Implement shorten start and end case.
            }
        }

        /// <summary>
        /// Set <see cref="XLine"/> maximum length for <see cref="LineFixedLengthFlags.Vertical"/> mode.
        /// </summary>
        /// <param name="line">The line shape.</param>
        /// <param name="y1">The calculated Y coordinate for <see cref="XLine.Start"/> point.</param>
        /// <param name="y2">The calculated Y coordinate for <see cref="XLine.End"/> point.</param>
        public static void SetMaxLengthVertical(XLine line, ref double y1, ref double y2)
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

            if (shortenStart && shortenEnd)
            {
                // TODO: Implement shorten start and end case.
            }
        }

        /// <summary>
        /// Adjust curved line position.
        /// </summary>
        /// <param name="orientation">The curved line orientation.</param>
        /// <param name="offset">The curved line offset.</param>
        /// <param name="p1a">The line start point alignment.</param>
        /// <param name="p2a">The line end point alignment.</param>
        /// <param name="p1x">The adjusted X coordinate for curve start control point.</param>
        /// <param name="p1y">The adjusted Y coordinate for curve start control point.</param>
        /// <param name="p2x">The adjusted X coordinate for curve end control point.</param>
        /// <param name="p2y">The adjusted Y coordinate for curve end control point.</param>
        public static void AdjustLineCurve(CurveOrientation orientation, double offset, PointAlignment p1a, PointAlignment p2a, ref double p1x, ref double p1y, ref double p2x, ref double p2y)
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
