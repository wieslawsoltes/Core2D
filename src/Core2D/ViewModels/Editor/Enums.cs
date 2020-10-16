using System;
using Core2D.Containers;
using Core2D.Path;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Editor
{
    public static class Enums
    {
        public static LineCap[] LineCapValues { get; } = (LineCap[])Enum.GetValues(typeof(LineCap));

        public static CurveOrientation[] CurveOrientationValues { get; } = (CurveOrientation[])Enum.GetValues(typeof(CurveOrientation));

        public static ArrowType[] ArrowTypeValues { get; } = (ArrowType[])Enum.GetValues(typeof(ArrowType));

        public static TextHAlignment[] TextHAlignmentValues { get; } = (TextHAlignment[])Enum.GetValues(typeof(TextHAlignment));

        public static TextVAlignment[] TextVAlignmentValues { get; } = (TextVAlignment[])Enum.GetValues(typeof(TextVAlignment));

        public static MoveMode[] MoveModeValues { get; } = (MoveMode[])Enum.GetValues(typeof(MoveMode));

        public static FillRule[] XFillRuleValues { get; } = (FillRule[])Enum.GetValues(typeof(FillRule));

        public static SweepDirection[] XSweepDirectionValues { get; } = (SweepDirection[])Enum.GetValues(typeof(SweepDirection));

        public static PointAlignment[] PointAlignmentValues { get; } = (PointAlignment[])Enum.GetValues(typeof(PointAlignment));
    }
}
