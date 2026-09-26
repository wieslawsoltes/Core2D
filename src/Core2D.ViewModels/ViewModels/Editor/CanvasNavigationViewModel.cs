// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>Per-viewport commands and display state; has no dependency on Avalonia.</summary>
public sealed class CanvasNavigationViewModel : ReactiveObject, IDisposable
{
    private readonly Action<double> _zoom;
    private decimal _percent = 100;
    private bool _sync, _showRulers = true, _hand, _hasSelection;
    private string _pointer = string.Empty, _selection = string.Empty;
    private CanvasGuidesViewModel? _guides;

    /// <summary>Creates navigation commands whose operations target one viewport.</summary>
    public CanvasNavigationViewModel(Action<double> zoom, Action fitPage, Action fitSelection)
    {
        _zoom = zoom;
        // These commands perform synchronous UI-thread operations. Do not defer CanExecute
        // notifications to an unconfigured host's default task-pool scheduler.
        ZoomIn = ReactiveCommand.Create(() => { ZoomPercent *= 1.25m; }, outputScheduler: CurrentThreadScheduler.Instance);
        ZoomOut = ReactiveCommand.Create(() => { ZoomPercent /= 1.25m; }, outputScheduler: CurrentThreadScheduler.Instance);
        ResetZoom = ReactiveCommand.Create(() => { ZoomPercent = 100; }, outputScheduler: CurrentThreadScheduler.Instance);
        FitPage = ReactiveCommand.Create(fitPage, outputScheduler: CurrentThreadScheduler.Instance);
        FitSelection = ReactiveCommand.Create(fitSelection, this.WhenAnyValue(x => x.HasSelection), outputScheduler: CurrentThreadScheduler.Instance);
        ClearGuides = ReactiveCommand.Create(() => Guides?.Clear(), outputScheduler: CurrentThreadScheduler.Instance);
    }
    /// <summary>Gets or sets zoom as a percentage, between 1 and 25600.</summary>
    public decimal ZoomPercent
    {
        get => _percent;
        set
        {
            decimal next = Math.Clamp(value, 1, 25600);
            if (_percent == next) return;
            this.RaiseAndSetIfChanged(ref _percent, next);
            this.RaisePropertyChanged(nameof(ZoomText));
            if (!_sync) _zoom((double)next / 100);
        }
    }
    /// <summary>Gets the current zoom label.</summary>
    public string ZoomText => $"{ZoomPercent.ToString("0.##", CultureInfo.CurrentCulture)}%";
    /// <summary>Gets or sets ruler visibility.</summary>
    public bool ShowRulers { get => _showRulers; set => this.RaiseAndSetIfChanged(ref _showRulers, value); }
    /// <summary>Gets or sets safe hand-tool mode without changing the drawing tool.</summary>
    public bool IsHandTool { get => _hand; set => this.RaiseAndSetIfChanged(ref _hand, value); }
    /// <summary>Gets or sets whether selection bounds are available.</summary>
    public bool HasSelection { get => _hasSelection; set => this.RaiseAndSetIfChanged(ref _hasSelection, value); }
    /// <summary>Gets or sets cursor coordinates in page space.</summary>
    public string PointerSummary { get => _pointer; set => this.RaiseAndSetIfChanged(ref _pointer, value); }
    /// <summary>Gets or sets a non-editing selection measurement readout.</summary>
    public string SelectionSummary { get => _selection; set => this.RaiseAndSetIfChanged(ref _selection, value); }
    /// <summary>Gets or sets the current page's guide session.</summary>
    public CanvasGuidesViewModel? Guides { get => _guides; set => this.RaiseAndSetIfChanged(ref _guides, value); }
    /// <summary>Zooms in around the viewport center.</summary>
    public ReactiveCommand<Unit, Unit> ZoomIn { get; }
    /// <summary>Zooms out around the viewport center.</summary>
    public ReactiveCommand<Unit, Unit> ZoomOut { get; }
    /// <summary>Sets 100 percent zoom.</summary>
    public ReactiveCommand<Unit, Unit> ResetZoom { get; }
    /// <summary>Fits the current page.</summary>
    public ReactiveCommand<Unit, Unit> FitPage { get; }
    /// <summary>Fits the current selection.</summary>
    public ReactiveCommand<Unit, Unit> FitSelection { get; }
    /// <summary>Removes page guides as one history operation.</summary>
    public ReactiveCommand<Unit, Unit> ClearGuides { get; }
    /// <summary>Updates measured zoom without writing back into the viewport.</summary>
    public void UpdateZoom(double zoom)
    {
        if (!double.IsFinite(zoom) || zoom <= 0) return;
        _sync = true;
        try { ZoomPercent = (decimal)Math.Clamp(zoom * 100, 1, 25600); }
        finally { _sync = false; }
    }
    /// <summary>Releases command subscriptions when the viewport detaches.</summary>
    public void Dispose()
    {
        ZoomIn.Dispose(); ZoomOut.Dispose(); ResetZoom.Dispose(); FitPage.Dispose(); FitSelection.Dispose(); ClearGuides.Dispose();
    }
}
