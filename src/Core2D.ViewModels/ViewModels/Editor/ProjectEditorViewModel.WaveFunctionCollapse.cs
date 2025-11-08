// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Procedural.WaveFunctionCollapse;

namespace Core2D.ViewModels.Editor;

public partial class WaveFunctionCollapseServiceViewModel : ViewModelBase, IWaveFunctionCollapseService
{
    private static readonly PropertyChangedEventArgs SelectedPresetPropertyChangedEventArgs = new(nameof(SelectedPreset));
    private static readonly PropertyChangedEventArgs GridColumnsPropertyChangedEventArgs = new(nameof(GridColumns));
    private static readonly PropertyChangedEventArgs GridRowsPropertyChangedEventArgs = new(nameof(GridRows));
    private static readonly PropertyChangedEventArgs CellSizePropertyChangedEventArgs = new(nameof(CellSize));
    private static readonly PropertyChangedEventArgs PaddingPropertyChangedEventArgs = new(nameof(Padding));
    private static readonly PropertyChangedEventArgs SeedPropertyChangedEventArgs = new(nameof(Seed));
    private static readonly PropertyChangedEventArgs MaxAttemptsPropertyChangedEventArgs = new(nameof(MaxAttempts));

    private readonly ImmutableArray<WaveFunctionPreset> _presets;
    private WaveFunctionPreset? _selectedPreset;
    private WaveFunctionShapeGenerator? _generator;

    private int _gridColumns;
    private int _gridRows;
    private double _cellSize;
    private double _padding;
    private int _seed;
    private int _maxAttempts = 32;

    [AutoNotify] private bool _clearLayerBeforeGenerate = true;
    [AutoNotify] private bool _autoIncrementSeed = true;

    public WaveFunctionCollapseServiceViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _presets = WaveFunctionDiagramPresets.All;
        _seed = Environment.TickCount;
        if (!_presets.IsDefaultOrEmpty)
        {
            SelectedPreset = _presets[0];
        }
        else
        {
            GridColumns = 10;
            GridRows = 6;
            CellSize = 120;
            Padding = 12;
        }
    }

    public ImmutableArray<WaveFunctionPreset> Presets => _presets;

    public WaveFunctionPreset? SelectedPreset
    {
        get => _selectedPreset;
        private set
        {
            if (Equals(_selectedPreset, value))
            {
                return;
            }

            RaiseAndSetIfChanged(ref _selectedPreset, value, SelectedPresetPropertyChangedEventArgs);
            if (value is { })
            {
                ApplyPresetDefaults(value);
            }
        }
    }

    public int GridColumns
    {
        get => _gridColumns;
        set => RaiseAndSetIfChanged(ref _gridColumns, Math.Clamp(value, 2, 64), GridColumnsPropertyChangedEventArgs);
    }

    public int GridRows
    {
        get => _gridRows;
        set => RaiseAndSetIfChanged(ref _gridRows, Math.Clamp(value, 2, 64), GridRowsPropertyChangedEventArgs);
    }

    public double CellSize
    {
        get => _cellSize;
        set
        {
            var clamped = Math.Clamp(value, 40.0, 400.0);
            if (Math.Abs(_cellSize - clamped) < double.Epsilon)
            {
                return;
            }

            RaiseAndSetIfChanged(ref _cellSize, clamped, CellSizePropertyChangedEventArgs);
            if (_padding > clamped * 0.45)
            {
                Padding = clamped * 0.45;
            }
        }
    }

    public double Padding
    {
        get => _padding;
        set => RaiseAndSetIfChanged(ref _padding, Math.Clamp(value, 4.0, CellSize * 0.45), PaddingPropertyChangedEventArgs);
    }

    public int Seed
    {
        get => _seed;
        set => RaiseAndSetIfChanged(ref _seed, value, SeedPropertyChangedEventArgs);
    }

    public int MaxAttempts
    {
        get => _maxAttempts;
        set => RaiseAndSetIfChanged(ref _maxAttempts, Math.Clamp(value, 1, 200), MaxAttemptsPropertyChangedEventArgs);
    }

    public void OnSelectPreset(object? parameter)
    {
        if (parameter is WaveFunctionPreset preset)
        {
            SelectedPreset = preset;
            return;
        }

        if (parameter is string name)
        {
            var match = _presets.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (match is { })
            {
                SelectedPreset = match;
            }
        }
    }

    public void OnGenerate()
    {
        var preset = SelectedPreset ?? (_presets.IsDefaultOrEmpty ? null : _presets[0]);
        if (preset is null)
        {
            return;
        }

        var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        var layer = project?.CurrentContainer?.CurrentLayer;
        var factory = ServiceProvider.GetService<IViewModelFactory>();
        if (project is null || layer is null || factory is null)
        {
            return;
        }

        _generator ??= new WaveFunctionShapeGenerator(factory);

        var limit = GridColumns * GridRows * preset.PropagationLimitMultiplier;
        var request = new WaveFunctionRunRequest(
            preset,
            GridColumns,
            GridRows,
            CellSize,
            Padding,
            preset.Periodic,
            preset.Heuristic,
            Seed,
            limit,
            preset.StrokeThickness);

        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var runSeed = unchecked(Seed + attempt);
            var runRequest = request with { Seed = runSeed };
            if (!_generator.TryGenerate(runRequest, out var shapes))
            {
                continue;
            }

            if (ClearLayerBeforeGenerate)
            {
                project.ClearLayer(layer);
            }

            project.AddShapes(layer, shapes);

            if (AutoIncrementSeed)
            {
                Seed = unchecked(runSeed + 1);
            }

            ServiceProvider.GetService<ISelectionService>()?.OnDeselectAll();
            layer.RaiseInvalidateLayer();
            return;
        }

        ServiceProvider.GetService<ILog>()?.LogWarning("WaveFunctionCollapse could not produce a valid layout.");
    }

    public void OnRegenerate()
    {
        Seed = unchecked(Seed + 1);
        OnGenerate();
    }

    public void OnRandomizeSeed()
    {
        Seed = HashCode.Combine(DateTime.UtcNow.Ticks, Environment.TickCount);
    }

    public void OnIncreaseColumns() => GridColumns += 1;

    public void OnDecreaseColumns() => GridColumns -= 1;

    public void OnIncreaseRows() => GridRows += 1;

    public void OnDecreaseRows() => GridRows -= 1;

    public void OnIncreaseCellSize() => CellSize += 10;

    public void OnDecreaseCellSize() => CellSize -= 10;

    public void OnIncreasePadding() => Padding += 2;

    public void OnDecreasePadding() => Padding -= 2;

    public void OnToggleClearLayer() => ClearLayerBeforeGenerate = !ClearLayerBeforeGenerate;

    public void OnToggleAutoIncrement() => AutoIncrementSeed = !AutoIncrementSeed;

    public void OnIncreaseAttempts() => MaxAttempts += 5;

    public void OnDecreaseAttempts() => MaxAttempts -= 5;

    private void ApplyPresetDefaults(WaveFunctionPreset preset)
    {
        GridColumns = preset.DefaultColumns;
        GridRows = preset.DefaultRows;
        CellSize = preset.CellSize;
        Padding = preset.Padding;
        MaxAttempts = preset.MaxAttempts;
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }
}
