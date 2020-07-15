using System;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    /// <summary>
    /// Node shape renderer.
    /// </summary>
    public abstract class NodeRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private readonly ICache<string, IDisposable> _biCache;
        private readonly ICache<object, IDrawNode> _drawNodeCache;
        private readonly IDrawNodeFactory _drawNodeFactory;

        /// <inheritdoc/>
        public IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="drawNodeFactory">The draw node factory.</param>
        public NodeRenderer(IServiceProvider serviceProvider, IDrawNodeFactory drawNodeFactory)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, IDisposable>(x => x.Dispose());
            _drawNodeCache = _serviceProvider.GetService<IFactory>().CreateCache<object, IDrawNode>(x => x.Dispose());
            _drawNodeFactory = drawNodeFactory;
        }

        /// <inheritdoc/>
        public void ClearCache()
        {
            _biCache.Reset();
            _drawNodeCache.Reset();
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var drawNodeCached = _drawNodeCache.Get(color);
            if (drawNodeCached != null)
            {
                if (drawNodeCached is IFillDrawNode drawNode)
                {
                    drawNode.X = x;
                    drawNode.Y = y;
                    drawNode.Width = width;
                    drawNode.Height = height;
                    drawNode.UpdateGeometry();
                    if (color.IsDirty())
                    {
                        drawNode.Color = color;
                        drawNode.UpdateStyle();
                        color.Invalidate();
                    }
                    drawNode.Draw(dc, _state.ZoomX);
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateFillDrawNode(x, y, width, height, color);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(color, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawPage(object dc, IPageContainer container)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawLayer(object dc, ILayerContainer layer)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.DrawShape(dc, this);
                }
            }

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.DrawPoints(dc, this);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawPoint(object dc, IPointShape point)
        {
            var isSelected = _state.SelectedShapes?.Count > 0 && _state.SelectedShapes.Contains(point);
            var pointStyle = isSelected ? _state.SelectedPointStyle : _state.PointStyle;
            var pointSize = _state.PointSize;
            if (pointStyle == null || pointSize <= 0.0)
            {
                return;
            }

            var drawNodeCached = _drawNodeCache.Get(point);
            if (drawNodeCached != null)
            {
                if (pointStyle.IsDirty() || drawNodeCached.Style != pointStyle)
                {
                    drawNodeCached.Style = pointStyle;
                    drawNodeCached.UpdateStyle();
                }

                if (point.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                if (_state.DrawPoints == true)
                {
                    drawNodeCached.Draw(dc, _state.ZoomX);
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreatePointDrawNode(point, pointStyle, pointSize);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(point, drawNode);

                if (_state.DrawPoints == true)
                {
                    drawNode.Draw(dc, _state.ZoomX);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawLine(object dc, ILineShape line)
        {
            var drawNodeCached = _drawNodeCache.Get(line);
            if (drawNodeCached != null)
            {
                if (line.Style.IsDirty() || drawNodeCached.Style != line.Style)
                {
                    drawNodeCached.Style = line.Style;
                    drawNodeCached.UpdateStyle();
                    drawNodeCached.UpdateGeometry();
                    line.Style.Invalidate();
                }

                if (line.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateLineDrawNode(line, line.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(line, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawRectangle(object dc, IRectangleShape rectangle)
        {
            var drawNodeCached = _drawNodeCache.Get(rectangle);
            if (drawNodeCached != null)
            {
                if (rectangle.Style.IsDirty() || drawNodeCached.Style != rectangle.Style)
                {
                    drawNodeCached.Style = rectangle.Style;
                    drawNodeCached.UpdateStyle();
                    rectangle.Style.Invalidate();
                }

                if (rectangle.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateRectangleDrawNode(rectangle, rectangle.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(rectangle, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawEllipse(object dc, IEllipseShape ellipse)
        {
            var drawNodeCached = _drawNodeCache.Get(ellipse);
            if (drawNodeCached != null)
            {
                if (ellipse.Style.IsDirty() || drawNodeCached.Style != ellipse.Style)
                {
                    drawNodeCached.Style = ellipse.Style;
                    drawNodeCached.UpdateStyle();
                    ellipse.Style.Invalidate();
                }

                if (ellipse.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateEllipseDrawNode(ellipse, ellipse.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(ellipse, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawArc(object dc, IArcShape arc)
        {
            var drawNodeCached = _drawNodeCache.Get(arc);
            if (drawNodeCached != null)
            {
                if (arc.Style.IsDirty() || drawNodeCached.Style != arc.Style)
                {
                    drawNodeCached.Style = arc.Style;
                    drawNodeCached.UpdateStyle();
                    arc.Style.Invalidate();
                }

                if (arc.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateArcDrawNode(arc, arc.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(arc, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawCubicBezier(object dc, ICubicBezierShape cubicBezier)
        {
            var drawNodeCached = _drawNodeCache.Get(cubicBezier);
            if (drawNodeCached != null)
            {
                if (cubicBezier.Style.IsDirty() || drawNodeCached.Style != cubicBezier.Style)
                {
                    drawNodeCached.Style = cubicBezier.Style;
                    drawNodeCached.UpdateStyle();
                    cubicBezier.Style.Invalidate();
                }

                if (cubicBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateCubicBezierDrawNode(cubicBezier, cubicBezier.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(cubicBezier, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawQuadraticBezier(object dc, IQuadraticBezierShape quadraticBezier)
        {
            var drawNodeCached = _drawNodeCache.Get(quadraticBezier);
            if (drawNodeCached != null)
            {
                if (quadraticBezier.Style.IsDirty() || drawNodeCached.Style != quadraticBezier.Style)
                {
                    drawNodeCached.Style = quadraticBezier.Style;
                    drawNodeCached.UpdateStyle();
                    quadraticBezier.Style.Invalidate();
                }

                if (quadraticBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateQuadraticBezierDrawNode(quadraticBezier, quadraticBezier.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(quadraticBezier, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawText(object dc, ITextShape text)
        {
            var drawNodeCached = _drawNodeCache.Get(text);
            if (drawNodeCached != null)
            {
                if (text.Style.IsDirty() || drawNodeCached.Style != text.Style)
                {
                    drawNodeCached.Style = text.Style;
                    drawNodeCached.UpdateStyle();
                    drawNodeCached.UpdateGeometry();
                    text.Style.Invalidate();
                }

                if (text.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateTextDrawNode(text, text.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(text, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawImage(object dc, IImageShape image)
        {
            var drawNodeCached = _drawNodeCache.Get(image);
            if (drawNodeCached != null)
            {
                if (image.Style.IsDirty() || drawNodeCached.Style != image.Style)
                {
                    drawNodeCached.Style = image.Style;
                    drawNodeCached.UpdateStyle();
                    image.Style.Invalidate();
                }

                if (image.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateImageDrawNode(image, image.Style, _state.ImageCache, _biCache);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(image, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawPath(object dc, IPathShape path)
        {
            var drawNodeCached = _drawNodeCache.Get(path);
            if (drawNodeCached != null)
            {
                if (path.Style.IsDirty() || drawNodeCached.Style != path.Style)
                {
                    drawNodeCached.Style = path.Style;
                    drawNodeCached.UpdateStyle();
                    path.Style.Invalidate();
                }

                if (path.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreatePathDrawNode(path, path.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(path, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeState() => _state != null;
    }
}
