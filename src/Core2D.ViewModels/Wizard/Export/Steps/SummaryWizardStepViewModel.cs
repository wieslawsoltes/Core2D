// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core2D.ViewModels.Wizard.Export.Destination;
using Core2D.ViewModels.Wizard.Export.Exporters;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.ViewModels.Wizard.Export.Steps;

public sealed partial class SummaryWizardStepViewModel : WizardStepViewModelBase
{
    private const int PreviewLimit = 5;
    private bool _isSubscribed;

    public SummaryWizardStepViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, 50, "Summary", "Review selections and run the export.")
    {
    }

    public ObservableCollection<string> ScopeSummaries { get; } = new();

    public ObservableCollection<string> ExporterSummaries { get; } = new();

    public ObservableCollection<DestinationPreviewItem> PreviewItems { get; } = new();

    public ObservableCollection<SummaryIssueViewModel> Issues { get; } = new();

    [AutoNotify] private int _estimatedJobCount;
    [AutoNotify] private bool _hasIssues = true;
    [AutoNotify] private bool _hasScopes;
    [AutoNotify] private bool _hasExporters;

    public override async Task EnterAsync(CancellationToken cancellationToken)
    {
        EnsureSubscriptions();
        RefreshSummary();
        await base.EnterAsync(cancellationToken).ConfigureAwait(false);
        await ValidateAsync(cancellationToken).ConfigureAwait(false);
    }

    public override Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        BuildIssues();
        var ready = !HasIssues;
        State = ready ? WizardStepState.Ready : WizardStepState.Warning;
        return Task.FromResult(ready);
    }

    private void EnsureSubscriptions()
    {
        if (_isSubscribed)
        {
            return;
        }

        _isSubscribed = true;
        Context.PropertyChanged += OnContextPropertyChanged;
        Context.SelectedScopes.CollectionChanged += OnScopesChanged;
        Context.SelectedExporters.CollectionChanged += OnExportersChanged;
    }

    private void OnContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Context.DestinationFolder)
            or nameof(Context.SubfolderTemplate)
            or nameof(Context.FileNameTemplate)
            or nameof(Context.OverwriteExisting))
        {
            RefreshSummary();
        }
    }

    private void OnScopesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => RefreshSummary();

    private void OnExportersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Add && e.NewItems is { })
        {
            AttachExporterHandlers(e.NewItems.Cast<object?>());
        }

        RefreshSummary();
    }

    private void AttachExporterHandlers(IEnumerable<object?> exporters)
    {
        foreach (var exporter in exporters.OfType<ExporterSelectionViewModel>())
        {
            exporter.Options.Validate();
        }
    }

    private void RefreshSummary()
    {
        ScopeSummaries.Clear();
        foreach (var scope in Context.SelectedScopes)
        {
            ScopeSummaries.Add(ExportScopeFormatter.Describe(scope));
        }

        ExporterSummaries.Clear();
        foreach (var exporter in Context.SelectedExporters)
        {
            var display = $"{exporter.DisplayName} ({exporter.Extension})";
            ExporterSummaries.Add(display);
        }

        HasScopes = ScopeSummaries.Count > 0;
        HasExporters = ExporterSummaries.Count > 0;
        EstimatedJobCount = Context.SelectedScopes.Count * Context.SelectedExporters.Count;
        RefreshPreview();
        _ = ValidateAsync(CancellationToken.None);
    }

    private void RefreshPreview()
    {
        PreviewItems.Clear();

        if (Context.SelectedScopes.Count == 0 || Context.SelectedExporters.Count == 0)
        {
            return;
        }

        var count = 0;
        foreach (var scope in Context.SelectedScopes)
        {
            foreach (var exporter in Context.SelectedExporters)
            {
                var path = ExportPathBuilder.BuildPath(Context, scope, exporter);
                var label = ExportScopeFormatter.Describe(scope);
                PreviewItems.Add(new DestinationPreviewItem(label, exporter.DisplayName, path));

                count++;
                if (count >= PreviewLimit)
                {
                    return;
                }
            }
        }
    }

    private void BuildIssues()
    {
        Issues.Clear();

        if (Context.SelectedScopes.Count == 0)
        {
            Issues.Add(new SummaryIssueViewModel(SummaryIssueSeverity.Error, "Select at least one scope."));
        }

        if (Context.SelectedExporters.Count == 0)
        {
            Issues.Add(new SummaryIssueViewModel(SummaryIssueSeverity.Error, "Select at least one exporter."));
        }

        if (Context.SelectedExporters.Any(x => !x.Options.IsValid))
        {
            Issues.Add(new SummaryIssueViewModel(SummaryIssueSeverity.Error, "Fix exporter validation errors in the Settings step."));
        }

        if (string.IsNullOrWhiteSpace(Context.DestinationFolder))
        {
            Issues.Add(new SummaryIssueViewModel(SummaryIssueSeverity.Warning, "Destination folder is empty; exports will be created relative to the application working directory."));
        }

        HasIssues = Issues.Count > 0;
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
