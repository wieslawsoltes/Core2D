// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.Procedural.WaveFunctionCollapse;

internal sealed class WaveFunctionTileConstraintModel : WaveFunctionModel
{
    private static readonly WaveFunctionDirection[] Directions =
    {
        WaveFunctionDirection.West,
        WaveFunctionDirection.South,
        WaveFunctionDirection.East,
        WaveFunctionDirection.North
    };

    public WaveFunctionTileConstraintModel(WaveFunctionRunRequest request)
        : base(request.Columns, request.Rows, 1, request.Periodic, request.Heuristic)
    {
        var tileCount = request.Preset.Tiles.Count;
        T = tileCount;
        weights = new double[tileCount];
        for (var i = 0; i < tileCount; i++)
        {
            weights[i] = Math.Max(0.001, request.Preset.Tiles[i].Weight);
        }

        propagator = BuildPropagator(request.Preset.Tiles);
    }

    private static int[][][] BuildPropagator(IReadOnlyList<WaveFunctionTileDefinition> tiles)
    {
        var tileCount = tiles.Count;
        var propagator = new int[4][][];

        for (var dir = 0; dir < 4; dir++)
        {
            propagator[dir] = new int[tileCount][];
            var direction = Directions[dir];

            for (var sourceIndex = 0; sourceIndex < tileCount; sourceIndex++)
            {
                var sourceTile = tiles[sourceIndex];
                var allowed = new List<int>(tileCount);

                for (var targetIndex = 0; targetIndex < tileCount; targetIndex++)
                {
                    var targetTile = tiles[targetIndex];
                    if (IsCompatible(sourceTile, targetTile, direction))
                    {
                        allowed.Add(targetIndex);
                    }
                }

                propagator[dir][sourceIndex] = allowed.ToArray();
            }
        }

        return propagator;
    }

    private static bool IsCompatible(WaveFunctionTileDefinition source, WaveFunctionTileDefinition target, WaveFunctionDirection direction)
    {
        if (source.Flags.HasFlag(WaveFunctionTileFlags.Wildcard) || target.Flags.HasFlag(WaveFunctionTileFlags.Wildcard))
        {
            return true;
        }

        var sourceHasConnector = source.Connectors.HasConnector(direction);
        var targetHasConnector = target.Connectors.HasConnector(direction.Opposite());
        return sourceHasConnector == targetHasConnector;
    }
}
