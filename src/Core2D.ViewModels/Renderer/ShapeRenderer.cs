// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    /// <summary>
    /// Native shape renderer base class.
    /// </summary>
    public abstract class ShapeRenderer : ObservableObject, IShapeRenderer
    {
        private IShapeRendererState _state = Factory.CreateShapeRendererState();

        /// <inheritdoc/>
        public virtual IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeState() => _state != null;

        /// <inheritdoc/>
        public virtual void ClearCache(bool isZooming) { }

        /// <inheritdoc/>
        public abstract void Fill(object dc, double x, double y, double width, double height, IColor color);

        /// <inheritdoc/>
        public abstract object PushMatrix(object dc, IMatrixObject matrix);

        /// <inheritdoc/>
        public abstract void PopMatrix(object dc, object state);

        /// <inheritdoc/>
        public virtual void Draw(object dc, IPageContainer container, double dx, double dy, object db, object r)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(dc, layer, dx, dy, db, r);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void Draw(object dc, ILayerContainer layer, double dx, double dy, object db, object r)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(State.DrawShapeState.Flags))
                {
                    shape.Draw(dc, this, dx, dy, db, r);
                }
            }
        }

        /// <inheritdoc/>
        public abstract void Draw(object dc, ILineShape line, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, IRectangleShape rectangle, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, IEllipseShape ellipse, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, IArcShape arc, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, ITextShape text, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, IImageShape image, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public abstract void Draw(object dc, IPathShape path, double dx, double dy, object db, object r);
    }
}
