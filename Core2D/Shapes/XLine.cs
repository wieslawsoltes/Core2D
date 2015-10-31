// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class XLine : BaseShape
    {
        private XPoint _start;
        private XPoint _end;

        /// <summary>
        /// 
        /// </summary>
        public XPoint Start
        {
            get { return _start; }
            set { Update(ref _start, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public XPoint End
        {
            get { return _end; }
            set { Update(ref _end, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            var record = r ?? this.Data.Record;
            _start.TryToBind("Start", this.Data.Bindings, record);
            _end.TryToBind("End", this.Data.Bindings, record);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Data.Record;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
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

        /// <summary>
        /// Creates a new <see cref="XLine"/> instance.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XLine Create(
            XPoint start,
            XPoint end,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            string name = "")
        {
            return new XLine()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<ShapeBinding>(),
                    Properties = ImmutableArray.Create<ShapeProperty>()
                },
                Start = start,
                End = end
            };
        }

        /// <summary>
        /// Creates a new <see cref="XLine"/> instance.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XLine Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            string name = "")
        {
            return new XLine()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = false,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<ShapeBinding>(),
                    Properties = ImmutableArray.Create<ShapeProperty>()
                },
                Start = XPoint.Create(x1, y1, point),
                End = XPoint.Create(x2, y2, point)
            };
        }

        /// <summary>
        /// Creates a new <see cref="XLine"/> instance.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XLine Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            string name = "")
        {
            return Create(x, y, x, y, style, point, isStroked, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public static void SetMaxLength(
            XLine line,
            ref double x1, ref double y1,
            ref double x2, ref double y2)
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
                    bool isVertical = Math.Round(x1, 1) == Math.Round(x2, 1);
                    if (isVertical)
                    {
                        SetMaxLengthVertical(line, ref y1, ref y2);
                    }
                }

                if (ls.FixedLength.Flags.HasFlag(LineFixedLengthFlags.Horizontal))
                {
                    bool isHorizontal = Math.Round(y1, 1) == Math.Round(y2, 1);
                    if (isHorizontal)
                    {
                        SetMaxLengthHorizontal(line, ref x1, ref x2);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public static void SetMaxLengthAll(
            XLine line,
            ref double x1, ref double y1,
            ref double x2, ref double y2)
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
                double distance = Math.Sqrt(dx * dx + dy * dy);
                x1 = x2 - (x2 - x1) / distance * ls.FixedLength.Length;
                y1 = y2 - (y2 - y1) / distance * ls.FixedLength.Length;
            }

            if (!shortenStart && shortenEnd)
            {
                double dx = x2 - x1;
                double dy = y2 - y1;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                x2 = x1 - (x1 - x2) / distance * ls.FixedLength.Length;
                y2 = y1 - (y1 - y2) / distance * ls.FixedLength.Length;
            }

            if (shortenStart && shortenEnd)
            {
                // TODO: Implement shorten start and end case.
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
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
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
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
    }
}
