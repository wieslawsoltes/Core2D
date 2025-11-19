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

public sealed partial class DestinationWizardStepViewModel : WizardStepViewModelBase
{
    private const int PreviewLimit = 10;
    private readonly ObservableCollection<DestinationPreviewItem> _previewItems = new();
    private bool _subscribed;

    public DestinationWizardStepViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, 40, "Destination", "Pick output folders and naming templates.")
    {
        PreviewItems = new ReadOnlyObservableCollection<DestinationPreviewItem>(_previewItems);
    }

    public ReadOnlyObservableCollection<DestinationPreviewItem> PreviewItems { get; }

    public override async Task EnterAsync(CancellationToken cancellationToken)
    {
        EnsureDefaults();
        EnsureSubscriptions();
        RefreshPreview();
        await base.EnterAsync(cancellationToken).ConfigureAwait(false);
        await ValidateAsync(cancellationToken).ConfigureAwait(false);
    }

    public override Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        var hasFolder = !string.IsNullOrWhiteSpace(Context.DestinationFolder);
        var hasExporter = Context.SelectedExporters.Count > 0;
        var hasScope = Context.SelectedScopes.Count > 0;
        var ready = hasFolder && hasExporter && hasScope;
        State = ready ? WizardStepState.Ready : WizardStepState.Warning;
        return Task.FromResult(ready);
    }

    private void EnsureDefaults()
    {
        if (string.IsNullOrWhiteSpace(Context.SubfolderTemplate))
        {
            Context.SubfolderTemplate = "{ProjectName}";
        }

        if (string.IsNullOrWhiteSpace(Context.FileNameTemplate))
        {
            Context.FileNameTemplate = "{DocumentName}_{PageName}_{Exporter}.{ExporterExtension}";
        }
    }

    private void EnsureSubscriptions()
    {
        if (_subscribed)
        {
            return;
        }

        _subscribed = true;
        Context.PropertyChanged += OnContextPropertyChanged;
        Context.SelectedExporters.CollectionChanged += OnSelectionChanged;
        Context.SelectedScopes.CollectionChanged += OnSelectionChanged;
    }

    private void OnContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Context.DestinationFolder)
            or nameof(Context.SubfolderTemplate)
            or nameof(Context.FileNameTemplate)
            or nameof(Context.OverwriteExisting))
        {
            RefreshPreview();
        }
    }

    private void OnSelectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => RefreshPreview();

    private void RefreshPreview()
    {
        _previewItems.Clear();

        if (Context.SelectedScopes.Count == 0 || Context.SelectedExporters.Count == 0)
        {
            return;
        }

        foreach (var scope in Context.SelectedScopes.Take(PreviewLimit))
        {
            foreach (var exporter in Context.SelectedExporters)
            {
                if (!exporter.Descriptor.Capabilities.Contains(scope.Kind.ToString()))
                {
                    continue;
                }

                var path = ExportPathBuilder.BuildPath(Context, scope, exporter);
                var label = ExportScopeFormatter.Describe(scope);
                _previewItems.Add(new DestinationPreviewItem(label, exporter.DisplayName, path));

                if (_previewItems.Count >= PreviewLimit)
                {
                    Telemetry.DestinationPreviewUpdated(_previewItems.Count);
                    return;
                }
            }
        }

        Telemetry.DestinationPreviewUpdated(_previewItems.Count);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
