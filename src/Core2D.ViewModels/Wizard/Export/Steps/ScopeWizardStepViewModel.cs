// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.ViewModels.Wizard.Export.Steps;

public sealed class ScopeWizardStepViewModel : WizardStepViewModelBase
{
    private readonly List<ExportScopeNodeViewModel> _trackedNodes = new();

    public ScopeWizardStepViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, 10, "Scope", "Select the project, documents, or pages to export.")
    {
    }

    public ObservableCollection<ExportScopeNodeViewModel> Nodes { get; } = new();

    public override async Task EnterAsync(CancellationToken cancellationToken)
    {
        BuildScopeTree();
        await base.EnterAsync(cancellationToken).ConfigureAwait(false);
        await ValidateAsync(cancellationToken).ConfigureAwait(false);
    }

    public override Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        var isValid = Context.SelectedScopes.Count > 0;
        State = isValid ? WizardStepState.Ready : WizardStepState.Warning;
        return Task.FromResult(isValid);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotSupportedException();

    private void BuildScopeTree()
    {
        foreach (var node in _trackedNodes)
        {
            node.SelectionChanged -= OnNodeSelectionChanged;
        }

        _trackedNodes.Clear();
        Nodes.Clear();

        var project = Context.Project;
        if (project is null)
        {
            Context.SelectedScopes.Clear();
            return;
        }

        var previousSelections = Context.SelectedScopes.ToList();
        Context.SelectedScopes.Clear();

        var projectNode = CreateProjectNode(project);
        Nodes.Add(projectNode);
        projectNode.IsExpanded = true;

        foreach (var selection in previousSelections)
        {
            var node = FindNode(selection);
            if (node is { })
            {
                node.SetIsSelected(true, suppressNotification: true);
                Context.SelectedScopes.Add(node.ToSelection());
            }
        }

        _ = ValidateAsync(CancellationToken.None);
    }

    private ExportScopeNodeViewModel? FindNode(ExportScopeSelection selection)
        => _trackedNodes.FirstOrDefault(node => node.Matches(selection));

    private ExportScopeNodeViewModel CreateProjectNode(ProjectContainerViewModel project)
    {
        var node = new ExportScopeNodeViewModel(ServiceProvider, ExportScopeKind.Project, project, null, null)
        {
            Title = string.IsNullOrWhiteSpace(project.Name) ? "Project" : project.Name,
            IsExpanded = true
        };

        RegisterNode(node);

        foreach (var document in project.Documents)
        {
            if (document is null)
            {
                continue;
            }

            node.Children.Add(CreateDocumentNode(project, document));
        }

        return node;
    }

    private ExportScopeNodeViewModel CreateDocumentNode(ProjectContainerViewModel project, DocumentContainerViewModel document)
    {
        var node = new ExportScopeNodeViewModel(ServiceProvider, ExportScopeKind.Document, project, document, null)
        {
            Title = string.IsNullOrWhiteSpace(document.Name) ? "Document" : document.Name,
            IsExpanded = true
        };

        RegisterNode(node);

        foreach (var page in document.Pages)
        {
            if (page is null)
            {
                continue;
            }

            node.Children.Add(CreatePageNode(project, document, page));
        }

        return node;
    }

    private ExportScopeNodeViewModel CreatePageNode(ProjectContainerViewModel project, DocumentContainerViewModel document, PageContainerViewModel page)
    {
        var node = new ExportScopeNodeViewModel(ServiceProvider, ExportScopeKind.Page, project, document, page)
        {
            Title = string.IsNullOrWhiteSpace(page.Name) ? "Page" : page.Name
        };

        RegisterNode(node);
        return node;
    }

    private void RegisterNode(ExportScopeNodeViewModel node)
    {
        _trackedNodes.Add(node);
        node.SelectionChanged += OnNodeSelectionChanged;
    }

    private void OnNodeSelectionChanged(object? sender, bool isSelected)
    {
        if (sender is not ExportScopeNodeViewModel node)
        {
            return;
        }

        var existing = Context.SelectedScopes.FirstOrDefault(selection => node.Matches(selection));

        if (isSelected)
        {
            if (existing is null)
            {
                Context.SelectedScopes.Add(node.ToSelection());
            }
        }
        else
        {
            if (existing is { })
            {
                Context.SelectedScopes.Remove(existing);
            }
        }

        _ = ValidateAsync(CancellationToken.None);
    }
}
