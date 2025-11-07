// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;

namespace Core2D.Spatial;

/// <summary>
/// Spatial hash grid for quickly querying axis-aligned bounding boxes.
/// </summary>
/// <typeparam name="T">Stored payload type.</typeparam>
public sealed class SpatialHashGrid<T>
{
    private readonly double _cellSize;
    private readonly List<SpatialHashHit<T>> _entries = new();
    private readonly Dictionary<(int X, int Y), List<int>> _cells = new();
    private readonly List<T> _fallback = new();

    public SpatialHashGrid(double cellSize)
    {
        if (cellSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cellSize));
        }

        _cellSize = cellSize;
    }

    /// <summary>
    /// Shapes that could not be indexed because of missing bounds.
    /// </summary>
    public IEnumerable<T> Fallback => _fallback;

    public void Add(T item, Rect2 bounds)
    {
        var index = _entries.Count;
        _entries.Add(new SpatialHashHit<T>(item, bounds));

        var minX = ToCell(bounds.Left);
        var maxX = ToCell(bounds.Right);
        var minY = ToCell(bounds.Top);
        var maxY = ToCell(bounds.Bottom);

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var key = (x, y);
                if (!_cells.TryGetValue(key, out var bucket))
                {
                    bucket = new List<int>();
                    _cells.Add(key, bucket);
                }
                bucket.Add(index);
            }
        }
    }

    public void AddWithoutBounds(T item)
    {
        _fallback.Add(item);
    }

    public IEnumerable<SpatialHashHit<T>> Query(Rect2 area)
    {
        if (_entries.Count == 0)
        {
            yield break;
        }

        var minX = ToCell(area.Left);
        var maxX = ToCell(area.Right);
        var minY = ToCell(area.Top);
        var maxY = ToCell(area.Bottom);
        var candidateIndices = new SortedSet<int>();

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                if (_cells.TryGetValue((x, y), out var bucket))
                {
                    foreach (var index in bucket)
                    {
                        candidateIndices.Add(index);
                    }
                }
            }
        }

        foreach (var index in candidateIndices)
        {
            var entry = _entries[index];
            if (entry.Bounds.IntersectsWith(area))
            {
                yield return entry;
            }
        }
    }

    private int ToCell(double coordinate)
    {
        return (int)Math.Floor(coordinate / _cellSize);
    }
}

public readonly record struct SpatialHashHit<T>(T Item, Rect2 Bounds);
