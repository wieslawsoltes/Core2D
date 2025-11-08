// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core2D.ViewModels.Wizard.Export.Exporters;

namespace Core2D.ViewModels.Wizard.Export.Steps;

public sealed partial class SettingsWizardStepViewModel : WizardStepViewModelBase
{
    private bool _isSubscribed;

    public SettingsWizardStepViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, 30, "Settings", "Adjust exporter-specific settings.")
    {
    }

    private ExporterSelectionViewModel? _activeSelection;

    public ExporterSelectionViewModel? ActiveSelection
    {
        get => _activeSelection;
        set
        {
            if (!ReferenceEquals(_activeSelection, value))
            {
                _activeSelection = value;
                RaisePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(ActiveSelection)));
                if (value is { })
                {
                    value.Options.Validate();
                }

                _ = ValidateAsync(CancellationToken.None);
            }
        }
    }

    public override async Task EnterAsync(CancellationToken cancellationToken)
    {
        EnsureSubscriptions();
        SetDefaultSelection();
        await base.EnterAsync(cancellationToken).ConfigureAwait(false);
        await ValidateAsync(cancellationToken).ConfigureAwait(false);
    }

    public override Task LeaveAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public override Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        var any = Context.SelectedExporters.Count > 0;
        foreach (var exporter in Context.SelectedExporters)
        {
            exporter.Options.Validate();
            if (!exporter.Options.IsValid)
            {
                State = WizardStepState.Warning;
                return Task.FromResult(false);
            }
        }

        State = any ? WizardStepState.Ready : WizardStepState.Warning;
        return Task.FromResult(any);
    }

    private void EnsureSubscriptions()
    {
        if (_isSubscribed)
        {
            return;
        }

        _isSubscribed = true;
        Context.SelectedExporters.CollectionChanged += OnSelectedExportersChanged;
    }

    private void OnSelectedExportersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Context.SelectedExporters.Count == 0)
        {
            ActiveSelection = null;
        }
        else if (ActiveSelection is null || !Context.SelectedExporters.Contains(ActiveSelection))
        {
            ActiveSelection = Context.SelectedExporters.FirstOrDefault();
        }

        _ = ValidateAsync(CancellationToken.None);
    }

    private void SetDefaultSelection()
    {
        if (ActiveSelection is null)
        {
            ActiveSelection = Context.SelectedExporters.FirstOrDefault();
        }
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
