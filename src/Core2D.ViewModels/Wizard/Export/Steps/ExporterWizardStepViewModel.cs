// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Core2D.ViewModels.Export;
using Core2D.ViewModels.Wizard.Export.Exporters;

namespace Core2D.ViewModels.Wizard.Export.Steps;

public sealed partial class ExporterWizardStepViewModel : WizardStepViewModelBase
{
    private readonly IExportOptionsCatalog _catalog;
    private readonly List<ExporterEntryViewModel> _allEntries = new();

    public ExporterWizardStepViewModel(IServiceProvider? serviceProvider, IExportOptionsCatalog catalog)
        : base(serviceProvider, 20, "Exporters", "Choose exporters and configure presets.")
    {
        _catalog = catalog;
    }

    public ObservableCollection<ExporterCategoryViewModel> Categories { get; } = new();

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                RaisePropertyChanged(new PropertyChangedEventArgs(nameof(SearchText)));
                ApplyFilter(_searchText);
            }
        }
    }

    public override async Task EnterAsync(CancellationToken cancellationToken)
    {
        BuildCatalog();
        ApplyFilter(SearchText);
        await base.EnterAsync(cancellationToken).ConfigureAwait(false);
        await ValidateAsync(cancellationToken).ConfigureAwait(false);
    }

    public override Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        var isValid = Context.SelectedExporters.Count > 0;
        State = isValid ? WizardStepState.Ready : WizardStepState.Warning;
        return Task.FromResult(isValid);
    }

    private void BuildCatalog()
    {
        if (Categories.Count > 0)
        {
            return;
        }

        foreach (var group in _catalog.Describe().GroupBy(d => d.Category).OrderBy(g => g.Key))
        {
            var category = new ExporterCategoryViewModel(ServiceProvider, group.Key);
            foreach (var descriptor in group.OrderBy(d => d.DisplayName))
            {
                var entry = new ExporterEntryViewModel(ServiceProvider, descriptor, OnAddRequested);
                category.Entries.Add(entry);
                _allEntries.Add(entry);
            }

            Categories.Add(category);
        }
    }

    private void OnAddRequested(ExporterEntryViewModel entry)
        => AddSelection(entry.Descriptor, null);

    private void AddSelection(ExportOptionsDescriptor descriptor, ExportOptionsBase? optionsOverride)
    {
        var options = optionsOverride ?? descriptor.Factory(ServiceProvider);
        var selection = new ExporterSelectionViewModel(ServiceProvider, descriptor, options, RemoveSelection, DuplicateSelection);
        Context.SelectedExporters.Add(selection);
        _ = ValidateAsync(CancellationToken.None);
    }

    private void RemoveSelection(ExporterSelectionViewModel selection)
    {
        Context.SelectedExporters.Remove(selection);
        _ = ValidateAsync(CancellationToken.None);
    }

    private void DuplicateSelection(ExporterSelectionViewModel selection)
    {
        var duplicate = selection.CreateDuplicate(RemoveSelection, DuplicateSelection);
        Context.SelectedExporters.Add(duplicate);
        _ = ValidateAsync(CancellationToken.None);
    }

    private void ApplyFilter(string? text)
    {
        foreach (var entry in _allEntries)
        {
            entry.ApplyFilter(text);
        }
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();
}
