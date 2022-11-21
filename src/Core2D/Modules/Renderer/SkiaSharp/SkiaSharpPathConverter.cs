#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp;

public class SkiaSharpPathConverter : IPathConverter
{
    private readonly IServiceProvider? _serviceProvider;

    public SkiaSharpPathConverter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public PathShapeViewModel? ToPathShape(IEnumerable<BaseShapeViewModel>? shapes)
    {
        if (shapes is null)
        {
            return null;
        }
        var path = PathGeometryConverter.ToSKPath(shapes);
        if (path is null)
        {
            return null;
        }
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var first = shapes.FirstOrDefault();
        if (first is null)
        {
            return null;
        }
        var style = first.Style is { } ?
            first.Style?.CopyShared(null) :
            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var pathShape = PathGeometryConverter.ToPathGeometry(path, factory);
        pathShape.Name = "Path";
        pathShape.Style = style;
        pathShape.IsStroked = first.IsStroked;
        pathShape.IsFilled = first.IsFilled;
        return pathShape;
    }

    public PathShapeViewModel? ToPathShape(BaseShapeViewModel? shape)
    {
        if (shape is null)
        {
            return null;
        }
        var path = PathGeometryConverter.ToSKPath(shape);
        if (path is null)
        {
            return null;
        }
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var style = shape.Style is { } ?
            shape.Style?.CopyShared(null) :
            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var pathShape = PathGeometryConverter.ToPathGeometry(path, factory);
        pathShape.Name = "Path";
        pathShape.Style = style;
        pathShape.IsStroked = shape.IsStroked;
        pathShape.IsFilled = shape.IsFilled;
        return pathShape;
    }

    public PathShapeViewModel? ToStrokePathShape(BaseShapeViewModel? shape)
    {
        if (shape is null)
        {
            return null;
        }
        var path = PathGeometryConverter.ToSKPath(shape);
        if (path is null)
        {
            return null;
        }
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var style = shape.Style is { } ?
            shape.Style?.CopyShared(null) :
            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        if (style is null)
        {
            return null;
        }
        var stroke = style.Stroke?.Color.CopyShared(null);
        var fill = style.Fill?.Color.CopyShared(null);
        if (style.Stroke is { })
        {
            style.Stroke.Color = fill;
        }
        if (style.Fill is { })
        {
            style.Fill.Color = stroke;
        }
        using var pen = SkiaSharpDrawUtil.ToSKPaintPen(style, style.Stroke?.Thickness ?? 1d);
        var result = pen.GetFillPath(path, 1.0f);
        if (result is { })
        {
            if (result.IsEmpty)
            {
                result.Dispose();
                return null;
            }
            var pathShape = PathGeometryConverter.ToPathGeometry(result, factory);
            pathShape.Name = "Path";
            pathShape.Style = style;
            pathShape.IsStroked = true;
            pathShape.IsFilled = false;
            result.Dispose();
            return pathShape;
        }
        return null;
    }

    public PathShapeViewModel? ToFillPathShape(BaseShapeViewModel? shape)
    {
        if (shape is null)
        {
            return null;
        }
        var path = PathGeometryConverter.ToSKPath(shape);
        if (path is null)
        {
            return null;
        }
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var style = shape.Style is { } ?
            shape.Style?.CopyShared(null) :
            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        if (style?.Fill?.Color is null)
        {
            return null;
        }
        using var brush = SkiaSharpDrawUtil.ToSKPaintBrush(style.Fill.Color);
        var result = brush.GetFillPath(path, 1.0f);
        if (result is { })
        {
            if (result.IsEmpty)
            {
                result.Dispose();
                return null;
            }
            var pathShape = PathGeometryConverter.ToPathGeometry(result, factory);
            pathShape.Name = "Path";
            pathShape.Style = style;
            pathShape.IsStroked = false;
            pathShape.IsFilled = true;
            result.Dispose();
            return pathShape;
        }
        return null;
    }

    public PathShapeViewModel? ToWindingPathShape(BaseShapeViewModel? shape)
    {
        if (shape is null)
        {
            return null;
        }
        var path = PathGeometryConverter.ToSKPath(shape);
        if (path is null)
        {
            return null;
        }
        var result = path.ToWinding();
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var style = shape.Style is { } ?
            shape.Style?.CopyShared(null) :
            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var pathShape = PathGeometryConverter.ToPathGeometry(result, factory);
        pathShape.Name = "Path";
        pathShape.Style = style;
        pathShape.IsStroked = shape.IsStroked;
        pathShape.IsFilled = shape.IsFilled;
        result.Dispose();
        return pathShape;
    }

    public PathShapeViewModel? Simplify(BaseShapeViewModel? shape)
    {
        if (shape is null)
        {
            return null;
        }
        var path = PathGeometryConverter.ToSKPath(shape)?.Simplify();
        if (path is null)
        {
            return null;
        }
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var style = shape.Style is { } ?
            shape.Style?.CopyShared(null) :
            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var pathShape = PathGeometryConverter.ToPathGeometry(path, factory);
        pathShape.Name = "Path";
        pathShape.Style = style;
        pathShape.IsStroked = shape.IsStroked;
        pathShape.IsFilled = shape.IsFilled;
        return pathShape;
    }

    public PathShapeViewModel? Op(IEnumerable<BaseShapeViewModel>? shapes, PathOp op)
    {
        if (shapes is null || !shapes.Any())
        {
            return null;
        }

        var paths = new List<SKPath>();

        foreach (var s in shapes)
        {
            var path = PathGeometryConverter.ToSKPath(s);
            if (path is { })
            {
                paths.Add(path);
            }
        }

        if (paths.Count <= 0)
        {
            return null;
        }

        PathGeometryConverter.Op(paths, PathGeometryConverter.ToSKPathOp(op), out var result, out var haveResult);
        if (haveResult == false || result is null || result.IsEmpty)
        {
            return null;
        }

        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var shape = shapes.FirstOrDefault();
        var style = shape.Style is { } ?
            shape.Style?.CopyShared(null) :
            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var pathShape = PathGeometryConverter.ToPathGeometry(result, factory);
        pathShape.Name = "Path";
        pathShape.Style = style;
        pathShape.IsStroked = shape.IsStroked;
        pathShape.IsFilled = shape.IsFilled;
        result.Dispose();
        return pathShape;
    }

    public PathShapeViewModel? FromSvgPathData(string? svgPath, bool isStroked, bool isFilled)
    {
        if (svgPath is null)
        {
            return null;
        }
        var path = SKPath.ParseSvgPathData(svgPath);
        if (path is null)
        {
            return null;
        }
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return null;
        }
        var style = factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var pathShape = PathGeometryConverter.ToPathGeometry(path, factory);
        pathShape.Name = "Path";
        pathShape.Style = style;
        pathShape.IsStroked = isStroked;
        pathShape.IsFilled = isFilled;
        return pathShape;
    }

    public string? ToSvgPathData(BaseShapeViewModel? shape)
    {
        if (shape is null)
        {
            return null;
        }
        var path = PathGeometryConverter.ToSKPath(shape);
        return path?.ToSvgPathData();
    }
}
