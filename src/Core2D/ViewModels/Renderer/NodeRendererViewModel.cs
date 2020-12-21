#nullable disable
using System;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Renderer
{
    public partial class NodeRendererViewModel : ViewModelBase, IShapeRenderer
    {
        private readonly ICache<string, IDisposable> _biCache;
        private readonly ICache<object, IDrawNode> _drawNodeCache;
        private readonly IDrawNodeFactory _drawNodeFactory;

        [AutoNotify] private ShapeRendererStateViewModel _state;

        public NodeRendererViewModel(IServiceProvider serviceProvider, IDrawNodeFactory drawNodeFactory) : base(serviceProvider)
        {
            _state = serviceProvider.GetService<IFactory>().CreateShapeRendererState();
            _biCache = serviceProvider.GetService<IFactory>().CreateCache<string, IDisposable>(x => x.Dispose());
            _drawNodeCache = serviceProvider.GetService<IFactory>().CreateCache<object, IDrawNode>(x => x.Dispose());
            _drawNodeFactory = drawNodeFactory;
        }

        public void ClearCache()
        {
            _biCache.Reset();
            _drawNodeCache.Reset();
        }

        public void Fill(object dc, double x, double y, double width, double height, BaseColorViewModel color)
        {
            var drawNodeCached = _drawNodeCache.Get(color);
            if (drawNodeCached is { })
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
            if (drawNodeCached is { })
            {
                if (drawNodeCached is IGridDrawNode drawNode)
                {
                    drawNode.X = x;
                    drawNode.Y = y;
                    drawNode.Width = width;
                    drawNode.Height = height;
                    drawNode.UpdateGeometry();
                    if (grid.IsDirty() || (grid.GridStrokeColor is { } && grid.GridStrokeColor.IsDirty()))
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

        public void DrawPoint(object dc, PointShapeViewModel point, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(point);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
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
                var drawNode = _drawNodeFactory.CreatePointDrawNode(point, style, _state.PointSize);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(point, drawNode);

                if (_state.DrawPoints == true)
                {
                    drawNode.Draw(dc, _state.ZoomX);
                }
            }
        }

        public void DrawLine(object dc, LineShapeViewModel line, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(line);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = line.Style;
                    drawNodeCached.UpdateStyle();
                    drawNodeCached.UpdateGeometry();
                    style.Invalidate();
                }

                if (line.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateLineDrawNode(line, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(line, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawRectangle(object dc, RectangleShapeViewModel rectangle, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(rectangle);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    style.Invalidate();
                }

                if (rectangle.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateRectangleDrawNode(rectangle, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(rectangle, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawEllipse(object dc, EllipseShapeViewModel ellipse, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(ellipse);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    style.Invalidate();
                }

                if (ellipse.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateEllipseDrawNode(ellipse, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(ellipse, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawArc(object dc, ArcShapeViewModel arc, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(arc);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    style.Invalidate();
                }

                if (arc.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateArcDrawNode(arc, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(arc, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawCubicBezier(object dc, CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(cubicBezier);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    style.Invalidate();
                }

                if (cubicBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateCubicBezierDrawNode(cubicBezier, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(cubicBezier, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(quadraticBezier);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    style.Invalidate();
                }

                if (quadraticBezier.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateQuadraticBezierDrawNode(quadraticBezier, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(quadraticBezier, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawText(object dc, TextShapeViewModel text, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(text);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    drawNodeCached.UpdateGeometry();
                    style.Invalidate();
                }

                if (text.IsDirty() || IsBoundTextDirty(drawNodeCached, text))
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);

                static bool IsBoundTextDirty(IDrawNode drawNodeCached, TextShapeViewModel text)
                {
                    var boundTextCheck = text.GetProperty(nameof(TextShapeViewModel.Text)) is string boundText ? boundText : text.Text;
                    return drawNodeCached is ITextDrawNode textDrawNode
                        && boundTextCheck != textDrawNode.BoundText;
                }
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateTextDrawNode(text, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(text, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawImage(object dc, ImageShapeViewModel image, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(image);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    style.Invalidate();
                }

                if (image.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreateImageDrawNode(image, style, _state.ImageCache, _biCache);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(image, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }

        public void DrawPath(object dc, PathShapeViewModel path, ShapeStyleViewModel style)
        {
            var drawNodeCached = _drawNodeCache.Get(path);
            if (drawNodeCached is { })
            {
                if (style.IsDirty() || drawNodeCached.Style != style)
                {
                    drawNodeCached.Style = style;
                    drawNodeCached.UpdateStyle();
                    style.Invalidate();
                }

                if (path.IsDirty())
                {
                    drawNodeCached.UpdateGeometry();
                }

                drawNodeCached.Draw(dc, _state.ZoomX);
            }
            else
            {
                var drawNode = _drawNodeFactory.CreatePathDrawNode(path, style);

                drawNode.UpdateStyle();

                _drawNodeCache.Set(path, drawNode);

                drawNode.Draw(dc, _state.ZoomX);
            }
        }
    }
}
