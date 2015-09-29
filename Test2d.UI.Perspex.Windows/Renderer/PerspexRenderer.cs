// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Media;

namespace Test2d.UI.Perspex.Windows
{
    public class PerspexRenderer : ObservableObject, IRenderer
    {
        private RendererState _state = new RendererState();

        /// <summary>
        /// 
        /// </summary>
        public RendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public PerspexRenderer() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IRenderer Create()
        {
            return new PerspexRenderer();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private Pen ToPen(BaseStyle style)
        {
            var lineCap = default(PenLineCap);
            var dashStyle = default(DashStyle);
            
            switch (style.LineCap)
            {
                case LineCap.Flat:
                    lineCap = PenLineCap.Flat;
                    break;
                case LineCap.Square:
                    lineCap = PenLineCap.Square;
                    break;
                case LineCap.Round:
                    lineCap = PenLineCap.Round;
                    break;
            }
            
            if (style.Dashes != null)
            {
                dashStyle = new DashStyle(
                    style.Dashes,
                    style.DashOffset);
            }
            
            var pen = new Pen(
                ToSolidBrush(style.Stroke), 
                style.Thickness / _state.Zoom,
                dashStyle, lineCap, 
                lineCap, lineCap);
  
            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private SolidColorBrush ToSolidBrush(ArgbColor color)
        {
            return new SolidColorBrush(ToColor(color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isZooming"></param>
        public void ClearCache(bool isZooming)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, Container container, ImmutableArray<ShapeProperty> db, Record r)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(dc, layer, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, Layer layer, ImmutableArray<ShapeProperty> db, Record r)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_state.DrawShapeState))
                {
                    shape.Draw(dc, this, 0, 0, db, r);
                }
            }
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
        public void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
            var _dc = dc as IDrawingContext;

            Pen strokeLine = ToPen(line.Style);
            
            _dc.DrawLine(
                strokeLine,
                new Point(line.Start.X, line.Start.Y),
                new Point(line.End.X, line.End.Y)); 
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
        public void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
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
        public void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
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
        public void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
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
        public void Draw(object dc, XBezier bezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
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
        public void Draw(object dc, XQBezier qbezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
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
        public void Draw(object dc, XText text, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
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
        public void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
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
        public void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }
    }
}
