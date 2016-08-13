// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;
using Core2D.Shapes;

namespace Core2D.Renderer
{
    /// <summary>
    /// Native shape renderer base class.
    /// </summary>
    public abstract class ShapeRenderer : ObservableObject
    {
        private ShapeRendererState _state = new ShapeRendererState();

        /// <summary>
        /// Gets or sets renderer state.
        /// </summary>
        public virtual ShapeRendererState State
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
        /// Draws a <see cref="XContainer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="container">The <see cref="XContainer"/> object.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public virtual void Draw(object dc, XContainer container, ImmutableArray<XProperty> db, XRecord r)
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
        /// Draws a <see cref="XLayer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="layer">The <see cref="XLayer"/> object.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public virtual void Draw(object dc, XLayer layer, ImmutableArray<XProperty> db, XRecord r)
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
        public abstract void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XRectangle"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="rectangle">The <see cref="XRectangle"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XEllipse"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="ellipse">The <see cref="XEllipse"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XArc"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="arc">The <see cref="XArc"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XCubicBezier"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="cubicBezier">The <see cref="XCubicBezier"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XQuadraticBezier"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="quadraticBezier">The <see cref="XQuadraticBezier"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XText"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="text">The <see cref="XText"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XText text, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XImage"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="image">The <see cref="XImage"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Draws a <see cref="XPath"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="path">The <see cref="XPath"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        public abstract void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);
    }
}
