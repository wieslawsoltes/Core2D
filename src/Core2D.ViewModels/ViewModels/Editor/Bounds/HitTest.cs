// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds;

public class HitTest : IHitTest
{
    private const double SpatialCellSize = 256.0;
    private readonly List<PointShapeViewModel> _pointsBuffer = new();

    public IDictionary<Type, IBounds> Registered { get; }

    public HitTest(IServiceProvider? serviceProvider)
    {
        Registered = new Dictionary<Type, IBounds>();

        var bounds = serviceProvider.GetService<IBounds[]>();
        if (bounds is { })
        {
            Register(bounds);
        }
    }

    public void Register(IBounds hitTest)
    {
        Registered.Add(hitTest.TargetType, hitTest);
    }

    public void Register(IEnumerable<IBounds> hitTests)
    {
        foreach (var hitTest in hitTests)
        {
            Registered.Add(hitTest.TargetType, hitTest);
        }
    }

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale)
    {
        return Registered[shape.TargetType].TryToGetPoint(shape, target, radius, scale, Registered);
    }

    public PointShapeViewModel? TryToGetPoint(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale)
    {
        var shapeList = Materialize(shapes);
        if (shapeList.Count == 0)
        {
            return null;
        }

        var queryRect = CreatePointRect(target, radius);
        var index = BuildSpatialIndex(shapeList, scale);

        foreach (var candidate in index.Query(queryRect))
        {
            var result = TryToGetPoint(candidate.Item, target, radius, scale);
            if (result is { })
            {
                return result;
            }
        }

        foreach (var fallback in index.Fallback)
        {
            var result = TryToGetPoint(fallback, target, radius, scale);
            if (result is { })
            {
                return result;
            }
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale)
    {
        return Registered[shape.TargetType].Contains(shape, target, radius, scale, Registered);
    }

    public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale)
    {
        return Registered[shape.TargetType].Overlaps(shape, target, radius, scale, Registered);
    }

    public BaseShapeViewModel? TryToGetShape(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale)
    {
        var shapeList = Materialize(shapes);
        if (shapeList.Count == 0)
        {
            return null;
        }

        var queryRect = CreatePointRect(target, radius);
        var index = BuildSpatialIndex(shapeList, scale);

        foreach (var candidate in index.Query(queryRect))
        {
            if (Registered[candidate.Item.TargetType].Contains(candidate.Item, target, radius, scale, Registered))
            {
                return candidate.Item;
            }
        }

        foreach (var fallback in index.Fallback)
        {
            if (Registered[fallback.TargetType].Contains(fallback, target, radius, scale, Registered))
            {
                return fallback;
            }
        }

        return null;
    }

    public ISet<BaseShapeViewModel>? TryToGetShapes(IEnumerable<BaseShapeViewModel> shapes, Rect2 target, double radius, double scale)
    {
        var shapeList = Materialize(shapes);
        if (shapeList.Count == 0)
        {
            return null;
        }

        var index = BuildSpatialIndex(shapeList, scale);
        var selected = new HashSet<BaseShapeViewModel>();

        foreach (var candidate in index.Query(target))
        {
            var result = Registered[candidate.Item.TargetType].Overlaps(candidate.Item, target, radius, scale, Registered);
            if (result)
            {
                selected.Add(candidate.Item);
            }
        }

        foreach (var fallback in index.Fallback)
        {
            var result = Registered[fallback.TargetType].Overlaps(fallback, target, radius, scale, Registered);
            if (result)
            {
                selected.Add(fallback);
            }
        }

        return selected.Count > 0 ? selected : null;
    }

    private static List<BaseShapeViewModel> Materialize(IEnumerable<BaseShapeViewModel> shapes)
    {
        return shapes as List<BaseShapeViewModel> ?? shapes.ToList();
    }

    private SpatialHashGrid<BaseShapeViewModel> BuildSpatialIndex(IEnumerable<BaseShapeViewModel> shapes, double scale)
    {
        var index = new SpatialHashGrid<BaseShapeViewModel>(SpatialCellSize);
        foreach (var shape in shapes)
        {
            if (TryCreateBounds(shape, scale, out var bounds))
            {
                index.Add(shape, bounds);
            }
            else
            {
                index.AddWithoutBounds(shape);
            }
        }
        return index;
    }

    private Rect2 CreatePointRect(Point2 point, double radius)
    {
        var size = radius * 2.0;
        return new Rect2(point.X - radius, point.Y - radius, size, size);
    }

    private bool TryCreateBounds(BaseShapeViewModel shape, double scale, out Rect2 bounds)
    {
        _pointsBuffer.Clear();

        try
        {
            shape.GetPoints(_pointsBuffer);
        }
        catch (NotImplementedException)
        {
            bounds = default;
            return false;
        }

        if (_pointsBuffer.Count == 0)
        {
            bounds = default;
            return false;
        }

        double minX = double.PositiveInfinity;
        double minY = double.PositiveInfinity;
        double maxX = double.NegativeInfinity;
        double maxY = double.NegativeInfinity;

        foreach (var point in _pointsBuffer)
        {
            minX = Math.Min(minX, point.X);
            minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X);
            maxY = Math.Max(maxY, point.Y);
        }

        if (double.IsInfinity(minX) || double.IsInfinity(minY) || double.IsInfinity(maxX) || double.IsInfinity(maxY))
        {
            bounds = default;
            return false;
        }

        var rect = Rect2.FromPoints(minX, minY, maxX, maxY);

        if (shape.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            rect = HitTestHelper.Inflate(ref rect, scale);
        }

        bounds = rect;
        return true;
    }

}
