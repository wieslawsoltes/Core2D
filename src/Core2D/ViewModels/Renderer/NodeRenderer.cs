using System;
using System.Runtime.Serialization;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    [DataContract(IsReference = true)]
    public abstract class NodeRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private ShapeRendererState _state;
        private readonly ICache<string, IDisposable> _biCache;
        private readonly ICache<object, IDrawNode> _drawNodeCache;
        private readonly IDrawNodeFactory _drawNodeFactory;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ShapeRendererState State
        {
            get => _state;
            set => RaiseAndSetIfChanged(ref _state, value);
        }

        public NodeRenderer(IServiceProvider serviceProvider, IDrawNodeFactory drawNodeFactory)
        {
            _serviceProvider = serviceProvider;
            _state = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, IDisposable>(x => x.Dispose());
            _drawNodeCache = _serviceProvider.GetService<IFactory>().CreateCache<object, IDrawNode>(x => x.Dispose());
            _drawNodeFactory = drawNodeFactory;
        }

        public void ClearCache()
        {
            _biCache.Reset();
            _drawNodeCache.Reset();
        }

        public void Fill(object dc, double x, double y, double width, double height, BaseColor color)
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

        public void Grid(object dc, IGrid grid, double x, double y, double width, double height)
        {
            var drawNodeCached = _drawNodeCache.Get(grid);
            if (drawNodeCached != null)
            {
                if (drawNodeCached is IGridDrawNode drawNode)
                {
                    drawNode.X = x;
                    drawNode.Y = y;
                    drawNode.Width = width;
                    drawNode.Height = height;
                    drawNode.UpdateGeometry();
                    if (grid.IsDirty() || (grid.GridStrokeColor != null && grid.GridStrokeColor.IsDirty()))
                    {
                        drawNode.UpdateStyle();
                        grid.GridStrokeColor?.Invalidate();
                    }
                    drawNode.Draw(dc, _state.ZoomX);
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateGridDrawNode(grid, x, y, width, height);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(grid, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawPage(object dc, PageContainer container)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer);
                }
            }
        }

        public void DrawLayer(object dc, LayerContainer layer)
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

        public void DrawPoint(object dc, PointShape point)
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

        public void DrawLine(object dc, LineShape line)
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

        public void DrawRectangle(object dc, RectangleShape rectangle)
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

        public void DrawEllipse(object dc, EllipseShape ellipse)
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

        public void DrawArc(object dc, ArcShape arc)
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

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier)
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

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier)
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

        public void DrawText(object dc, TextShape text)
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

                if (text.IsDirty() || IsBoundTextDirty(drawNodeCached, text))
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);

                static bool IsBoundTextDirty(IDrawNode drawNodeCached, TextShape text)
                {
                    var boundTextCheck = text.GetProperty(nameof(TextShape.Text)) is string boundText ? boundText : text.Text;
                    return drawNodeCached is ITextDrawNode textDrawNode
                        && boundTextCheck != textDrawNode.BoundText;
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateTextDrawNode(text, text.Style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(text, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawImage(object dc, ImageShape image)
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

        public void DrawPath(object dc, PathShape path)
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
    }
}
