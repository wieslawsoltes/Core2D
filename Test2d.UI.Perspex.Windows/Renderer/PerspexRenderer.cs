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
        /// <param name="isZooming"></param>
        public void ClearCache(bool isZooming)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, Container container, ImmutableArray<ShapeProperty> db, Record r)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(context, layer, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, Layer layer, ImmutableArray<ShapeProperty> db, Record r)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_state.DrawShapeState))
                {
                    shape.Draw(context, this, 0, 0, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XLine line, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XRectangle rectangle, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XEllipse ellipse, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XArc arc, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XBezier bezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XQBezier qbezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XText text, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XImage image, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object context, XPath path, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
        }
    }
}
