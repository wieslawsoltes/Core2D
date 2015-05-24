// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
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
            set
            {
                if (value != _start)
                {
                    _start = value;
                    Notify("Start");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XPoint End
        {
            get { return _end; }
            set
            {
                if (value != _end)
                {
                    _end = value;
                    Notify("End");
                }
            }
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
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, IList<ShapeProperty> db, DataRecord r)
        {
            var record = r != null ? r : this.Record;

            if (State.HasFlag(ShapeState.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record); 
            }

            if (renderer.SelectedShape != null)
            {
                if (this == renderer.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy, db, record);
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_start == renderer.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_end == renderer.SelectedShape)
                {
                    _end.Draw(dc, renderer, dx, dy, db, record);
                }
            }
            
            if (renderer.SelectedShapes != null)
            {
                if (renderer.SelectedShapes.Contains(this))
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
            Start.Move(dx, dy);
            End.Move(dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XLine Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            string name = "")
        {
            return new XLine()
            {
                Name = name,
                Style = style,
                Properties = new ObservableCollection<ShapeProperty>(),
                Start = XPoint.Create(x1, y1, point),
                End = XPoint.Create(x2, y2, point)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XLine Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            string name = "")
        {
            return Create(x, y, x, y, style, point, name);
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

            if (ls.MaxLengthFlags == MaxLengthFlags.Disabled)
                return;

            if (ls.MaxLengthFlags.HasFlag(MaxLengthFlags.All))
            {
                SetMaxLengthAll(line, ref x1, ref y1, ref x2, ref y2);
            }
            else
            {
                if (ls.MaxLengthFlags.HasFlag(MaxLengthFlags.Vertical))
                {
                    bool isVertical = Math.Round(x1, 1) == Math.Round(x2, 1);
                    if (isVertical)
                    {
                        SetMaxLengthVertical(line, ref y1, ref y2);
                    }
                }

                if (ls.MaxLengthFlags.HasFlag(MaxLengthFlags.Horizontal))
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

            bool shortenStart = ls.MaxLengthStartState != ShapeState.Default
                && line.Start.State.HasFlag(ls.MaxLengthStartState)
                && ls.MaxLengthFlags.HasFlag(MaxLengthFlags.Start);

            bool shortenEnd = ls.MaxLengthEndState != ShapeState.Default
                && line.End.State.HasFlag(ls.MaxLengthEndState)
                && ls.MaxLengthFlags.HasFlag(MaxLengthFlags.End);

            if (shortenStart && !shortenEnd)
            {
                var p1 = Point2.Create(x1, y1);
                var p2 = Point2.Create(x2, y2);
                double length = p1.Distance(p2);
                x1 = p2.X - (p2.X - p1.X) / length * ls.MaxLength;
                y1 = p2.Y - (p2.Y - p1.Y) / length * ls.MaxLength;
            }

            if (!shortenStart && shortenEnd)
            {
                var p1 = Point2.Create(x2, y2);
                var p2 = Point2.Create(x1, y1);
                double length = p1.Distance(p2);
                x2 = p2.X - (p2.X - p1.X) / length * ls.MaxLength;
                y2 = p2.Y - (p2.Y - p1.Y) / length * ls.MaxLength;
            }

            if (shortenStart && shortenEnd)
            {
                // TODO:
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

            bool shortenStart = ls.MaxLengthStartState != ShapeState.Default
                && line.Start.State.HasFlag(ls.MaxLengthStartState)
                && ls.MaxLengthFlags.HasFlag(MaxLengthFlags.Start);

            bool shortenEnd = ls.MaxLengthEndState != ShapeState.Default
                && line.End.State.HasFlag(ls.MaxLengthEndState)
                && ls.MaxLengthFlags.HasFlag(MaxLengthFlags.End);

            if (shortenStart && !shortenEnd)
            {
                if (x2 > x1)
                    x1 = x2 - ls.MaxLength;
                else
                    x1 = x2 + ls.MaxLength;
            }

            if (!shortenStart && shortenEnd)
            {
                if (x2 > x1)
                    x2 = x1 + ls.MaxLength;
                else
                    x2 = x1 - ls.MaxLength;
            }

            if (shortenStart && shortenEnd)
            {
                // TODO:
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

            bool shortenStart = ls.MaxLengthStartState != ShapeState.Default
                && line.Start.State.HasFlag(ls.MaxLengthStartState)
                && ls.MaxLengthFlags.HasFlag(MaxLengthFlags.Start);

            bool shortenEnd = ls.MaxLengthEndState != ShapeState.Default
                && line.End.State.HasFlag(ls.MaxLengthEndState)
                && ls.MaxLengthFlags.HasFlag(MaxLengthFlags.End);

            if (shortenStart && !shortenEnd)
            {
                if (y2 > y1)
                    y1 = y2 - ls.MaxLength;
                else
                    y1 = y2 + ls.MaxLength;
            }

            if (!shortenStart && shortenEnd)
            {
                if (y2 > y1)
                    y2 = y1 + ls.MaxLength;
                else
                    y2 = y1 - ls.MaxLength;
            }

            /*
            if (shortenStart && shortenEnd)
            {
                if (y2 > y1)
                    y2 = y1 + ls.MaxLength;
                else
                    y2 = y1 - ls.MaxLength;
            }
            */
        }
    }
}
