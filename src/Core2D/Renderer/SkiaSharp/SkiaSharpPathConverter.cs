using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    public class SkiaSharpPathConverter : IPathConverter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpPathConverter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SkiaSharpPathConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private SKPathOp ToSKPathOp(PathOp op)
        {
            return op switch
            {
                PathOp.Intersect => SKPathOp.Intersect,
                PathOp.Union => SKPathOp.Union,
                PathOp.Xor => SKPathOp.Xor,
                PathOp.ReverseDifference => SKPathOp.ReverseDifference,
                _ => SKPathOp.Difference,
            };
        }

        private SKPath ToSKPath(IBaseShape shape, double dx, double dy, Func<double, float> scale)
        {
            return shape switch
            {
                ILineShape line => PathGeometryConverter.ToSKPath(line, dx, dy, scale),
                IRectangleShape rectangle => PathGeometryConverter.ToSKPath(rectangle, dx, dy, scale),
                IEllipseShape ellipse => PathGeometryConverter.ToSKPath(ellipse, dx, dy, scale),
                IArcShape arc => PathGeometryConverter.ToSKPath(arc, dx, dy, scale),
                ICubicBezierShape cubicBezier => PathGeometryConverter.ToSKPath(cubicBezier, dx, dy, scale),
                IQuadraticBezierShape quadraticBezier => PathGeometryConverter.ToSKPath(quadraticBezier, dx, dy, scale),
                ITextShape text => PathGeometryConverter.ToSKPath(text, dx, dy, scale),
                IPathShape path => PathGeometryConverter.ToSKPath(path, dx, dy, scale),
                _ => null,
            };
        }

        /// <inheritdoc/>
        public IPathShape ToPathShape(IBaseShape shape)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var style = (IShapeStyle)shape.Style.Copy(null);
            var path = ToSKPath(shape, 0.0, 0.0, (value) => (float)value);
            var geometry = PathGeometryConverter.ToPathGeometry(path, 0.0, 0.0, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shape.IsStroked,
                shape.IsFilled);
            return pathShape;
        }

        /// <inheritdoc/>
        public IPathShape ToStrokePathShape(IBaseShape shape)
        {
            var path = ToSKPath(shape, 0.0, 0.0, (value) => (float)value);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = (IShapeStyle)shape.Style.Copy(null);
            using var paint = SkiaSharpRenderer.ToSKPaintPen(shape.Style, (value) => (float)value, 96.0, 72.0, true);
            var result = paint.GetFillPath(path, 1.0f);
            if (result != null)
            {
                var geometry = PathGeometryConverter.ToPathGeometry(result, 0.0, 0.0, factory);
                var pathShape = factory.CreatePathShape(
                    "Path",
                    style,
                    geometry,
                    shape.IsStroked,
                    shape.IsFilled);
                result.Dispose();
                return pathShape;
            }
            return null;
        }

        /// <inheritdoc/>
        public IPathShape ToFillPathShape(IBaseShape shape)
        {
            var path = ToSKPath(shape, 0.0, 0.0, (value) => (float)value);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = (IShapeStyle)shape.Style.Copy(null);
            using var paint = SkiaSharpRenderer.ToSKPaintBrush(shape.Style.Fill, true);
            var result = paint.GetFillPath(path, 1.0f);
            if (result != null)
            {
                var geometry = PathGeometryConverter.ToPathGeometry(result, 0.0, 0.0, factory);
                var pathShape = factory.CreatePathShape(
                    "Path",
                    style,
                    geometry,
                    shape.IsStroked,
                    shape.IsFilled);
                result.Dispose();
                return pathShape;
            }
            return null;
        }

        /// <inheritdoc/>
        public IPathShape Op(IEnumerable<IBaseShape> shapes, PathOp op)
        {
            if (shapes == null || shapes.Count() <= 0)
            {
                return null;
            }

            var paths = new List<SKPath>();

            foreach (var shape in shapes)
            {
                var path = ToSKPath(shape, 0.0, 0.0, (value) => (float)value);
                if (path != null)
                {
                    paths.Add(path);
                }
            }

            if (paths == null || paths.Count <= 0)
            {
                return null;
            }

            var pathOp = ToSKPathOp(op);
            var haveResult = false;
            var result = new SKPath(paths[0]) { FillType = paths[0].FillType };

            if (paths.Count == 1)
            {
                using var empty = new SKPath() { FillType = paths[0].FillType };
                result = empty.Op(paths[0], pathOp);
                haveResult = true;
            }
            else
            {
                for (int i = 1; i < paths.Count; i++)
                {
                    var next = result.Op(paths[i], pathOp);
                    if (next != null)
                    {
                        result.Dispose();
                        result = next;
                        haveResult = true;
                    }
                }
            }

            if (haveResult)
            {
                var factory = _serviceProvider.GetService<IFactory>();
                var shape = shapes.FirstOrDefault();
                var style = (IShapeStyle)shape.Style.Copy(null);
                var geometry = PathGeometryConverter.ToPathGeometry(result, 0.0, 0.0, factory);
                var pathShape = factory.CreatePathShape(
                    "Path",
                    style,
                    geometry,
                    shape.IsStroked,
                    shape.IsFilled);
                result.Dispose();
                return pathShape;
            }

            return null;
        }
    }
}
