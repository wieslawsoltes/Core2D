using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using AM = Avalonia.Media;
using AMI = Avalonia.Media.Imaging;

namespace Core2D.UI.Renderer
{
    /// <summary>
    /// Native Avalonia shape renderer.
    /// </summary>
    public class AvaloniaRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private IShapeRendererState _state;
        private readonly ICache<string, AMI.Bitmap> _biCache;
        private readonly ICache<object, DrawNode> _drawNodeCache;

        /// <inheritdoc/>
        public IShapeRendererState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, AMI.Bitmap>(x => x.Dispose());
            _drawNodeCache = _serviceProvider.GetService<IFactory>().CreateCache<object, DrawNode>(x => x.Dispose());
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void InvalidateCache(IShapeStyle style)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void InvalidateCache(IBaseShape shape, IShapeStyle style, double dx, double dy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                // TODO: _biCache.Reset();
                // TODO: _drawNodeCache.Reset();
            }
        }

        /// <inheritdoc/>
        public void Fill(object dc, double x, double y, double width, double height, IColor color)
        {
            var context = dc as AM.DrawingContext;

            var drawNodeCached = _drawNodeCache.Get(color);
            if (drawNodeCached != null)
            {
                if (drawNodeCached is FillDrawNode drawNode)
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
                    drawNode.Draw(context, 0, 0, _state.ZoomX);
                }
            }
            else
            {
                var drawNode = new FillDrawNode(x, y, width, height, color);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(color, drawNode);

                drawNode.Draw(context, 0, 0, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawPage(object dc, IPageContainer container, double dx, double dy)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawLayer(object dc, ILayerContainer layer, double dx, double dy)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.DrawShape(dc, this, dx, dy);
                }
            }

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.Flags.HasFlag(_state.DrawShapeState.Flags))
                {
                    shape.DrawPoints(dc, this, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawPoint(object dc, IPointShape point, double dx, double dy)
        {
            var isSelected = _state.SelectedShapes?.Count > 0 && _state.SelectedShapes.Contains(point);
            var pointStyle = isSelected ? _state.SelectedPointStyle : _state.PointStyle;
            var pointSize = _state.PointSize;
            if (pointStyle == null || pointSize <= 0.0)
            {
                return;
            }

            var context = dc as AM.DrawingContext;

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
                    drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
                }
            }
            else
            {
                var drawNode = new PointDrawNode(point, pointStyle, pointSize);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(point, drawNode);

                if (_state.DrawPoints == true)
                {
                    drawNode.Draw(context, dx, dy, _state.ZoomX);
                }
            }
        }

        /// <inheritdoc/>
        public void DrawLine(object dc, ILineShape line, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new LineDrawNode(line, line.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(line, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawRectangle(object dc, IRectangleShape rectangle, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new RectangleDrawNode(rectangle, rectangle.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(rectangle, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawEllipse(object dc, IEllipseShape ellipse, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new EllipseDrawNode(ellipse, ellipse.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(ellipse, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawArc(object dc, IArcShape arc, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new ArcDrawNode(arc, arc.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(arc, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawCubicBezier(object dc, ICubicBezierShape cubicBezier, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new CubicBezierDrawNode(cubicBezier, cubicBezier.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(cubicBezier, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawQuadraticBezier(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new QuadraticBezierDrawNode(quadraticBezier, quadraticBezier.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(quadraticBezier, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawText(object dc, ITextShape text, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new TextDrawNode(text, text.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(text, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawImage(object dc, IImageShape image, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new ImageDrawNode(image, image.Style, _state.ImageCache, _biCache);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(image, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }

        /// <inheritdoc/>
        public void DrawPath(object dc, IPathShape path, double dx, double dy)
        {
            var context = dc as AM.DrawingContext;

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

                drawNodeCached.Draw(context, dx, dy, _state.ZoomX);
            }
            else
            {
                var drawNode = new PathDrawNode(path, path.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(path, drawNode);

                drawNode.Draw(context, dx, dy, _state.ZoomX);
            }
        }
    }
}
