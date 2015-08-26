// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using N = System.Numerics;
using Test2d;

namespace Test.Uwp
{
    public static class XPathGeometryConverter
    {
        public static CanvasGeometry ToCanvasGeometry(this XPathGeometry pg, CanvasDrawingSession ds)
        {
            CanvasGeometry g;

            using (var builder = new CanvasPathBuilder(ds))
            {
                if (pg.FillRule == XFillRule.EvenOdd)
                    builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Alternate);
                else
                    builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Winding);

                foreach (var pf in pg.Figures)
                {
                    builder.BeginFigure(
                        (float)pf.StartPoint.X,
                        (float)pf.StartPoint.Y,
                        pf.IsFilled ? CanvasFigureFill.Default : CanvasFigureFill.DoesNotAffectFills);

                    foreach (var segment in pf.Segments)
                    {
                        var options = CanvasFigureSegmentOptions.None;
                        if (!segment.IsStroked)
                            options |= CanvasFigureSegmentOptions.ForceUnstroked;

                        if (segment.IsSmoothJoin)
                            options |= CanvasFigureSegmentOptions.ForceRoundLineJoin;

                        builder.SetSegmentOptions(options);

                        if (segment is XArcSegment)
                        {
                            var arcSegment = segment as XArcSegment;
                            builder.AddArc(
                                new N.Vector2(
                                    (float)arcSegment.Point.X,
                                    (float)arcSegment.Point.Y),
                                (float)(arcSegment.Size.Width / 2.0),
                                (float)(arcSegment.Size.Height / 2.0),
                                (float)arcSegment.RotationAngle,
                                arcSegment.SweepDirection == XSweepDirection.Clockwise ? CanvasSweepDirection.Clockwise : CanvasSweepDirection.CounterClockwise,
                                arcSegment.IsLargeArc ? CanvasArcSize.Large : CanvasArcSize.Small);
                        }
                        else if (segment is XBezierSegment)
                        {
                            var bezierSegment = segment as XBezierSegment;
                            builder.AddCubicBezier(
                                new N.Vector2(
                                    (float)bezierSegment.Point1.X,
                                    (float)bezierSegment.Point1.Y),
                                new N.Vector2(
                                    (float)bezierSegment.Point2.X,
                                    (float)bezierSegment.Point2.Y),
                                new N.Vector2(
                                    (float)bezierSegment.Point3.X,
                                    (float)bezierSegment.Point3.Y));
                        }
                        else if (segment is XLineSegment)
                        {
                            var lineSegment = segment as XLineSegment;
                            builder.AddLine(
                                new N.Vector2(
                                    (float)lineSegment.Point.X,
                                    (float)lineSegment.Point.Y));
                        }
                        else if (segment is XPolyBezierSegment)
                        {
                            var polyBezierSegment = segment as XPolyBezierSegment;
                            if (polyBezierSegment.Points.Count >= 3)
                            {
                                builder.AddCubicBezier(
                                    new N.Vector2(
                                        (float)polyBezierSegment.Points[0].X,
                                        (float)polyBezierSegment.Points[0].Y),
                                    new N.Vector2(
                                        (float)polyBezierSegment.Points[1].X,
                                        (float)polyBezierSegment.Points[1].Y),
                                    new N.Vector2(
                                        (float)polyBezierSegment.Points[2].X,
                                        (float)polyBezierSegment.Points[2].Y));
                            }

                            if (polyBezierSegment.Points.Count > 3
                                && polyBezierSegment.Points.Count % 3 == 0)
                            {
                                for (int i = 3; i < polyBezierSegment.Points.Count; i += 3)
                                {
                                    builder.AddCubicBezier(
                                        new N.Vector2(
                                            (float)polyBezierSegment.Points[i].X,
                                            (float)polyBezierSegment.Points[i].Y),
                                        new N.Vector2(
                                            (float)polyBezierSegment.Points[i + 1].X,
                                            (float)polyBezierSegment.Points[i + 1].Y),
                                        new N.Vector2(
                                            (float)polyBezierSegment.Points[i + 2].X,
                                            (float)polyBezierSegment.Points[i + 2].Y));
                                }
                            }
                        }
                        else if (segment is XPolyLineSegment)
                        {
                            var polyLineSegment = segment as XPolyLineSegment;

                            if (polyLineSegment.Points.Count >= 1)
                            {
                                builder.AddLine(
                                    new N.Vector2(
                                        (float)polyLineSegment.Points[0].X,
                                        (float)polyLineSegment.Points[0].Y));
                            }

                            if (polyLineSegment.Points.Count > 1)
                            {
                                for (int i = 1; i < polyLineSegment.Points.Count; i++)
                                {
                                    builder.AddLine(
                                        new N.Vector2(
                                            (float)polyLineSegment.Points[i].X,
                                            (float)polyLineSegment.Points[i].Y));
                                }
                            }
                        }
                        else if (segment is XPolyQuadraticBezierSegment)
                        {
                            var polyQuadraticSegment = segment as XPolyQuadraticBezierSegment;
                            if (polyQuadraticSegment.Points.Count >= 2)
                            {
                                builder.AddQuadraticBezier(
                                    new N.Vector2(
                                        (float)polyQuadraticSegment.Points[0].X,
                                        (float)polyQuadraticSegment.Points[0].Y),
                                    new N.Vector2(
                                        (float)polyQuadraticSegment.Points[1].X,
                                        (float)polyQuadraticSegment.Points[1].Y));
                            }

                            if (polyQuadraticSegment.Points.Count > 2
                                && polyQuadraticSegment.Points.Count % 2 == 0)
                            {
                                for (int i = 3; i < polyQuadraticSegment.Points.Count; i += 3)
                                {
                                    builder.AddQuadraticBezier(
                                        new N.Vector2(
                                            (float)polyQuadraticSegment.Points[i].X,
                                            (float)polyQuadraticSegment.Points[i].Y),
                                        new N.Vector2(
                                            (float)polyQuadraticSegment.Points[i + 1].X,
                                            (float)polyQuadraticSegment.Points[i + 1].Y));
                                }
                            }
                        }
                        else if (segment is XQuadraticBezierSegment)
                        {
                            var qbezierSegment = segment as XQuadraticBezierSegment;
                            builder.AddQuadraticBezier(
                                new N.Vector2(
                                    (float)qbezierSegment.Point1.X,
                                    (float)qbezierSegment.Point1.Y),
                                new N.Vector2(
                                    (float)qbezierSegment.Point2.X,
                                    (float)qbezierSegment.Point2.Y));
                        }
                        else
                        {
                            throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        }
                    }

                    builder.EndFigure(pf.IsClosed ? CanvasFigureLoop.Closed : CanvasFigureLoop.Open);
                }

                g = CanvasGeometry.CreatePath(builder);
            }

            return g;
        }
    }
}
