// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Immutable;
using Core2D.Procedural.WaveFunctionCollapse;

namespace Core2D.ViewModels.Editor;

public interface IWaveFunctionCollapseService
{
    ImmutableArray<WaveFunctionPreset> Presets { get; }

    WaveFunctionPreset? SelectedPreset { get; }

    int GridColumns { get; set; }

    int GridRows { get; set; }

    double CellSize { get; set; }

    double Padding { get; set; }

    int Seed { get; set; }

    bool ClearLayerBeforeGenerate { get; set; }

    bool AutoIncrementSeed { get; set; }

    int MaxAttempts { get; set; }

    void OnSelectPreset(object? parameter);

    void OnGenerate();

    void OnRegenerate();

    void OnRandomizeSeed();

    void OnIncreaseColumns();

    void OnDecreaseColumns();

    void OnIncreaseRows();

    void OnDecreaseRows();

    void OnIncreaseCellSize();

    void OnDecreaseCellSize();

    void OnIncreasePadding();

    void OnDecreasePadding();

    void OnToggleClearLayer();

    void OnToggleAutoIncrement();

    void OnIncreaseAttempts();

    void OnDecreaseAttempts();
}
