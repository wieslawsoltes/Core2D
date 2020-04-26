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
        public IPathShape ToPathShape(IEnumerable<IBaseShape> shapes)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var first = shapes.FirstOrDefault();
            var style = first.Style != null ?
                (IShapeStyle)first.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var path = PathGeometryConverter.ToSKPath(shapes, 0.0, 0.0, (value) => (float)value);
            var geometry = PathGeometryConverter.ToPathGeometry(path, 0.0, 0.0, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                first.IsStroked,
                first.IsFilled);
            return pathShape;
        }

        /// <inheritdoc/>
        public IPathShape ToPathShape(IBaseShape shape)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (IShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var path = PathGeometryConverter.ToSKPath(shape, 0.0, 0.0, (value) => (float)value);
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
            var path = PathGeometryConverter.ToSKPath(shape, 0.0, 0.0, (value) => (float)value);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (IShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            using var paint = SkiaSharpRenderer.ToSKPaintPen(style, (value) => (float)value, 96.0, 96.0, true);
            var result = paint.GetFillPath(path, 1.0f);
            if (result != null)
            {
                if (result.IsEmpty)
                {
                    result.Dispose();
                    return null;
                }
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
            var path = PathGeometryConverter.ToSKPath(shape, 0.0, 0.0, (value) => (float)value);
            if (path == null)
            {
                return null;
            }
            var factory = _serviceProvider.GetService<IFactory>();
            var style = shape.Style != null ?
                (IShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            using var paint = SkiaSharpRenderer.ToSKPaintBrush(style.Fill, true);
            var result = paint.GetFillPath(path, 1.0f);
            if (result != null)
            {
                if (result.IsEmpty)
                {
                    result.Dispose();
                    return null;
                }
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

            foreach (var s in shapes)
            {
                var path = PathGeometryConverter.ToSKPath(s, 0.0, 0.0, (value) => (float)value);
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
                (IShapeStyle)shape.Style?.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
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

        ///<inheritdoc/>
        public IEnumerable<IPathShape> Fragment(IEnumerable<IBaseShape> shapes)
        {
            if (shapes == null || shapes.Count() <= 0)
            {
                return null;
            }

            var paths = new List<SKPath>();

            foreach (var s in shapes)
            {
                var path = PathGeometryConverter.ToSKPath(s, 0.0, 0.0, (value) => (float)value);
                if (path != null)
                {
                    paths.Add(path);
                }
            }

            if (paths == null || paths.Count <= 1)
            {
                return null;
            }

            if (paths.Count == 2)
            {
                var results = new List<SKPath>();

                PathGeometryConverter.Op(paths[0], paths[1], SKPathOp.Difference, out var difference, out var haveResultDifference);
                if (haveResultDifference == true && difference != null && !difference.IsEmpty)
                {
                    results.Add(difference);
                }

                PathGeometryConverter.Op(paths[0], paths[1], SKPathOp.Intersect, out var intersect, out var haveResultIntersect);
                if (haveResultIntersect == true && intersect != null && !intersect.IsEmpty)
                {
                    results.Add(intersect);
                }

                PathGeometryConverter.Op(paths[0], paths[1], SKPathOp.ReverseDifference, out var reverseDifference, out var haveResultReverseDifference);
                if (haveResultReverseDifference == true && reverseDifference != null && !reverseDifference.IsEmpty)
                {
                    results.Add(reverseDifference);
                }

                if (results.Count > 0)
                {
                    var factory = _serviceProvider.GetService<IFactory>();
                    var shape = shapes.FirstOrDefault();
                    var pathFigures = new List<IPathFigure>();
                    var pathShapes = new List<IPathShape>();

                    //foreach (var result in results)
                    //{
                    //    var pathGeometry = PathGeometryConverter.ToPathGeometry(result, 0.0, 0.0, factory);
                    //    pathFigures.AddRange(pathGeometry.Figures);
                    //}

                    //foreach (var pathFigure in pathFigures)
                    //{
                    //    var geometry = factory.CreatePathGeometry(
                    //        ImmutableArray.Create<IPathFigure>(pathFigure),
                    //        paths[0].FillType == SKPathFillType.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero);
                    //    var style = shape.Style != null ?
                    //        (IShapeStyle)shape.Style?.Copy(null) :
                    //        factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                    //    var pathShape = factory.CreatePathShape(
                    //        "Path",
                    //        style,
                    //        geometry,
                    //        shape.IsStroked,
                    //        shape.IsFilled);
                    //    pathShapes.Add(pathShape);
                    //}

                    foreach (var result in results)
                    {
                        var style = shape.Style != null ?
                            (IShapeStyle)shape.Style?.Copy(null) :
                            factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                        var geometry = PathGeometryConverter.ToPathGeometry(result, 0.0, 0.0, factory);
                        var pathShape = factory.CreatePathShape(
                            "Path",
                            style,
                            geometry,
                            shape.IsStroked,
                            shape.IsFilled);
                        result.Dispose();
                        pathShapes.Add(pathShape);
                    }

                    return pathShapes;
                }
            }

            return null;
        }
    }
}
