// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using PDF = PdfSharp.Drawing;

namespace Core2D.Modules.Renderer.PdfSharp;

public static class PathGeometryConverter
{
    public static PDF.XGraphicsPath? ToXGraphicsPath(this PathShapeViewModel pathShape, Func<double, double> scale)
    {
        var graphicsPath = new PDF.XGraphicsPath
        {
            FillMode = pathShape.FillRule == FillRule.EvenOdd ? PDF.XFillMode.Alternate : PDF.XFillMode.Winding
        };

        foreach (var figure in pathShape.Figures)
        {
            if (figure.StartPoint is null)
            {
                return null;
            }

            var startPoint = figure.StartPoint;

            foreach (var segment in figure.Segments)
            {
                switch (segment)
                {
                    case LineSegmentViewModel lineSegment:
                    {
                        if (lineSegment.Point is null)
                        {
                            return null;
                        }
                        graphicsPath.AddLine(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(lineSegment.Point.X),
                            scale(lineSegment.Point.Y));
                        startPoint = lineSegment.Point;
                        break;
                    }
                    case CubicBezierSegmentViewModel cubicBezierSegment:
                    {
                        if (cubicBezierSegment.Point1 is null || cubicBezierSegment.Point2 is null || cubicBezierSegment.Point3 is null)
                        {
                            return null;
                        }
                        graphicsPath.AddBezier(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(cubicBezierSegment.Point1.X),
                            scale(cubicBezierSegment.Point1.Y),
                            scale(cubicBezierSegment.Point2.X),
                            scale(cubicBezierSegment.Point2.Y),
                            scale(cubicBezierSegment.Point3.X),
                            scale(cubicBezierSegment.Point3.Y));
                        startPoint = cubicBezierSegment.Point3;
                        break;
                    }
                    case QuadraticBezierSegmentViewModel quadraticBezierSegment:
                    {
                        if (quadraticBezierSegment.Point1 is null || quadraticBezierSegment.Point2 is null)
                        {
                            return null;
                        }
                        var p1 = startPoint;
                        var p2 = quadraticBezierSegment.Point1;
                        var p3 = quadraticBezierSegment.Point2;
                        var x1 = p1.X;
                        var y1 = p1.Y;
                        var x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                        var y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                        var x3 = x2 + (p3.X - p1.X) / 3.0;
                        var y3 = y2 + (p3.Y - p1.Y) / 3.0;
                        var x4 = p3.X;
                        var y4 = p3.Y;
                        graphicsPath.AddBezier(
                            scale(x1),
                            scale(y1),
                            scale(x2),
                            scale(y2),
                            scale(x3),
                            scale(y3),
                            scale(x4),
                            scale(y4));
                        startPoint = quadraticBezierSegment.Point2;
                        break;
                    }
                    case ArcSegmentViewModel arcSegment:
                    {
                        if (arcSegment.Point is null || arcSegment.Size is null)
                        {
                            return null;
                        }
                        var point1 = new PDF.XPoint(
                            scale(startPoint.X),
                            scale(startPoint.Y));
                        var point2 = new PDF.XPoint(
                            scale(arcSegment.Point.X),
                            scale(arcSegment.Point.Y));
                        var size = new PDF.XSize(
                            scale(arcSegment.Size.Width),
                            scale(arcSegment.Size.Height));
                        graphicsPath.AddArc(
                            point1,
                            point2,
                            size, arcSegment.RotationAngle, arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? PDF.XSweepDirection.Clockwise : PDF.XSweepDirection.Counterclockwise);
                        startPoint = arcSegment.Point;
                        break;
                    }
                    default:
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                }
            }

            if (figure.IsClosed)
            {
                graphicsPath.CloseFigure();
            }
            else
            {
                graphicsPath.StartFigure();
            }
        }

        return graphicsPath;
    }
}
