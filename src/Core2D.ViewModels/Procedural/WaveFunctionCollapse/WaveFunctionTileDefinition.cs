// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Procedural.WaveFunctionCollapse;

[Flags]
public enum WaveFunctionConnector
{
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8,
    All = North | East | South | West
}

public enum WaveFunctionDirection
{
    West = 0,
    South = 1,
    East = 2,
    North = 3
}

[Flags]
public enum WaveFunctionTileFlags
{
    None = 0,
    Wildcard = 1
}

public static class WaveFunctionDirectionExtensions
{
    public static WaveFunctionDirection Opposite(this WaveFunctionDirection direction) => direction switch
    {
        WaveFunctionDirection.West => WaveFunctionDirection.East,
        WaveFunctionDirection.East => WaveFunctionDirection.West,
        WaveFunctionDirection.North => WaveFunctionDirection.South,
        WaveFunctionDirection.South => WaveFunctionDirection.North,
        _ => WaveFunctionDirection.West
    };

    public static bool HasConnector(this WaveFunctionConnector connectors, WaveFunctionDirection direction) => direction switch
    {
        WaveFunctionDirection.North => connectors.HasFlag(WaveFunctionConnector.North),
        WaveFunctionDirection.East => connectors.HasFlag(WaveFunctionConnector.East),
        WaveFunctionDirection.South => connectors.HasFlag(WaveFunctionConnector.South),
        WaveFunctionDirection.West => connectors.HasFlag(WaveFunctionConnector.West),
        _ => false
    };
}

public sealed record WaveFunctionTileDefinition(
    string Id,
    WaveFunctionConnector Connectors,
    double Weight,
    Func<WaveFunctionTileBuilderContext, IEnumerable<BaseShapeViewModel>> Builder,
    WaveFunctionTileFlags Flags = WaveFunctionTileFlags.None);

public sealed class WaveFunctionTileBuilderContext
{
    public WaveFunctionTileBuilderContext(
        IViewModelFactory factory,
        WaveFunctionPreset preset,
        WaveFunctionTileDefinition tile,
        WaveFunctionStyleCache styleCache,
        Random random,
        int column,
        int row,
        double cellSize,
        double padding,
        double strokeThickness)
    {
        Factory = factory;
        Preset = preset;
        Tile = tile;
        Styles = styleCache;
        Random = random;
        Column = column;
        Row = row;
        CellSize = cellSize;
        Padding = padding;
        StrokeThickness = strokeThickness;
        Left = column * cellSize;
        Top = row * cellSize;
        Right = Left + cellSize;
        Bottom = Top + cellSize;
        CenterX = Left + cellSize / 2.0;
        CenterY = Top + cellSize / 2.0;
    }

    public IViewModelFactory Factory { get; }

    public WaveFunctionPreset Preset { get; }

    public WaveFunctionTileDefinition Tile { get; }

    public WaveFunctionStyleCache Styles { get; }

    public Random Random { get; }

    public int Column { get; }

    public int Row { get; }

    public double CellSize { get; }

    public double Padding { get; }

    public double StrokeThickness { get; }

    public double Left { get; }

    public double Top { get; }

    public double Right { get; }

    public double Bottom { get; }

    public double CenterX { get; }

    public double CenterY { get; }

    public double InnerWidth => CellSize - 2.0 * Padding;

    public double InnerHeight => CellSize - 2.0 * Padding;
}

public sealed class WaveFunctionStyleCache
{
    private readonly Dictionary<string, ShapeStyleViewModel> _styles = new(StringComparer.Ordinal);

    public ShapeStyleViewModel GetOrCreate(string key, Func<ShapeStyleViewModel> factory)
    {
        if (!_styles.TryGetValue(key, out var style))
        {
            style = factory();
            _styles[key] = style;
        }

        return style;
    }
}
