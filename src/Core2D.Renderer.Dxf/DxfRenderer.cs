// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Core2D.Style;
using DXF = netDxf;
using DXFE = netDxf.Entities;
using DXFO = netDxf.Objects;
using DXFT = netDxf.Tables;

namespace Core2D.Renderer.Dxf
{
    /// <summary>
    /// Native netDxf shape renderer.
    /// </summary>
    public partial class DxfRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private ICache<string, DXFO.ImageDefinition> _biCache;
        private double _pageWidth;
        private double _pageHeight;
        private string _outputPath;
        private DXFT.Layer _currentLayer;
        private double _sourceDpi = 96.0;
        private double _targetDpi = 72.0;

        /// <inheritdoc/>
        public IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DxfRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DxfRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, DXFO.ImageDefinition>();
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="DxfRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="DxfRenderer"/> class.</returns>
        public static IShapeRenderer Create() => new DxfRenderer();

        private static double s_lineweightFactor = 96.0 / 2540.0;

        private static short[] s_lineweights = { -3, -2, -1, 0, 5, 9, 13, 15, 18, 20, 25, 30, 35, 40, 50, 53, 60, 70, 80, 90, 100, 106, 120, 140, 158, 200, 211 };

        private static DXF.Lineweight ToLineweight(double thickness)
        {
            short lineweight = (short)(thickness / s_lineweightFactor);
            return (DXF.Lineweight)s_lineweights.OrderBy(x => Math.Abs((long)x - lineweight)).First();
        }

        private static DXF.AciColor ToColor(IColor color)
        {
            switch (color)
            {
                case IArgbColor argbColor:
                    return new DXF.AciColor(argbColor.R, argbColor.G, argbColor.B);
                default:
                    throw new NotSupportedException($"The {color.GetType()} color type is not supported.");
            }
        }

        private static short ToTransparency(IColor color)
        {
            switch (color)
            {
                case IArgbColor argbColor:
                    return (short)(90.0 - argbColor.A * 90.0 / 255.0);;
                default:
                    throw new NotSupportedException($"The {color.GetType()} color type is not supported.");
            }
        }

        private double ToDxfX(double x) => x;

        private double ToDxfY(double y) => _pageHeight - y;

        private DXFE.Line CreateLine(double x1, double y1, double x2, double y2)
        {
            double _x1 = ToDxfX(x1);
            double _y1 = ToDxfY(y1);
            double _x2 = ToDxfX(x2);
            double _y2 = ToDxfY(y2);
            return new DXFE.Line(new DXF.Vector3(_x1, _y1, 0), new DXF.Vector3(_x2, _y2, 0));
        }

        private DXFE.Ellipse CreateEllipse(double x, double y, double width, double height)
        {
            double _cx = ToDxfX(x + width / 2.0);
            double _cy = ToDxfY(y + height / 2.0);
            double minor = Math.Min(height, width);
            double major = Math.Max(height, width);

            return new DXFE.Ellipse()
            {
                Center = new DXF.Vector3(_cx, _cy, 0),
                MajorAxis = major,
                MinorAxis = minor,
                StartAngle = 0.0,
                EndAngle = 360.0,
                Rotation = height > width ? 90.0 : 0.0
            };
        }

        private DXFE.Ellipse CreateEllipticalArc(IArcShape arc, double dx, double dy)
        {
            var a = new Spatial.Arc.GdiArc(
                Spatial.Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Spatial.Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Spatial.Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Spatial.Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            double _cx = ToDxfX(a.X + dx + a.Width / 2.0);
            double _cy = ToDxfY(a.Y + dy + a.Height / 2.0);
            double minor = Math.Min(a.Height, a.Width);
            double major = Math.Max(a.Height, a.Width);
            double startAngle = -a.EndAngle;
            double endAngle = -a.StartAngle;
            double rotation = 0;

            if (a.Height > a.Width)
            {
                startAngle += 90;
                endAngle += 90;
                rotation = -90;
            }

            return new DXFE.Ellipse()
            {
                Center = new DXF.Vector3(_cx, _cy, 0),
                MajorAxis = major,
                MinorAxis = minor,
                StartAngle = startAngle,
                EndAngle = endAngle,
                Rotation = rotation
            };
        }

        private DXFE.Spline CreateQuadraticSpline(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y)
        {
            double _p1x = ToDxfX(p1x);
            double _p1y = ToDxfY(p1y);
            double _p2x = ToDxfX(p2x);
            double _p2y = ToDxfY(p2y);
            double _p3x = ToDxfX(p3x);
            double _p3y = ToDxfY(p3y);

            return new DXFE.Spline(
                new List<DXFE.SplineVertex>
                {
                    new DXFE.SplineVertex(_p1x, _p1y, 0.0),
                    new DXFE.SplineVertex(_p2x, _p2y, 0.0),
                    new DXFE.SplineVertex(_p3x, _p3y, 0.0)
                }, 2);
        }

        private DXFE.Spline CreateCubicSpline(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y, double p4x, double p4y)
        {
            double _p1x = ToDxfX(p1x);
            double _p1y = ToDxfY(p1y);
            double _p2x = ToDxfX(p2x);
            double _p2y = ToDxfY(p2y);
            double _p3x = ToDxfX(p3x);
            double _p3y = ToDxfY(p3y);
            double _p4x = ToDxfX(p4x);
            double _p4y = ToDxfY(p4y);

            return new DXFE.Spline(
                new List<DXFE.SplineVertex>
                {
                    new DXFE.SplineVertex(_p1x, _p1y, 0.0),
                    new DXFE.SplineVertex(_p2x, _p2y, 0.0),
                    new DXFE.SplineVertex(_p3x, _p3y, 0.0),
                    new DXFE.SplineVertex(_p4x, _p4y, 0.0)
                }, 3);
        }

        private void DrawLineInternal(DXF.DxfDocument dxf, DXFT.Layer layer, IBaseStyle style, bool isStroked, double x1, double y1, double x2, double y2)
        {
            if (isStroked)
            {
                var stroke = ToColor(style.Stroke);
                var strokeTansparency = ToTransparency(style.Stroke);
                var lineweight = ToLineweight(style.Thickness);

                var dxfLine = CreateLine(x1, y1, x2, y2);

                dxfLine.Layer = layer;
                dxfLine.Color = stroke;
                dxfLine.Transparency.Value = strokeTansparency;
                dxfLine.Lineweight = lineweight;

                dxf.AddEntity(dxfLine);
            }
        }

        private void DrawRectangleInternal(DXF.DxfDocument dxf, DXFT.Layer layer, bool isFilled, bool isStroked, IBaseStyle style, ref Spatial.Rect2 rect)
        {
            if (isFilled)
            {
                FillRectangle(dxf, layer, rect.X, rect.Y, rect.Width, rect.Height, style.Fill);
            }

            if (isStroked)
            {
                StrokeRectangle(dxf, layer, style, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        private void FillRectangle(DXF.DxfDocument dxf, DXFT.Layer layer, double x, double y, double width, double height, IColor color)
        {
            var fill = ToColor(color);
            var fillTransparency = ToTransparency(color);

            var bounds =
                new List<DXFE.HatchBoundaryPath>
                {
                        new DXFE.HatchBoundaryPath(
                            new List<DXFE.EntityObject>
                            {
                                CreateLine(x, y, x + width, y),
                                CreateLine(x + width, y, x + width, y + height),
                                CreateLine(x + width, y + height, x, y + height),
                                CreateLine(x, y + height, x, y)
                            })
                };

            var hatch = new DXFE.Hatch(DXFE.HatchPattern.Solid, bounds, false);
            hatch.Layer = layer;
            hatch.Color = fill;
            hatch.Transparency.Value = fillTransparency;

            dxf.AddEntity(hatch);
        }

        private void StrokeRectangle(DXF.DxfDocument dxf, DXFT.Layer layer, IBaseStyle style, double x, double y, double width, double height)
        {
            DrawLineInternal(dxf, layer, style, true, x, y, x + width, y);
            DrawLineInternal(dxf, layer, style, true, x + width, y, x + width, y + height);
            DrawLineInternal(dxf, layer, style, true, x + width, y + height, x, y + height);
            DrawLineInternal(dxf, layer, style, true, x, y + height, x, y);
        }

        private void DrawEllipseInternal(DXF.DxfDocument dxf, DXFT.Layer layer, bool isFilled, bool isStroked, IBaseStyle style, ref Spatial.Rect2 rect)
        {
            var dxfEllipse = CreateEllipse(rect.X, rect.Y, rect.Width, rect.Height);

            if (isFilled)
            {
                FillEllipse(dxf, layer, dxfEllipse, style.Fill);
            }

            if (isStroked)
            {
                StrokeEllipse(dxf, layer, dxfEllipse, style.Stroke, style.Thickness);
            }
        }

        private void StrokeEllipse(DXF.DxfDocument dxf, DXFT.Layer layer, DXFE.Ellipse dxfEllipse, IColor color, double thickness)
        {
            var stroke = ToColor(color);
            var strokeTansparency = ToTransparency(color);
            var lineweight = ToLineweight(thickness);

            dxfEllipse.Layer = layer;
            dxfEllipse.Color = stroke;
            dxfEllipse.Transparency.Value = strokeTansparency;
            dxfEllipse.Lineweight = lineweight;

            dxf.AddEntity(dxfEllipse);
        }

        private void FillEllipse(DXF.DxfDocument dxf, DXFT.Layer layer, DXFE.Ellipse dxfEllipse, IColor color)
        {
            var fill = ToColor(color);
            var fillTransparency = ToTransparency(color);

            // TODO: The netDxf does not create hatch for Ellipse with end angle equal to 360.
            var bounds =
                new List<DXFE.HatchBoundaryPath>
                {
                        new DXFE.HatchBoundaryPath(
                            new List<DXFE.EntityObject>
                            {
                                (DXFE.Ellipse)dxfEllipse.Clone()
                            })
                };

            var hatch = new DXFE.Hatch(DXFE.HatchPattern.Solid, bounds, false);
            hatch.Layer = layer;
            hatch.Color = fill;
            hatch.Transparency.Value = fillTransparency;

            dxf.AddEntity(hatch);
        }

        private void DrawGridInternal(DXF.DxfDocument dxf, DXFT.Layer layer, IShapeStyle style, double offsetX, double offsetY, double cellWidth, double cellHeight, ref Spatial.Rect2 rect)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double gx = sx; gx < ex; gx += cellWidth)
            {
                DrawLineInternal(dxf, layer, style, true, gx, oy, gx, ey);
            }

            for (double gy = sy; gy < ey; gy += cellHeight)
            {
                DrawLineInternal(dxf, layer, style, true, ox, gy, ex, gy);
            }
        }

        private void CreateHatchBoundsAndEntitiess(IPathGeometry pg, double dx, double dy, out IList<DXFE.HatchBoundaryPath> bounds, out ICollection<DXFE.EntityObject> entities)
        {
            bounds = new List<DXFE.HatchBoundaryPath>();
            entities = new List<DXFE.EntityObject>();

            // TODO: FillMode = pg.FillRule == FillRule.EvenOdd ? FillMode.Alternate : FillMode.Winding;

            foreach (var pf in pg.Figures)
            {
                var edges = new List<DXFE.EntityObject>();
                var startPoint = pf.StartPoint;

                foreach (var segment in pf.Segments)
                {
                    if (segment is ArcSegment arcSegment)
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                        // TODO: Convert WPF/SVG elliptical arc segment format to DXF ellipse arc.
                        //startPoint = arcSegment.Point;
                    }
                    else if (segment is CubicBezierSegment cubicBezierSegment)
                    {
                        var dxfSpline = CreateCubicSpline(
                            startPoint.X + dx,
                            startPoint.Y + dy,
                            cubicBezierSegment.Point1.X + dx,
                            cubicBezierSegment.Point1.Y + dy,
                            cubicBezierSegment.Point2.X + dx,
                            cubicBezierSegment.Point2.Y + dy,
                            cubicBezierSegment.Point3.X + dx,
                            cubicBezierSegment.Point3.Y + dy);
                        edges.Add(dxfSpline);
                        entities.Add((DXFE.Spline)dxfSpline.Clone());
                        startPoint = cubicBezierSegment.Point3;
                    }
                    else if (segment is LineSegment lineSegment)
                    {
                        var dxfLine = CreateLine(
                            startPoint.X + dx,
                            startPoint.Y + dy,
                            lineSegment.Point.X + dx,
                            lineSegment.Point.Y + dy);
                        edges.Add(dxfLine);
                        entities.Add((DXFE.Line)dxfLine.Clone());
                        startPoint = lineSegment.Point;
                    }
                    else if (segment is PolyCubicBezierSegment polyCubicBezierSegment)
                    {
                        if (polyCubicBezierSegment.Points.Length >= 3)
                        {
                            var dxfSpline = CreateCubicSpline(
                                startPoint.X + dx,
                                startPoint.Y + dy,
                                polyCubicBezierSegment.Points[0].X + dx,
                                polyCubicBezierSegment.Points[0].Y + dy,
                                polyCubicBezierSegment.Points[1].X + dx,
                                polyCubicBezierSegment.Points[1].Y + dy,
                                polyCubicBezierSegment.Points[2].X + dx,
                                polyCubicBezierSegment.Points[2].Y + dy);
                            edges.Add(dxfSpline);
                            entities.Add((DXFE.Spline)dxfSpline.Clone());
                        }

                        if (polyCubicBezierSegment.Points.Length > 3
                            && polyCubicBezierSegment.Points.Length % 3 == 0)
                        {
                            for (int i = 3; i < polyCubicBezierSegment.Points.Length; i += 3)
                            {
                                var dxfSpline = CreateCubicSpline(
                                    polyCubicBezierSegment.Points[i - 1].X + dx,
                                    polyCubicBezierSegment.Points[i - 1].Y + dy,
                                    polyCubicBezierSegment.Points[i].X + dx,
                                    polyCubicBezierSegment.Points[i].Y + dy,
                                    polyCubicBezierSegment.Points[i + 1].X + dx,
                                    polyCubicBezierSegment.Points[i + 1].Y + dy,
                                    polyCubicBezierSegment.Points[i + 2].X + dx,
                                    polyCubicBezierSegment.Points[i + 2].Y + dy);
                                edges.Add(dxfSpline);
                                entities.Add((DXFE.Spline)dxfSpline.Clone());
                            }
                        }

                        startPoint = polyCubicBezierSegment.Points.Last();
                    }
                    else if (segment is PolyLineSegment polyLineSegment)
                    {
                        if (polyLineSegment.Points.Length >= 1)
                        {
                            var dxfLine = CreateLine(
                                startPoint.X + dx,
                                startPoint.Y + dy,
                                polyLineSegment.Points[0].X + dx,
                                polyLineSegment.Points[0].Y + dy);
                            edges.Add(dxfLine);
                            entities.Add((DXFE.Line)dxfLine.Clone());
                        }

                        if (polyLineSegment.Points.Length > 1)
                        {
                            for (int i = 1; i < polyLineSegment.Points.Length; i++)
                            {
                                var dxfLine = CreateLine(
                                    polyLineSegment.Points[i - 1].X + dx,
                                    polyLineSegment.Points[i - 1].Y + dy,
                                    polyLineSegment.Points[i].X + dx,
                                    polyLineSegment.Points[i].Y + dy);
                                edges.Add(dxfLine);
                                entities.Add((DXFE.Line)dxfLine.Clone());
                            }
                        }

                        startPoint = polyLineSegment.Points.Last();
                    }
                    else if (segment is PolyQuadraticBezierSegment polyQuadraticSegment)
                    {
                        if (polyQuadraticSegment.Points.Length >= 2)
                        {
                            var dxfSpline = CreateQuadraticSpline(
                                startPoint.X + dx,
                                startPoint.Y + dy,
                                polyQuadraticSegment.Points[0].X + dx,
                                polyQuadraticSegment.Points[0].Y + dy,
                                polyQuadraticSegment.Points[1].X + dx,
                                polyQuadraticSegment.Points[1].Y + dy);
                            edges.Add(dxfSpline);
                            entities.Add((DXFE.Spline)dxfSpline.Clone());
                        }

                        if (polyQuadraticSegment.Points.Length > 2
                            && polyQuadraticSegment.Points.Length % 2 == 0)
                        {
                            for (int i = 3; i < polyQuadraticSegment.Points.Length; i += 3)
                            {
                                var dxfSpline = CreateQuadraticSpline(
                                    polyQuadraticSegment.Points[i - 1].X + dx,
                                    polyQuadraticSegment.Points[i - 1].Y + dy,
                                    polyQuadraticSegment.Points[i].X + dx,
                                    polyQuadraticSegment.Points[i].Y + dy,
                                    polyQuadraticSegment.Points[i + 1].X + dx,
                                    polyQuadraticSegment.Points[i + 1].Y + dy);
                                edges.Add(dxfSpline);
                                entities.Add((DXFE.Spline)dxfSpline.Clone());
                            }
                        }

                        startPoint = polyQuadraticSegment.Points.Last();
                    }
                    else if (segment is QuadraticBezierSegment quadraticBezierSegment)
                    {
                        var dxfSpline = CreateQuadraticSpline(
                            startPoint.X + dx,
                            startPoint.Y + dy,
                            quadraticBezierSegment.Point1.X + dx,
                            quadraticBezierSegment.Point1.Y + dy,
                            quadraticBezierSegment.Point2.X + dx,
                            quadraticBezierSegment.Point2.Y + dy);
                        edges.Add(dxfSpline);
                        entities.Add((DXFE.Spline)dxfSpline.Clone());
                        startPoint = quadraticBezierSegment.Point2;
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                // TODO: Add support for pf.IsClosed

                var path = new DXFE.HatchBoundaryPath(edges);
                bounds.Add(path);
            }
        }

        /// <inheritdoc/>
        public void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                _biCache.Reset();
            }
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var dxf = dc as DXF.DxfDocument;
            var rect = Spatial.Rect2.FromPoints(x, y, x + width, y + height);
            FillRectangle(dxf, _currentLayer, x, y, width, height, color);
        }

        /// <inheritdoc/>
        public object PushMatrix(object dc, IMatrixObject matrix)
        {
            // TODO: Implement push matrix.
            return null;
        }

        /// <inheritdoc/>
        public void PopMatrix(object dc, object state)
        {
            // TODO: Implement pop matrix.
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPageContainer container, double dx, double dy, object db, object r)
        {
            var dxf = dc as DXF.DxfDocument;

            foreach (var layer in container.Layers)
            {
                var dxfLayer = new DXFT.Layer(layer.Name)
                {
                    IsVisible = layer.IsVisible
                };

                dxf.Layers.Add(dxfLayer);

                _currentLayer = dxfLayer;

                Draw(dc, layer, dx, dy, db, r);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILayerContainer layer, double dx, double dy, object db, object r)
        {
            var dxf = dc as DXF.DxfDocument;

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(State.DrawShapeState.Flags))
                {
                    shape.Draw(dxf, this, dx, dy, db, r);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ILineShape line, double dx, double dy, object db, object r)
        {
            if (!line.IsStroked)
                return;

            var dxf = dc as DXF.DxfDocument;

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            LineShapeExtensions.GetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            // TODO: Draw line start arrow.

            // TODO: Draw line end arrow.

            // TODO: Draw line curve.

            DrawLineInternal(dxf, _currentLayer, line.Style, line.IsStroked, _x1, _y1, _x2, _y2);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IRectangleShape rectangle, double dx, double dy, object db, object r)
        {
            if (!rectangle.IsStroked && !rectangle.IsFilled && !rectangle.IsGrid)
                return;

            var dxf = dc as DXF.DxfDocument;
            var style = rectangle.Style;
            var rect = Spatial.Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                dx, dy);

            DrawRectangleInternal(dxf, _currentLayer, rectangle.IsFilled, rectangle.IsStroked, style, ref rect);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    dxf,
                    _currentLayer,
                    style,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    ref rect);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IEllipseShape ellipse, double dx, double dy, object db, object r)
        {
            if (!ellipse.IsStroked && !ellipse.IsFilled)
                return;

            var dxf = dc as DXF.DxfDocument;
            var style = ellipse.Style;
            var rect = Spatial.Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                dx, dy);

            DrawEllipseInternal(dxf, _currentLayer, ellipse.IsFilled, ellipse.IsStroked, style, ref rect);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IArcShape arc, double dx, double dy, object db, object r)
        {
            var dxf = dc as DXF.DxfDocument;
            var style = arc.Style;

            var dxfEllipse = CreateEllipticalArc(arc, dx, dy);

            if (arc.IsFilled)
            {
                var fill = ToColor(style.Fill);
                var fillTransparency = ToTransparency(style.Fill);

                // TODO: The netDxf does not create hatch for Ellipse with end angle equal to 360.
                var bounds =
                    new List<DXFE.HatchBoundaryPath>
                    {
                        new DXFE.HatchBoundaryPath(
                            new List<DXFE.EntityObject>
                            {
                                (DXFE.Ellipse)dxfEllipse.Clone()
                            })
                    };

                var hatch = new DXFE.Hatch(DXFE.HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                dxf.AddEntity(hatch);
            }

            if (arc.IsStroked)
            {
                var stroke = ToColor(style.Stroke);
                var strokeTansparency = ToTransparency(style.Stroke);
                var lineweight = ToLineweight(style.Thickness);

                dxfEllipse.Layer = _currentLayer;
                dxfEllipse.Color = stroke;
                dxfEllipse.Transparency.Value = strokeTansparency;
                dxfEllipse.Lineweight = lineweight;

                dxf.AddEntity(dxfEllipse);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy, object db, object r)
        {
            if (!cubicBezier.IsStroked && !cubicBezier.IsFilled)
                return;

            var dxf = dc as DXF.DxfDocument;
            var style = cubicBezier.Style;

            var dxfSpline = CreateCubicSpline(
                cubicBezier.Point1.X + dx,
                cubicBezier.Point1.Y + dy,
                cubicBezier.Point2.X + dx,
                cubicBezier.Point2.Y + dy,
                cubicBezier.Point3.X + dx,
                cubicBezier.Point3.Y + dy,
                cubicBezier.Point4.X + dx,
                cubicBezier.Point4.Y + dy);

            if (cubicBezier.IsFilled)
            {
                var fill = ToColor(style.Fill);
                var fillTransparency = ToTransparency(style.Fill);

                var bounds =
                    new List<DXFE.HatchBoundaryPath>
                    {
                        new DXFE.HatchBoundaryPath(
                            new List<DXFE.EntityObject>
                            {
                                (DXFE.Spline)dxfSpline.Clone()
                            })
                    };

                var hatch = new DXFE.Hatch(DXFE.HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                dxf.AddEntity(hatch);
            }

            if (cubicBezier.IsStroked)
            {
                var stroke = ToColor(style.Stroke);
                var strokeTansparency = ToTransparency(style.Stroke);
                var lineweight = ToLineweight(style.Thickness);

                dxfSpline.Layer = _currentLayer;
                dxfSpline.Color = stroke;
                dxfSpline.Transparency.Value = strokeTansparency;
                dxfSpline.Lineweight = lineweight;

                dxf.AddEntity(dxfSpline);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy, object db, object r)
        {
            if (!quadraticBezier.IsStroked && !quadraticBezier.IsFilled)
                return;

            var dxf = dc as DXF.DxfDocument;
            var style = quadraticBezier.Style;

            var dxfSpline = CreateQuadraticSpline(
                quadraticBezier.Point1.X + dx,
                quadraticBezier.Point1.Y + dy,
                quadraticBezier.Point2.X + dx,
                quadraticBezier.Point2.Y + dy,
                quadraticBezier.Point3.X + dx,
                quadraticBezier.Point3.Y + dy);

            if (quadraticBezier.IsFilled)
            {
                var fill = ToColor(style.Fill);
                var fillTransparency = ToTransparency(style.Fill);

                var bounds =
                    new List<DXFE.HatchBoundaryPath>
                    {
                        new DXFE.HatchBoundaryPath(
                            new List<DXFE.EntityObject>
                            {
                                (DXFE.Spline)dxfSpline.Clone()
                            })
                    };

                var hatch = new DXFE.Hatch(DXFE.HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                dxf.AddEntity(hatch);
            }

            if (quadraticBezier.IsStroked)
            {
                var stroke = ToColor(style.Stroke);
                var strokeTansparency = ToTransparency(style.Stroke);
                var lineweight = ToLineweight(style.Thickness);

                dxfSpline.Layer = _currentLayer;
                dxfSpline.Color = stroke;
                dxfSpline.Transparency.Value = strokeTansparency;
                dxfSpline.Lineweight = lineweight;

                dxf.AddEntity(dxfSpline);
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, ITextShape text, double dx, double dy, object db, object r)
        {
            var dxf = dc as DXF.DxfDocument;

            var properties = (ImmutableArray<IProperty>)db;
            var record = (IRecord)r;
            var tbind = text.BindText(properties, record);
            if (string.IsNullOrEmpty(tbind))
                return;

            var style = text.Style;
            var stroke = ToColor(style.Stroke);
            var strokeTansparency = ToTransparency(style.Stroke);

            var attachmentPoint = default(DXFE.MTextAttachmentPoint);
            double x, y;
            var rect = Spatial.Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y,
                dx, dy);

            switch (text.Style.TextStyle.TextHAlignment)
            {
                default:
                case TextHAlignment.Left:
                    x = rect.X;
                    break;
                case TextHAlignment.Center:
                    x = rect.X + rect.Width / 2.0;
                    break;
                case TextHAlignment.Right:
                    x = rect.X + rect.Width;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                default:
                case TextVAlignment.Top:
                    y = rect.Y;
                    break;
                case TextVAlignment.Center:
                    y = rect.Y + rect.Height / 2.0;
                    break;
                case TextVAlignment.Bottom:
                    y = rect.Y + rect.Height;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                default:
                case TextVAlignment.Top:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case TextHAlignment.Left:
                            attachmentPoint = DXFE.MTextAttachmentPoint.TopLeft;
                            break;
                        case TextHAlignment.Center:
                            attachmentPoint = DXFE.MTextAttachmentPoint.TopCenter;
                            break;
                        case TextHAlignment.Right:
                            attachmentPoint = DXFE.MTextAttachmentPoint.TopRight;
                            break;
                    }
                    break;
                case TextVAlignment.Center:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case TextHAlignment.Left:
                            attachmentPoint = DXFE.MTextAttachmentPoint.MiddleLeft;
                            break;
                        case TextHAlignment.Center:
                            attachmentPoint = DXFE.MTextAttachmentPoint.MiddleCenter;
                            break;
                        case TextHAlignment.Right:
                            attachmentPoint = DXFE.MTextAttachmentPoint.MiddleRight;
                            break;
                    }
                    break;
                case TextVAlignment.Bottom:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case TextHAlignment.Left:
                            attachmentPoint = DXFE.MTextAttachmentPoint.BottomLeft;
                            break;
                        case TextHAlignment.Center:
                            attachmentPoint = DXFE.MTextAttachmentPoint.BottomCenter;
                            break;
                        case TextHAlignment.Right:
                            attachmentPoint = DXFE.MTextAttachmentPoint.BottomRight;
                            break;
                    }
                    break;
            }

            var ts = new netDxf.Tables.TextStyle(style.TextStyle.FontName, style.TextStyle.FontFile);
            var dxfMText = new DXFE.MText(
                new DXF.Vector3(ToDxfX(x), ToDxfY(y), 0),
                text.Style.TextStyle.FontSize * _targetDpi / _sourceDpi,
                rect.Width,
                ts);
            dxfMText.AttachmentPoint = attachmentPoint;

            var options = new DXFE.MTextFormattingOptions(dxfMText.Style);
            var fs = text.Style.TextStyle.FontStyle;
            if (fs != null)
            {
                options.Bold = fs.Flags.HasFlag(FontStyleFlags.Bold);
                options.Italic = fs.Flags.HasFlag(FontStyleFlags.Italic);
                options.Underline = fs.Flags.HasFlag(FontStyleFlags.Underline);
                options.StrikeThrough = fs.Flags.HasFlag(FontStyleFlags.Strikeout);
            }

            options.Aligment = DXFE.MTextFormattingOptions.TextAligment.Default;
            options.Color = null;
            dxfMText.Write(tbind, options);

            dxfMText.Layer = _currentLayer;
            dxfMText.Transparency.Value = strokeTansparency;
            dxfMText.Color = stroke;

            dxf.AddEntity(dxfMText);
        }

        /// <inheritdoc/>
        public void Draw(object dc, IImageShape image, double dx, double dy, object db, object r)
        {
            var dxf = dc as DXF.DxfDocument;

            var bytes = State.ImageCache.GetImage(image.Key);
            if (bytes != null)
            {
                var rect = Spatial.Rect2.FromPoints(
                    image.TopLeft.X,
                    image.TopLeft.Y,
                    image.BottomRight.X,
                    image.BottomRight.Y,
                    dx, dy);

                var dxfImageDefinitionCached = _biCache.Get(image.Key);
                if (dxfImageDefinitionCached != null)
                {
                    var dxfImage = new DXFE.Image(
                        dxfImageDefinitionCached,
                        new DXF.Vector3(ToDxfX(rect.X), ToDxfY(rect.Y + rect.Height), 0),
                        rect.Width,
                        rect.Height);
                    dxf.AddEntity(dxfImage);
                }
                else
                {
                    if (State.ImageCache == null || string.IsNullOrEmpty(image.Key))
                        return;

                    var path = System.IO.Path.Combine(_outputPath, System.IO.Path.GetFileName(image.Key));
                    System.IO.File.WriteAllBytes(path, bytes);
                    var dxfImageDefinition = new DXFO.ImageDefinition(path);

                    _biCache.Set(image.Key, dxfImageDefinition);

                    var dxfImage = new DXFE.Image(
                        dxfImageDefinition,
                        new DXF.Vector3(ToDxfX(rect.X), ToDxfY(rect.Y + rect.Height), 0),
                        rect.Width,
                        rect.Height);
                    dxf.AddEntity(dxfImage);
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(object dc, IPathShape path, double dx, double dy, object db, object r)
        {
            if (!path.IsStroked && !path.IsFilled)
                return;

            var dxf = dc as DXF.DxfDocument;
            var style = path.Style;

            CreateHatchBoundsAndEntitiess(path.Geometry, dx, dy, out var bounds, out var entities);
            if (entities == null || bounds == null)
                return;

            if (path.IsFilled)
            {
                var fill = ToColor(style.Fill);
                var fillTransparency = ToTransparency(style.Fill);

                var hatch = new DXFE.Hatch(DXFE.HatchPattern.Solid, bounds, false);
                hatch.Layer = _currentLayer;
                hatch.Color = fill;
                hatch.Transparency.Value = fillTransparency;

                dxf.AddEntity(hatch);
            }

            if (path.IsStroked)
            {
                // TODO: Add support for Closed paths.

                var stroke = ToColor(style.Stroke);
                var strokeTansparency = ToTransparency(style.Stroke);
                var lineweight = ToLineweight(style.Thickness);

                foreach (var entity in entities)
                {
                    entity.Layer = _currentLayer;
                    entity.Color = stroke;
                    entity.Transparency.Value = strokeTansparency;
                    entity.Lineweight = lineweight;
                    dxf.AddEntity(entity);
                }
            }
        }

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeState() => _state != null;
    }
}
