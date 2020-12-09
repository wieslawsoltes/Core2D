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
    public class SkiaSharpPathConverter : IPathConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public SkiaSharpPathConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public PathShapeViewModel ToPathShape(IEnumerable<BaseShapeViewModel> shapes)
        {
            var path = PathGeometryConverter.ToSKPath(shapes);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var first = shapes.FirstOrDefault();
            var style = first.StyleViewModel != null ?
                (ShapeStyleViewModel)first.StyleViewModel?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                first.IsStroked,
                first.IsFilled);
            return pathShape;
        }

        public PathShapeViewModel ToPathShape(BaseShapeViewModel shapeViewModel)
        {
            var path = PathGeometryConverter.ToSKPath(shapeViewModel);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shapeViewModel.StyleViewModel != null ?
                (ShapeStyleViewModel)shapeViewModel.StyleViewModel?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shapeViewModel.IsStroked,
                shapeViewModel.IsFilled);
            return pathShape;
        }

        public PathShapeViewModel ToStrokePathShape(BaseShapeViewModel shapeViewModel)
        {
            var path = PathGeometryConverter.ToSKPath(shapeViewModel);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shapeViewModel.StyleViewModel != null ?
                (ShapeStyleViewModel)shapeViewModel.StyleViewModel?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var stroke = (BaseColorViewModel)style.Stroke.ColorViewModel.Copy(null);
            var fill = (BaseColorViewModel)style.Fill.ColorViewModel.Copy(null);
            style.Stroke.ColorViewModel = fill;
            style.Fill.ColorViewModel = stroke;
            using var pen = SkiaSharpDrawUtil.ToSKPaintPen(style, style.Stroke.Thickness);
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

        public PathShapeViewModel ToFillPathShape(BaseShapeViewModel shapeViewModel)
        {
            var path = PathGeometryConverter.ToSKPath(shapeViewModel);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shapeViewModel.StyleViewModel != null ?
                (ShapeStyleViewModel)shapeViewModel.StyleViewModel?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            using var brush = SkiaSharpDrawUtil.ToSKPaintBrush(style.Fill.ColorViewModel);
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

        public PathShapeViewModel ToWindingPathShape(BaseShapeViewModel shapeViewModel)
        {
            var path = PathGeometryConverter.ToSKPath(shapeViewModel);
            if (path == null)
            {
                return null;
            }
            var result = path.ToWinding();
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shapeViewModel.StyleViewModel != null ?
                (ShapeStyleViewModel)shapeViewModel.StyleViewModel?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(result, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shapeViewModel.IsStroked,
                shapeViewModel.IsFilled);
            return pathShape;
        }

        public PathShapeViewModel Simplify(BaseShapeViewModel shapeViewModel)
        {
            var path = PathGeometryConverter.ToSKPath(shapeViewModel)?.Simplify();
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shapeViewModel.StyleViewModel != null ?
                (ShapeStyleViewModel)shapeViewModel.StyleViewModel?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                shapeViewModel.IsStroked,
                shapeViewModel.IsFilled);
            return pathShape;
        }

        public PathShapeViewModel Op(IEnumerable<BaseShapeViewModel> shapes, PathOp op)
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
            var style = shape.StyleViewModel != null ?
                (ShapeStyleViewModel)shape.StyleViewModel?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
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

        public PathShapeViewModel FromSvgPathData(string svgPath, bool isStroked, bool isFilled)
        {
            var path = SKPath.ParseSvgPathData(svgPath);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                isStroked,
                isFilled);
            return pathShape;
        }

        public string ToSvgPathData(BaseShapeViewModel shapeViewModel)
        {
            var path = PathGeometryConverter.ToSKPath(shapeViewModel);
            return path?.ToSvgPathData();
        }
    }
}
