// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.ViewModels.Shapes;

namespace Core2D.Procedural.WaveFunctionCollapse;

internal sealed class WaveFunctionShapeGenerator
{
    private readonly IViewModelFactory _factory;

    public WaveFunctionShapeGenerator(IViewModelFactory factory)
    {
        _factory = factory;
    }

    public bool TryGenerate(WaveFunctionRunRequest request, out ImmutableArray<BaseShapeViewModel> shapes)
    {
        var model = new WaveFunctionTileConstraintModel(request);
        var success = model.Run(request.Seed, request.Limit);
        if (!success)
        {
            shapes = default;
            return false;
        }

        var allShapes = new List<BaseShapeViewModel>(request.Columns * request.Rows * 2);
        var styleCache = new WaveFunctionStyleCache();
        var random = new Random(request.Seed);

        for (var row = 0; row < request.Rows; row++)
        {
            for (var column = 0; column < request.Columns; column++)
            {
                var tileIndex = model.GetObservedValue(column, row);
                if (tileIndex < 0 || tileIndex >= request.Preset.Tiles.Count)
                {
                    shapes = default;
                    return false;
                }

                var tile = request.Preset.Tiles[tileIndex];
                var context = new WaveFunctionTileBuilderContext(
                    _factory,
                    request.Preset,
                    tile,
                    styleCache,
                    random,
                    column,
                    row,
                    request.CellSize,
                    request.Padding,
                    request.StrokeThickness);

                var produced = tile.Builder?.Invoke(context);
                if (produced is null)
                {
                    continue;
                }

                foreach (var shape in produced)
                {
                    if (shape is { })
                    {
                        allShapes.Add(shape);
                    }
                }
            }
        }

        shapes = allShapes.ToImmutableArray();
        return true;
    }
}
