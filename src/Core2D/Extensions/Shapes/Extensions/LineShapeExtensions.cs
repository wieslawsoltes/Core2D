using Core2D.Renderer;
using Core2D.Style;
using static System.Math;

namespace Core2D.Shapes
{
    public static class LineShapeExtensions
    {
        public static void GetMaxLength(this LineShape line, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            var ls = line.Style.LineStyle;

            if (ls.FixedLength.Flags == LineFixedLengthFlags.Disabled)
            {
                return;
            }

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

        public static void GetMaxLengthAll(this LineShape line, ref double x1, ref double y1, ref double x2, ref double y2)
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

        public static void GetMaxLengthHorizontal(this LineShape line, ref double x1, ref double x2)
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
                {
                    x1 = x2 - ls.FixedLength.Length;
                }
                else
                {
                    x1 = x2 + ls.FixedLength.Length;
                }
            }

            if (!shortenStart && shortenEnd)
            {
                if (x2 > x1)
                {
                    x2 = x1 + ls.FixedLength.Length;
                }
                else
                {
                    x2 = x1 - ls.FixedLength.Length;
                }
            }
        }

        public static void GetMaxLengthVertical(this LineShape line, ref double y1, ref double y2)
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
                {
                    y1 = y2 - ls.FixedLength.Length;
                }
                else
                {
                    y1 = y2 + ls.FixedLength.Length;
                }
            }

            if (!shortenStart && shortenEnd)
            {
                if (y2 > y1)
                {
                    y2 = y1 + ls.FixedLength.Length;
                }
                else
                {
                    y2 = y1 - ls.FixedLength.Length;
                }
            }
        }

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
