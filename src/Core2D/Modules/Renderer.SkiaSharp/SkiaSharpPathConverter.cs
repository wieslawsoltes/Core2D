using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D;
using Core2D.Editor;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// Path converter.
    /// </summary>
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

        /// <inheritdoc/>
        public PathShape ToPathShape(IEnumerable<BaseShape> shapes)
        {
            var path = PathGeometryConverter.ToSKPath(shapes);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var first = shapes.FirstOrDefault();
            var style = first.Style != null ?
                (ShapeStyle)first.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                first.IsStroked,
                first.IsFilled);
            return pathShape;
        }

        /// <inheritdoc/>
        public PathShape ToPathShape(BaseShape shape)
        {
            var path = PathGeometryConverter.ToSKPath(shape);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (ShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shape.IsStroked,
                shape.IsFilled);
            return pathShape;
        }

        /// <inheritdoc/>
        public PathShape ToStrokePathShape(BaseShape shape)
        {
            var path = PathGeometryConverter.ToSKPath(shape);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (ShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var stroke = (BaseColor)style.Stroke.Copy(null);
            var fill = (BaseColor)style.Fill.Copy(null);
            style.Stroke = fill;
            style.Fill = stroke;
            using var pen = SkiaSharpDrawUtil.ToSKPaintPen(style, style.Thickness);
            var result = pen.GetFillPath(path, 1.0f);
            if (result != null)
            {
                if (result.IsEmpty)
                {
                    result.Dispose();
                    return null;
                }
                var geometry = PathGeometryConverter.ToPathGeometry(result, factory);
                var pathShape = factory.CreatePathShape(
                    "Path",
                    style,
                    geometry,
                    true,
                    false);
                result.Dispose();
                return pathShape;
            }
            return null;
        }

        /// <inheritdoc/>
        public PathShape ToFillPathShape(BaseShape shape)
        {
            var path = PathGeometryConverter.ToSKPath(shape);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (ShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            using var brush = SkiaSharpDrawUtil.ToSKPaintBrush(style.Fill);
            var result = brush.GetFillPath(path, 1.0f);
            if (result != null)
            {
                if (result.IsEmpty)
                {
                    result.Dispose();
                    return null;
                }
                var geometry = PathGeometryConverter.ToPathGeometry(result, factory);
                var pathShape = factory.CreatePathShape(
                    "Path",
                    style,
                    geometry,
                    false,
                    true);
                result.Dispose();
                return pathShape;
            }
            return null;
        }

        /// <inheritdoc/>
        public PathShape ToWindingPathShape(BaseShape shape)
        {
            var path = PathGeometryConverter.ToSKPath(shape);
            if (path == null)
            {
                return null;
            }
            var result = path.ToWinding();
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (ShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(result, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shape.IsStroked,
                shape.IsFilled);
            return pathShape;
        }

        /// <inheritdoc/>
        public PathShape Simplify(BaseShape shape)
        {
            var path = PathGeometryConverter.ToSKPath(shape)?.Simplify();
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (ShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shape.IsStroked,
                shape.IsFilled);
            return pathShape;
        }

        /// <inheritdoc/>
        public PathShape Op(IEnumerable<BaseShape> shapes, PathOp op)
        {
            if (shapes == null || shapes.Count() <= 0)
            {
                return null;
            }

            var paths = new List<SKPath>();

            foreach (var s in shapes)
            {
                var path = PathGeometryConverter.ToSKPath(s);
                if (path != null)
                {
                    paths.Add(path);
                }
            }

            if (paths == null || paths.Count <= 0)
            {
                return null;
            }

            PathGeometryConverter.Op(paths, PathGeometryConverter.ToSKPathOp(op), out var result, out var haveResult);
            if (haveResult == false || result == null || result.IsEmpty)
            {
                return null;
            }

            var factory = _serviceProvider.GetService<IFactory>();
            var shape = shapes.FirstOrDefault();
            var style = shape.Style != null ?
                (ShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(result, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shape.IsStroked,
                shape.IsFilled);
            result.Dispose();
            return pathShape;
        }

        /// <inheritdoc/>
        public PathShape FromSvgPathData(string svgPath, bool isStroked, bool isFilled)
        {
            var path = SKPath.ParseSvgPathData(svgPath);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                isStroked,
                isFilled);
            return pathShape;
        }

        /// <inheritdoc/>
        public string ToSvgPathData(BaseShape shape)
        {
            var path = PathGeometryConverter.ToSKPath(shape);
            if (path == null)
            {
                return null;
            }
            return path.ToSvgPathData();
        }
    }
}
