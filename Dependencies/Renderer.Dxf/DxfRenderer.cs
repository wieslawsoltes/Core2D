// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using netDxf;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.IO;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using T2d=Test2d;

namespace netDxf
{
    /// <summary>
    /// 
    /// </summary>
    public static class DxfHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        public static double LineweightFactor = 96.0/2540.0;
        
        /// <summary>
        /// 
        /// </summary>
        public static short[] Lineweights = 
        {
            -3, -2, -1, 0, 5, 9, 13, 15, 18, 20, 25, 30, 35, 40, 50, 53, 60, 70, 80, 90, 100, 106, 120, 140, 158, 200, 211 
        };
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static short ThicknessToLineweight(double thickness)
        {
            short lineweight = (short)(thickness / LineweightFactor);
            return Lineweights.OrderBy(x => Math.Abs((long) x - lineweight)).First();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class DxfRenderer : T2d.ObservableObject, T2d.IRenderer
    {
        private T2d.RendererState _state = new T2d.RendererState();

        /// <summary>
        /// 
        /// </summary>
        public T2d.RendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DxfRenderer()
        {
            ClearCache(isZooming: false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static T2d.IRenderer Create()
        {
            return new DxfRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isZooming"></param>
        public void ClearCache(bool isZooming)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.Container container, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.Layer layer, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XLine line, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XRectangle rectangle, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XEllipse ellipse, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XArc arc, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XBezier bezier, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XQBezier qbezier, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XText text, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XImage image, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="path"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, T2d.XPath path, double dx, double dy, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            // TODO: Implement Draw().
        }

        private double _pageWidth;
        private double _pageHeight;

        private double ToDxfX(double x) 
        { 
            return x; 
        }

        private double ToDxfY(double y) 
        { 
            return _pageHeight - y;
        }

        private Line CreateLine(T2d.XLine line, double x1, double y1, double x2, double y2)
        {
            if (line != null)
            {
                T2d.XLine.SetMaxLength(line, ref x1, ref y1, ref x2, ref y2);
            }

            double _x1 = ToDxfX(x1);
            double _y1 = ToDxfY(y1);
            double _x2 = ToDxfX(x2);
            double _y2 = ToDxfY(y2);

            return new Line(new Vector3(_x1, _y1, 0), new Vector3(_x2, _y2, 0));
        }

        private Ellipse CreateEllipse(double x, double y, double width, double height)
        {
            double _cx = ToDxfX(x + width / 2.0);
            double _cy = ToDxfY(y + height / 2.0);
            double minor = Math.Min(height, width);
            double major = Math.Max(height, width);

            return new Ellipse()
            {
                Center = new Vector3(_cx, _cy, 0),
                MajorAxis = major,
                MinorAxis = minor,
                Rotation  = height > width ? 90.0 : 0.0
            };
        }

        private Arc CreateArc(double x, double y, double radius, double startAngle, double endAngle)
        {
            double _cx = ToDxfX(x + radius / 2.0);
            double _cy = ToDxfY(y + radius / 2.0);

            return new Arc(new Vector3(_cx, _cy, 0), radius, startAngle, endAngle);
        }

        private Spline CreateSpline(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y, double p4x, double p4y)
        {
            double _p1x = ToDxfX(p1x);
            double _p1y = ToDxfY(p1y);
            double _p2x = ToDxfX(p2x);
            double _p2y = ToDxfY(p2y);
            double _p3x = ToDxfX(p3x);
            double _p3y = ToDxfY(p3y);
            double _p4x = ToDxfX(p4x);
            double _p4y = ToDxfY(p4y);

            return new Spline(
                new List<SplineVertex>
                {
                    new SplineVertex(_p1x, _p1y, 0.0),
                    new SplineVertex(_p2x, _p2y, 0.0),
                    new SplineVertex(_p3x, _p3y, 0.0),
                    new SplineVertex(_p4x, _p4y, 0.0)
                }, 3);
        }

        private Text CreateText(string text, double x, double y, double height, TextAlignment alignment)
        {
            return new Text(text, new Vector3(ToDxfX(x), ToDxfY(y), 0), height)
            {
                Alignment = alignment
            };
        }

        private void DrawLine(DxfDocument doc, T2d.XLine line, Layer layer)
        {
            var style = line.Style;
            var dxfLine = CreateLine(line, line.Start.X, line.Start.Y, line.End.X, line.End.Y);
            dxfLine.Layer = layer;
            //dxfLine.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfLine.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfLine.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            doc.AddEntity(dxfLine);
        }

        private void DrawRectangle(DxfDocument doc, T2d.XRectangle rectangle, Layer layer)
        {
            var style = rectangle.Style;
            var rect = T2d.Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);
            var dxfLine1 = CreateLine(null, rect.X, rect.Y, rect.X + rect.Width, rect.Y);
            var dxfLine2 = CreateLine(null, rect.X, rect.Y + rect.Height, rect.X + rect.Width, rect.Y + rect.Height);
            var dxfLine3 = CreateLine(null, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
            var dxfLine4 = CreateLine(null, rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            dxfLine1.Layer = layer;
            //dxfLine1.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfLine1.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfLine1.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            dxfLine2.Layer = layer;
            //dxfLine2.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfLine2.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfLine2.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            dxfLine3.Layer = layer;
            //dxfLine3.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfLine3.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfLine3.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            dxfLine4.Layer = layer;
            //dxfLine4.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfLine4.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfLine4.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            doc.AddEntity(dxfLine1);
            doc.AddEntity(dxfLine2);
            doc.AddEntity(dxfLine3);
            doc.AddEntity(dxfLine4);
        }

        private void DrawEllipse(DxfDocument doc, T2d.XEllipse ellipse, Layer layer)
        {
            var style = ellipse.Style;
            var rect = T2d.Rect2.Create(ellipse.TopLeft, ellipse.BottomRight);
            var dxfEllipse = CreateEllipse(
                rect.X, 
                rect.Y,
                rect.Width, 
                rect.Height);
            dxfEllipse.Layer = layer;
            //dxfEllipse.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfEllipse.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfEllipse.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            doc.AddEntity(dxfEllipse);
        }

        private void DrawArc(DxfDocument doc, T2d.XArc arc, Layer layer)
        {
            var style = arc.Style;
            var a = T2d.GdiArc.FromXArc(arc, 0.0, 0.0);

            double _cx = ToDxfX(a.X + a.Width / 2.0);
            double _cy = ToDxfY(a.Y + a.Height / 2.0);
            double minor = Math.Min(a.Height, a.Width);
            double major = Math.Max(a.Height, a.Width);

            if (a.Height == a.Width)
            {
                //*/
                double startAngle = a.StartAngle;
                double endAngle = a.EndAngle;
                double rotation  = 0.0;
                
                var dxfEllipse = new Ellipse()
                {
                    Center = new Vector3(_cx, _cy, 0),
                    MajorAxis = major,
                    MinorAxis = minor,
                    StartAngle = startAngle,
                    EndAngle = endAngle,
                    Rotation  = rotation
                };
                dxfEllipse.Layer = layer;
                //dxfEllipse.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
                //dxfEllipse.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
                //dxfEllipse.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
                doc.AddEntity(dxfEllipse);
                //*/
            }
            else if (a.Height > a.Width)
            {
                //*
                double startAngle = a.StartAngle;
                double endAngle = a.EndAngle;
                double rotation  = 90.0;
                
                var dxfEllipse = new Ellipse()
                {
                    Center = new Vector3(_cx, _cy, 0),
                    MajorAxis = major,
                    MinorAxis = minor,
                    StartAngle = startAngle,
                    EndAngle = endAngle,
                    Rotation  = rotation
                };
                dxfEllipse.Layer = layer;
                //dxfEllipse.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
                //dxfEllipse.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
                //dxfEllipse.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
                doc.AddEntity(dxfEllipse);
                //*/
            }
            else if (a.Height < a.Width)
            {
                //*
                double startAngle = a.StartAngle;
                double endAngle = a.EndAngle;
                double rotation  = 0.0;
                
                var dxfEllipse = new Ellipse()
                {
                    Center = new Vector3(_cx, _cy, 0),
                    MajorAxis = major,
                    MinorAxis = minor,
                    StartAngle = startAngle,
                    EndAngle = endAngle,
                    Rotation  = rotation
                };
                dxfEllipse.Layer = layer;
                //dxfEllipse.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
                //dxfEllipse.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
                //dxfEllipse.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
                doc.AddEntity(dxfEllipse);
                //*/
            }

            /*
            var dxfArc = CreateArc(
                a.X, 
                a.Y,
                a.RadiusX, 
                a.StartAngle, 
                a.EndAngle);
            dxfArc.Layer = layer;

            doc.AddEntity(dxfArc);
            */
        }

        private void DrawBezier(DxfDocument doc, T2d.XBezier bezier, Layer layer)
        {
            var style = bezier.Style;
            var dxfSpline = CreateSpline(
                bezier.Point1.X, bezier.Point1.Y,
                bezier.Point2.X, bezier.Point2.Y,
                bezier.Point3.X, bezier.Point3.Y,
                bezier.Point4.X, bezier.Point4.Y);
            dxfSpline.Layer = layer;
            //dxfSpline.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfSpline.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfSpline.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            doc.AddEntity(dxfSpline);
        }

        private void DrawQBezier(DxfDocument doc, T2d.XQBezier qbezier, Layer layer)
        {
            var style = qbezier.Style;
            double x1 = qbezier.Point1.X;
            double y1 = qbezier.Point1.Y;
            double x2 = qbezier.Point1.X + (2.0 * (qbezier.Point2.X - qbezier.Point1.X)) / 3.0;
            double y2 = qbezier.Point1.Y + (2.0 * (qbezier.Point2.Y - qbezier.Point1.Y)) / 3.0;
            double x3 = x2 + (qbezier.Point3.X - qbezier.Point1.X) / 3.0;
            double y3 = y2 + (qbezier.Point3.Y - qbezier.Point1.Y) / 3.0;
            double x4 = qbezier.Point3.X;
            double y4 = qbezier.Point3.Y;

            var dxfSpline = CreateSpline(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4);
            dxfSpline.Layer = layer;
            //dxfSpline.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfSpline.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfSpline.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            doc.AddEntity(dxfSpline);
        }

        private void DrawText(DxfDocument doc, T2d.XText text, Layer layer, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            var style = text.Style;
            var alignment = default(TextAlignment);
            double x, y;
            var rect = T2d.Rect2.Create(text.TopLeft, text.BottomRight);
            
            switch (text.Style.TextStyle.TextHAlignment)
            {
                default:
                case T2d.TextHAlignment.Left:
                    x = rect.X;
                    break;
                case T2d.TextHAlignment.Center:
                    x = rect.X + rect.Width / 2.0;
                    break;
                case T2d.TextHAlignment.Right:
                    x = rect.X + rect.Width;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                default:
                case T2d.TextVAlignment.Top:
                    y = rect.Y;
                    break;
                case T2d.TextVAlignment.Center:
                    y = rect.Y + rect.Height / 2.0;
                    break;
                case T2d.TextVAlignment.Bottom:
                    y = rect.Y + rect.Height;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                default:
                case T2d.TextVAlignment.Top:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case T2d.TextHAlignment.Left:
                            alignment = TextAlignment.TopLeft;
                            break;
                        case T2d.TextHAlignment.Center:
                            alignment = TextAlignment.TopCenter;
                            break;
                        case T2d.TextHAlignment.Right:
                            alignment = TextAlignment.TopRight;
                            break;
                    }
                    break;
                case T2d.TextVAlignment.Center:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case T2d.TextHAlignment.Left:
                            alignment = TextAlignment.MiddleLeft;
                            break;
                        case T2d.TextHAlignment.Center:
                            alignment = TextAlignment.MiddleCenter;
                            break;
                        case T2d.TextHAlignment.Right:
                            alignment = TextAlignment.MiddleRight;
                            break;
                    }
                    break;
                case T2d.TextVAlignment.Bottom:
                    switch (text.Style.TextStyle.TextHAlignment)
                    {
                        default:
                        case T2d.TextHAlignment.Left:
                            alignment = TextAlignment.BaselineLeft;
                            break;
                        case T2d.TextHAlignment.Center:
                            alignment = TextAlignment.BottomCenter;
                            break;
                        case T2d.TextHAlignment.Right:
                            alignment = TextAlignment.BottomRight;
                            break;
                    }
                    break;
            }


            var dxfText = CreateText(
                text.BindToTextProperty(db, r),
                x, y,
                text.Style.TextStyle.FontSize * (72.0 / 96.0),
                alignment);
            dxfText.Layer = layer;
            //dxfText.Color = new AciColor(style.Stroke.R, style.Stroke.G, style.Stroke.B);
            //dxfText.Transparency.Value = (short)((double)style.Stroke.A * 90.0/255.0);
            //dxfText.Lineweight.Value = DxfHelpers.ThicknessToLineweight(style.Thickness);
            doc.AddEntity(dxfText);
        }

        private void DrawShapes(DxfDocument doc, IEnumerable<T2d.BaseShape> shapes, Layer layer, ImmutableArray<T2d.ShapeProperty> db, T2d.Record r)
        {
            foreach (var shape in shapes) 
            {
                if (shape.State.HasFlag(_state.DrawShapeState))
                {
                    if (shape is T2d.XPoint)
                    {
                        var point = shape as T2d.XPoint;
                        // TODO: Draw point.
                    }
                    else if (shape is T2d.XLine)
                    {
                        var line = shape as T2d.XLine;
                        DrawLine(doc, line, layer);
                        // TODO: Draw start and end arrows.
                    }
                    else if (shape is T2d.XRectangle)
                    {
                        var rectangle = shape as T2d.XRectangle;
                        DrawRectangle(doc, rectangle, layer);
                        // TODO: Draw rectangle grid.
                    }
                    else if (shape is T2d.XEllipse)
                    {
                        var ellipse = shape as T2d.XEllipse;
                        DrawEllipse(doc, ellipse, layer);
                    }
                    else if (shape is T2d.XArc)
                    {
                        var arc = shape as T2d.XArc;
                        DrawArc(doc, arc, layer);
                    }
                    else if (shape is T2d.XBezier)
                    {
                        var bezier = shape as T2d.XBezier;
                        DrawBezier(doc, bezier, layer);
                    }
                    else if (shape is T2d.XQBezier)
                    {
                        var qbezier = shape as T2d.XQBezier;
                        DrawQBezier(doc, qbezier, layer);
                    }
                    else if (shape is T2d.XText)
                    {
                        var text = shape as T2d.XText;
                        DrawText(doc, text, layer, db, r);
                    }
                    else if (shape is T2d.XImage)
                    {
                        var image = shape as T2d.XImage;
                        // TODO: Draw image.
                    }
                    else if (shape is T2d.XGroup)
                    {
                        var group = shape as T2d.XGroup;
                        DrawShapes(doc, group.Shapes, layer, db, r == null ? group.Record : r);
                    }
                    else if (shape is T2d.XPath)
                    {
                        var path = shape as T2d.XPath;
                        // TODO: Draw path.
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        public void Save(string path, T2d.Container container)
        {
            DxfDocument doc = new DxfDocument(Header.DxfVersion.AutoCad2010);

            _pageWidth = container.Width;
            _pageHeight = container.Height;
            
            if (container.Template != null)
            {
                foreach (var layer in container.Template.Layers) 
                {
                    var dxfLayer = new Layer(layer.Name)
                    {
                        IsVisible = layer.IsVisible
                    };
                    doc.Layers.Add(dxfLayer);

                    DrawShapes(doc, layer.Shapes, dxfLayer, container.Properties, null);
                }
            }
            
            foreach (var layer in container.Layers) 
            {
                var dxfLayer = new Layer(layer.Name)
                {
                    IsVisible = layer.IsVisible
                };
                doc.Layers.Add(dxfLayer);

                DrawShapes(doc, layer.Shapes, dxfLayer, container.Properties, null);
            }

            doc.Save(path);
        }
    }
}
