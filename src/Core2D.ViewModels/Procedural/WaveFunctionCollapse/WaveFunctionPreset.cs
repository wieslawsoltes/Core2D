// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.Procedural.WaveFunctionCollapse;

public sealed record WaveFunctionPreset(
    string Name,
    string Description,
    int DefaultColumns,
    int DefaultRows,
    double CellSize,
    double Padding,
    bool Periodic,
    WaveFunctionHeuristicType Heuristic,
    IReadOnlyList<WaveFunctionTileDefinition> Tiles)
{
    public int MaxAttempts { get; init; } = 16;

    public double StrokeThickness { get; init; } = 3.0;

    public int PropagationLimitMultiplier { get; init; } = 8;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentException("Preset name must be provided", nameof(Name));
        }

        if (Tiles.Count == 0)
        {
            throw new ArgumentException("Preset must define at least one tile", nameof(Tiles));
        }

        if (DefaultColumns <= 0 || DefaultRows <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(DefaultColumns), "Columns and rows must be greater than zero.");
        }
    }
}

internal sealed record WaveFunctionRunRequest(
    WaveFunctionPreset Preset,
    int Columns,
    int Rows,
    double CellSize,
    double Padding,
    bool Periodic,
    WaveFunctionHeuristicType Heuristic,
    int Seed,
    int Limit,
    double StrokeThickness);
