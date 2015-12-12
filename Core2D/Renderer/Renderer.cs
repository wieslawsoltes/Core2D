// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Base class for native shape renderer.
    /// </summary>
    public abstract class Renderer : ObservableObject
    {
        private RendererState _state = new RendererState();

        /// <summary>
        /// Gets or sets renderer state.
        /// </summary>
        public virtual RendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// Clears renderer cache.
        /// </summary>
        /// <param name="isZooming">The flag indicating zooming state.</param>
        public virtual void ClearCache(bool isZooming) { }

        /// <summary>
        /// Draws a <see cref="Container"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="container">The <see cref="Container"/> object.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public virtual void Draw(object dc, Container container, ImmutableArray<Property> db, Record r)
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
        /// Draws a <see cref="Layer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="layer">The <see cref="Layer"/> object.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public virtual void Draw(object dc, Layer layer, ImmutableArray<Property> db, Record r)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(State.DrawShapeState.Flags))
                {
                    shape.Draw(dc, this, 0, 0, db, r);
                }
            }
        }

        /// <summary>
        /// Draws a <see cref="XLine"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="line">The <see cref="XLine"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XRectangle"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="rectangle">The <see cref="XRectangle"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XEllipse"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="ellipse">The <see cref="XEllipse"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XArc"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="arc">The <see cref="XArc"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XBezier"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="bezier">The <see cref="XBezier"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XBezier bezier, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XQBezier"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="qbezier">The <see cref="XQBezier"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XQBezier qbezier, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XText"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="text">The <see cref="XText"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XText text, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XImage"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="image">The <see cref="XImage"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Draws a <see cref="XPath"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="path">The <see cref="XPath"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<Property> db, Record r);
    }
}
