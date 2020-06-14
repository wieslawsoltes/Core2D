using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    internal class ShapeEditor : IShapeEditor
    {
        private readonly IServiceProvider _serviceProvider;

        public ShapeEditor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void BreakPathFigure(IPathFigure pathFigure, IShapeStyle style, bool isStroked, bool isFilled, List<IBaseShape> result)
        {
            var factory = _serviceProvider.GetService<IFactory>();

            var firstPoint = pathFigure.StartPoint;
            var lastPoint = pathFigure.StartPoint;

            foreach (var segment in pathFigure.Segments)
            {
                switch (segment)
                {
                    case ILineSegment lineSegment:
                        {
                            var convertedStyle = style != null ?
                                (IShapeStyle)style?.Copy(null) :
                                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);

                            var convertedPathShape = factory.CreateLineShape(
                                lastPoint,
                                lineSegment.Point,
                                convertedStyle,
                                isStroked);

                            lastPoint = lineSegment.Point;

                            result.Add(convertedPathShape);
                        }
                        break;
                    case IQuadraticBezierSegment quadraticBezierSegment:
                        {
                            var convertedStyle = style != null ?
                                (IShapeStyle)style?.Copy(null) :
                                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);

                            var convertedPathShape = factory.CreateQuadraticBezierShape(
                                lastPoint,
                                quadraticBezierSegment.Point1,
                                quadraticBezierSegment.Point2,
                                convertedStyle,
                                isStroked,
                                isFilled);

                            lastPoint = quadraticBezierSegment.Point2;

                            result.Add(convertedPathShape);
                        }
                        break;
                    case ICubicBezierSegment cubicBezierSegment:
                        {
                            var convertedStyle = style != null ?
                                (IShapeStyle)style?.Copy(null) :
                                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);

                            var convertedPathShape = factory.CreateCubicBezierShape(
                                lastPoint,
                                cubicBezierSegment.Point1,
                                cubicBezierSegment.Point2,
                                cubicBezierSegment.Point3,
                                convertedStyle,
                                isStroked,
                                isFilled);

                            lastPoint = cubicBezierSegment.Point3;

                            result.Add(convertedPathShape);
                        }
                        break;
                    case IArcSegment arcSegment:
                        {
                            var convertedStyle = style != null ?
                                (IShapeStyle)style?.Copy(null) :
                                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);

                            var point2 = factory.CreatePointShape(0, 0); // TODO:

                            var point3 = factory.CreatePointShape(0, 0); // TODO:

                            var convertedPathShape = factory.CreateArcShape(
                                lastPoint,
                                point2,
                                point3,
                                arcSegment.Point,
                                convertedStyle,
                                isStroked,
                                isFilled);

                            lastPoint = arcSegment.Point;

                            result.Add(convertedPathShape);
                        }
                        break;
                }
            }

            if (pathFigure.Segments.Length > 0 && pathFigure.IsClosed)
            {
                var convertedStyle = style != null ?
                    (IShapeStyle)style?.Copy(null) :
                    factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);

                var convertedPathShape = factory.CreateLineShape(
                    lastPoint,
                    firstPoint,
                    convertedStyle,
                    isStroked);

                result.Add(convertedPathShape);
            }
        }

        public bool BreakPathShape(IPathShape pathShape, List<IBaseShape> result)
        {
            var factory = _serviceProvider.GetService<IFactory>();

            if (pathShape.Geometry.Figures.Length == 1)
            {
                BreakPathFigure(pathShape.Geometry.Figures[0], pathShape.Style, pathShape.IsStroked, pathShape.IsFilled, result);
                return true;
            }
            else if (pathShape.Geometry.Figures.Length > 1)
            {
                foreach (var pathFigure in pathShape.Geometry.Figures)
                {
                    var style = pathShape.Style != null ?
                        (IShapeStyle)pathShape.Style?.Copy(null) :
                        factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);

                    var convertedGeometry = factory.CreatePathGeometry(ImmutableArray.Create<IPathFigure>(), pathShape.Geometry.FillRule);
                    convertedGeometry.Figures = convertedGeometry.Figures.Add(pathFigure);

                    var convertedPathShape = factory.CreatePathShape(
                        pathShape.Name,
                        style,
                        convertedGeometry,
                        pathShape.IsStroked,
                        pathShape.IsFilled);

                    result.Add(convertedPathShape);
                }

                return true;
            }

            return false;
        }

        public void BreakShape(IBaseShape shape, List<IBaseShape> result, List<IBaseShape> remove)
        {
            switch (shape)
            {
                case IPathShape pathShape:
                    {
                        if (BreakPathShape(pathShape, result) == true)
                        {
                            remove.Add(pathShape);
                        }
                    }
                    break;
                case IGroupShape groupShape:
                    {
                        if (groupShape.Shapes.Length > 0)
                        {
                            var groupShapes = new List<IBaseShape>();

                            GroupShapeExtensions.Ungroup(groupShape.Shapes, groupShapes);

                            foreach (var brokenGroupShape in groupShapes)
                            {
                                BreakShape(brokenGroupShape, result, remove);
                            }

                            remove.Add(groupShape);
                        }
                    }
                    break;
                default:
                    {
                        var pathConverter = _serviceProvider.GetService<IPathConverter>();
                        var path = pathConverter?.ToPathShape(shape);
                        if (path != null)
                        {
                            BreakShape(path, result, remove);
                            remove.Add(shape);
                        }
                    }
                    break;
            }
        }
    }
}
