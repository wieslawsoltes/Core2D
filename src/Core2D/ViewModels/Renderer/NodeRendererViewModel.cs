using System;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public partial class NodeRendererViewModel : ViewModelBase, IShapeRenderer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICache<string, IDisposable> _biCache;
        private readonly ICache<object, IDrawNode> _drawNodeCache;
        private readonly IDrawNodeFactory _drawNodeFactory;

        [AutoNotify] private ShapeRendererStateViewModel _stateViewModel;

        protected NodeRendererViewModel(IServiceProvider serviceProvider, IDrawNodeFactory drawNodeFactory)
        {
            _serviceProvider = serviceProvider;
            _stateViewModel = _serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = _serviceProvider.GetService<IFactory>().CreateCache<string, IDisposable>(x => x.Dispose());
            _drawNodeCache = _serviceProvider.GetService<IFactory>().CreateCache<object, IDrawNode>(x => x.Dispose());
            _drawNodeFactory = drawNodeFactory;
        }

        public void ClearCache()
        {
            _biCache.Reset();
            _drawNodeCache.Reset();
        }

        public void Fill(object dc, double x, double y, double width, double height, BaseColorViewModel colorViewModel)
        {
            var drawNodeCached = _drawNodeCache.Get(colorViewModel);
            if (drawNodeCached != null)
            {
                if (drawNodeCached is IFillDrawNode drawNode)
                {
                    drawNode.X = x;
                    drawNode.Y = y;
                    drawNode.Width = width;
                    drawNode.Height = height;
                    drawNode.UpdateGeometry();
                    if (colorViewModel.IsDirty())
                    {
                        drawNode.ColorViewModel = colorViewModel;
                        drawNode.UpdateStyle();
                        colorViewModel.Invalidate();
                    }
                    drawNode.Draw(dc, _stateViewModel.ZoomX);
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateFillDrawNode(x, y, width, height, colorViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(colorViewModel, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
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
                    if (grid.IsDirty() || (grid.GridStrokeColorViewModel != null && grid.GridStrokeColorViewModel.IsDirty()))
                    {
                        drawNode.UpdateStyle();
                        grid.GridStrokeColorViewModel?.Invalidate();
                    }
                    drawNode.Draw(dc, _stateViewModel.ZoomX);
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateGridDrawNode(grid, x, y, width, height);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(grid, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawPage(object dc, PageContainerViewModel containerViewModel)
        {
            foreach (var layer in containerViewModel.Layers)
            {
                if (layer.IsVisible)
                {
                    DrawLayer(dc, layer);
                }
            }
        }

        public void DrawLayer(object dc, LayerContainerViewModel layer)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_stateViewModel.DrawShapeState))
                {
                    shape.DrawShape(dc, this);
                }
            }

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_stateViewModel.DrawShapeState))
                {
                    shape.DrawPoints(dc, this);
                }
            }
        }

        public void DrawPoint(object dc, PointShapeViewModel point)
        {
            var isSelected = _stateViewModel.SelectedShapes?.Count > 0 && _stateViewModel.SelectedShapes.Contains(point);
            var pointStyle = isSelected ? _stateViewModel.SelectedPointStyleViewModel : _stateViewModel.PointStyleViewModel;
            var pointSize = _stateViewModel.PointSize;
            if (pointStyle == null || pointSize <= 0.0)
            {
                return;
            }

            var drawNodeCached = _drawNodeCache.Get(point);
            if (drawNodeCached != null)
            {
                if (pointStyle.IsDirty() || drawNodeCached.StyleViewModel != pointStyle)
                {
                    drawNodeCached.StyleViewModel = pointStyle;
                    drawNodeCached.UpdateStyle();
                }

                if (point.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                if (_stateViewModel.DrawPoints == true)
                {
                    drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreatePointDrawNode(point, pointStyle, pointSize);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(point, drawNode);

                if (_stateViewModel.DrawPoints == true)
                {
                    drawNode.Draw(dc, _stateViewModel.ZoomX);
                }
            }
        }

        public void DrawLine(object dc, LineShapeViewModel line)
        {
            var drawNodeCached = _drawNodeCache.Get(line);
            if (drawNodeCached != null)
            {
                if (line.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != line.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = line.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    drawNodeCached.UpdateGeometry();
                    line.StyleViewModel.Invalidate();
                }

                if (line.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateLineDrawNode(line, line.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(line, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawRectangle(object dc, RectangleShapeViewModel rectangle)
        {
            var drawNodeCached = _drawNodeCache.Get(rectangle);
            if (drawNodeCached != null)
            {
                if (rectangle.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != rectangle.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = rectangle.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    rectangle.StyleViewModel.Invalidate();
                }

                if (rectangle.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateRectangleDrawNode(rectangle, rectangle.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(rectangle, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawEllipse(object dc, EllipseShapeViewModel ellipse)
        {
            var drawNodeCached = _drawNodeCache.Get(ellipse);
            if (drawNodeCached != null)
            {
                if (ellipse.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != ellipse.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = ellipse.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    ellipse.StyleViewModel.Invalidate();
                }

                if (ellipse.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateEllipseDrawNode(ellipse, ellipse.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(ellipse, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawArc(object dc, ArcShapeViewModelViewModel arc)
        {
            var drawNodeCached = _drawNodeCache.Get(arc);
            if (drawNodeCached != null)
            {
                if (arc.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != arc.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = arc.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    arc.StyleViewModel.Invalidate();
                }

                if (arc.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateArcDrawNode(arc, arc.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(arc, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawCubicBezier(object dc, CubicBezierShapeViewModel cubicBezier)
        {
            var drawNodeCached = _drawNodeCache.Get(cubicBezier);
            if (drawNodeCached != null)
            {
                if (cubicBezier.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != cubicBezier.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = cubicBezier.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    cubicBezier.StyleViewModel.Invalidate();
                }

                if (cubicBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateCubicBezierDrawNode(cubicBezier, cubicBezier.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(cubicBezier, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShapeViewModel quadraticBezier)
        {
            var drawNodeCached = _drawNodeCache.Get(quadraticBezier);
            if (drawNodeCached != null)
            {
                if (quadraticBezier.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != quadraticBezier.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = quadraticBezier.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    quadraticBezier.StyleViewModel.Invalidate();
                }

                if (quadraticBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateQuadraticBezierDrawNode(quadraticBezier, quadraticBezier.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(quadraticBezier, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawText(object dc, TextShapeViewModel text)
        {
            var drawNodeCached = _drawNodeCache.Get(text);
            if (drawNodeCached != null)
            {
                if (text.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != text.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = text.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    drawNodeCached.UpdateGeometry();
                    text.StyleViewModel.Invalidate();
                }

                if (text.IsDirty() || IsBoundTextDirty(drawNodeCached, text))
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);

                static bool IsBoundTextDirty(IDrawNode drawNodeCached, TextShapeViewModel text)
                {
                    var boundTextCheck = text.GetProperty(nameof(TextShapeViewModel.Text)) is string boundText ? boundText : text.Text;
                    return drawNodeCached is ITextDrawNode textDrawNode
                        && boundTextCheck != textDrawNode.BoundText;
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateTextDrawNode(text, text.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(text, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawImage(object dc, ImageShapeViewModel image)
        {
            var drawNodeCached = _drawNodeCache.Get(image);
            if (drawNodeCached != null)
            {
                if (image.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != image.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = image.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    image.StyleViewModel.Invalidate();
                }

                if (image.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateImageDrawNode(image, image.StyleViewModel, _stateViewModel.ImageCache, _biCache);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(image, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }

        public void DrawPath(object dc, PathShapeViewModel path)
        {
            var drawNodeCached = _drawNodeCache.Get(path);
            if (drawNodeCached != null)
            {
                if (path.StyleViewModel.IsDirty() || drawNodeCached.StyleViewModel != path.StyleViewModel)
                {
                    drawNodeCached.StyleViewModel = path.StyleViewModel;
                    drawNodeCached.UpdateStyle();
                    path.StyleViewModel.Invalidate();
                }

                if (path.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _stateViewModel.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreatePathDrawNode(path, path.StyleViewModel);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(path, drawNode);

                drawNode.Draw(dc, _stateViewModel.ZoomX);
            }
        }
    }
}
