// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Docking.Documents;
using Core2D.ViewModels.Shapes;
using Dock.Model;
using Dock.Model.Controls;
using Dock.Model.Core;

namespace Core2D.ViewModels.Editor;

public partial class ProjectEditorViewModel
{
    private readonly Dictionary<PageContainerViewModel, PageViewModel> _pageDocuments = new();
    private readonly Dictionary<PageViewModel, PageContainerViewModel> _documentToPage = new();
    private readonly Dictionary<TemplateContainerViewModel, TemplateViewModel> _templateDocuments = new();
    private readonly Dictionary<TemplateViewModel, TemplateContainerViewModel> _documentToTemplate = new();
    private readonly Dictionary<BlockShapeViewModel, BlockDocumentViewModel> _groupDocuments = new();
    private readonly Dictionary<BlockDocumentViewModel, BlockShapeViewModel> _documentToGroup = new();
    private readonly Dictionary<PageContainerViewModel, BlockDocumentViewModel> _containerToGroupDocument = new();
    private readonly Dictionary<BlockDocumentViewModel, GroupDocumentTracker> _groupDocumentTrackers = new();
    private IDocumentDock? _pagesDock;
    private bool _suppressPageActivation;
    private int _suppressDocumentDockCreate;

    private void EnsurePagesDock()
    {
        var dock = GetPagesDock();
        if (ReferenceEquals(_pagesDock, dock))
        {
            return;
        }

        if (_pagesDock is INotifyPropertyChanged oldNotify)
        {
            oldNotify.PropertyChanged -= OnPagesDockPropertyChanged;
        }

        _pagesDock = dock;

        if (_pagesDock is INotifyPropertyChanged newNotify)
        {
            newNotify.PropertyChanged += OnPagesDockPropertyChanged;
        }

        PrunePageDocuments();
        PruneTemplateDocuments();
        PruneGroupDocuments();
        SynchronizeDocumentsWithDock();

        if (_pagesDock is { } && Project?.CurrentContainer is { } container)
        {
            switch (container)
            {
                case PageContainerViewModel page when _containerToGroupDocument.TryGetValue(page, out var groupDocument) &&
                                                     _documentToGroup.TryGetValue(groupDocument, out var group):
                    OpenGroup(group);
                    break;
                case PageContainerViewModel page:
                    OpenPage(page);
                    break;
                case TemplateContainerViewModel template:
                    OpenTemplate(template);
                    break;
            }
        }
    }

    private IDocumentDock? GetPagesDock()
    {
        if (DockFactory is FactoryBase factoryBase)
        {
            return factoryBase.GetDockable<IDocumentDock>("Pages");
        }

        return null;
    }

    private void OnPagesDockPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_pagesDock is null || _suppressPageActivation)
        {
            return;
        }

        if (sender is not IDock dock)
        {
            return;
        }

        if (e.PropertyName == nameof(IDock.ActiveDockable))
        {
            if (dock.ActiveDockable is PageViewModel pageDocument &&
                _documentToPage.TryGetValue(pageDocument, out var page))
            {
                SyncProjectSelection(page);
            }
            else if (dock.ActiveDockable is TemplateViewModel templateDocument &&
                     _documentToTemplate.TryGetValue(templateDocument, out var template))
            {
                SyncProjectSelection(template);
            }
            else if (dock.ActiveDockable is BlockDocumentViewModel groupDocument &&
                     _documentToGroup.TryGetValue(groupDocument, out var group))
            {
                if (groupDocument.Container is { } container)
                {
                    SyncProjectSelection(container);
                }

                if (Project is { })
                {
                    Project.Selected = group;
                }
            }
        }
        else if (e.PropertyName == nameof(IDock.VisibleDockables))
        {
            SynchronizeDocumentsWithDock();
        }
    }

    private void SyncProjectSelection(FrameContainerViewModel container)
    {
        if (Project is null)
        {
            return;
        }

        switch (container)
        {
            case PageContainerViewModel page:
            {
                if (_containerToGroupDocument.TryGetValue(page, out var groupDocument) &&
                    _documentToGroup.TryGetValue(groupDocument, out var group))
                {
                    if (!ReferenceEquals(Project.CurrentContainer, page))
                    {
                        Project.SetCurrentContainer(page);
                    }

                    if (!ReferenceEquals(Project.Selected, group))
                    {
                        Project.Selected = group;
                    }

                    break;
                }

                var document = Project.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                if (document is { } && !ReferenceEquals(Project.CurrentDocument, document))
                {
                    Project.SetCurrentDocument(document);
                }

                if (!ReferenceEquals(Project.CurrentContainer, page))
                {
                    Project.SetCurrentContainer(page);
                }

                break;
            }
            case TemplateContainerViewModel template:
            {
                if (!ReferenceEquals(Project.CurrentTemplate, template))
                {
                    Project.SetCurrentTemplate(template);
                }

                if (!ReferenceEquals(Project.CurrentContainer, template))
                {
                    Project.SetCurrentContainer(template);
                }

                break;
            }
        }
    }

    public void OpenPage(PageContainerViewModel? page)
    {
        if (page is null)
        {
            return;
        }

        EnsurePagesDock();

        if (_pagesDock is null || DockFactory is not FactoryBase factoryBase)
        {
            return;
        }

        var document = EnsurePageDocument(page, factoryBase);
        if (document is null)
        {
            return;
        }

        if (!ReferenceEquals(_pagesDock.ActiveDockable, document))
        {
            try
            {
                _suppressPageActivation = true;
                factoryBase.SetActiveDockable(document);
                factoryBase.SetFocusedDockable(_pagesDock, document);
            }
            finally
            {
                _suppressPageActivation = false;
            }
        }

        SyncProjectSelection(page);
    }

    public void OpenTemplate(TemplateContainerViewModel? template)
    {
        if (template is null)
        {
            return;
        }

        EnsurePagesDock();

        if (_pagesDock is null || DockFactory is not FactoryBase factoryBase)
        {
            return;
        }

        var document = EnsureTemplateDocument(template, factoryBase);
        if (document is null)
        {
            return;
        }

        if (!ReferenceEquals(_pagesDock.ActiveDockable, document))
        {
            try
            {
                _suppressPageActivation = true;
                factoryBase.SetActiveDockable(document);
                factoryBase.SetFocusedDockable(_pagesDock, document);
            }
            finally
            {
                _suppressPageActivation = false;
            }
        }

        SyncProjectSelection(template);
        template.InvalidateLayer();
    }

    public void OpenGroup(BlockShapeViewModel? group)
    {
        if (group is null)
        {
            return;
        }

        EnsurePagesDock();

        if (_pagesDock is null || DockFactory is not FactoryBase factoryBase)
        {
            return;
        }

        var document = EnsureGroupDocument(group, factoryBase);
        if (document is null)
        {
            return;
        }

        if (!ReferenceEquals(_pagesDock.ActiveDockable, document))
        {
            try
            {
                _suppressPageActivation = true;
                factoryBase.SetActiveDockable(document);
                factoryBase.SetFocusedDockable(_pagesDock, document);
            }
            finally
            {
                _suppressPageActivation = false;
            }
        }

        if (Project is { })
        {
            if (document.Container is { } container && !ReferenceEquals(Project.CurrentContainer, container))
            {
                Project.SetCurrentContainer(container);
            }

            if (!ReferenceEquals(Project.Selected, group))
            {
                Project.Selected = group;
            }
        }

        document.Container?.InvalidateLayer();
    }

    private PageViewModel? EnsurePageDocument(PageContainerViewModel page, FactoryBase factory)
    {
        if (_pageDocuments.TryGetValue(page, out var existing))
        {
            UpdatePageDocument(existing, page);
            return existing;
        }

        if (_pagesDock is null)
        {
            return null;
        }

        var document = new PageViewModel
        {
            Id = $"Page::{Guid.NewGuid():N}",
            Title = page.Name,
            Context = this,
            Page = page,
            CanClose = true
        };

        _pageDocuments[page] = document;
        _documentToPage[document] = page;

        if (_pagesDock.VisibleDockables?.Contains(document) != true)
        {
            factory.AddDockable(_pagesDock, document);
        }

        return document;
    }

    private TemplateViewModel? EnsureTemplateDocument(TemplateContainerViewModel template, FactoryBase factory)
    {
        if (_templateDocuments.TryGetValue(template, out var existing))
        {
            UpdateTemplateDocument(existing, template);
            return existing;
        }

        if (_pagesDock is null)
        {
            return null;
        }

        var document = new TemplateViewModel
        {
            Id = $"Template::{Guid.NewGuid():N}",
            Title = template.Name,
            Context = this,
            Template = template,
            CanClose = true
        };

        _templateDocuments[template] = document;
        _documentToTemplate[document] = template;

        if (_pagesDock.VisibleDockables?.Contains(document) != true)
        {
            factory.AddDockable(_pagesDock, document);
        }

        return document;
    }

    private BlockDocumentViewModel? EnsureGroupDocument(BlockShapeViewModel group, FactoryBase factory)
    {
        if (_groupDocuments.TryGetValue(group, out var existing))
        {
            UpdateGroupDocument(existing, group);
            return existing;
        }

        if (_pagesDock is null)
        {
            return null;
        }

        var viewModelFactory = ServiceProvider?.GetService<IViewModelFactory>();
        if (viewModelFactory is null)
        {
            return null;
        }

        var container = viewModelFactory.CreatePageContainer(group.Name);
        if (container is null)
        {
            return null;
        }

        container.Template = null;
        container.Name = group.Name;

        var baseLayer = container.Layers.FirstOrDefault();
        if (baseLayer is null)
        {
            return null;
        }

        var shared = new Dictionary<object, object>();
        var shapesBuilder = ImmutableArray.CreateBuilder<BaseShapeViewModel>();
        var groupShapes = group.Shapes;
        shapesBuilder.AddRange(groupShapes.CopyShared(shared));
        var groupConnectors = group.Connectors;
        shapesBuilder.AddRange(groupConnectors.CopyShared(shared));
        baseLayer.Shapes = shapesBuilder.ToImmutable();
        container.CurrentLayer = baseLayer;

        var document = new BlockDocumentViewModel
        {
            Id = $"Block::{Guid.NewGuid():N}",
            Title = group.Name,
            Context = this,
            Group = group,
            Container = container,
            CanClose = true
        };

        _groupDocuments[group] = document;
        _documentToGroup[document] = group;
        _containerToGroupDocument[container] = document;

        AttachGroupDocument(document);

        if (_pagesDock.VisibleDockables?.Contains(document) != true)
        {
            factory.AddDockable(_pagesDock, document);
        }

        return document;
    }

    private void UpdatePageDocument(PageViewModel document, PageContainerViewModel page)
    {
        if (!ReferenceEquals(document.Page, page))
        {
            document.Page = page;
        }

        if (!string.Equals(document.Title, page.Name, StringComparison.Ordinal))
        {
            document.Title = page.Name;
        }
    }

    private void UpdateTemplateDocument(TemplateViewModel document, TemplateContainerViewModel template)
    {
        if (!ReferenceEquals(document.Template, template))
        {
            document.Template = template;
        }

        if (!string.Equals(document.Title, template.Name, StringComparison.Ordinal))
        {
            document.Title = template.Name;
        }
    }

    private void UpdateGroupDocument(BlockDocumentViewModel document, BlockShapeViewModel group)
    {
        if (!ReferenceEquals(document.Group, group))
        {
            document.Group = group;
        }

        if (document.Container is { } container && !string.Equals(container.Name, group.Name, StringComparison.Ordinal))
        {
            container.Name = group.Name;
        }

        if (!string.Equals(document.Title, group.Name, StringComparison.Ordinal))
        {
            document.Title = group.Name;
        }
    }

    private void DetachDocument(PageViewModel document, bool collapse)
    {
        if (_documentToPage.TryGetValue(document, out var page))
        {
            _documentToPage.Remove(document);
            _pageDocuments.Remove(page);
        }

        if (collapse && document.Owner is IDock && DockFactory is FactoryBase factory)
        {
            using (SuppressDocumentDockCreation())
            {
                factory.RemoveDockable(document, true);
            }
        }
    }

    private void DetachTemplateDocument(TemplateViewModel document, bool collapse)
    {
        if (_documentToTemplate.TryGetValue(document, out var template))
        {
            _documentToTemplate.Remove(document);
            _templateDocuments.Remove(template);
        }

        if (collapse && document.Owner is IDock && DockFactory is FactoryBase factory)
        {
            using (SuppressDocumentDockCreation())
            {
                factory.RemoveDockable(document, true);
            }
        }
    }

    private void DetachGroupDocument(BlockDocumentViewModel document, bool collapse)
    {
        if (_documentToGroup.TryGetValue(document, out var group))
        {
            _documentToGroup.Remove(document);
            _groupDocuments.Remove(group);
        }

        if (document.Container is { } container)
        {
            _containerToGroupDocument.Remove(container);

            if (Project is { } project && ReferenceEquals(project.CurrentContainer, container))
            {
                var fallbackPage = project.CurrentDocument?.Pages.FirstOrDefault();
                FrameContainerViewModel? fallback = fallbackPage;

                if (fallback is null)
                {
                    fallback = project.CurrentTemplate;
                }

                if (fallback is null)
                {
                    fallback = project.Documents.SelectMany(d => d.Pages).FirstOrDefault();
                }

                project.SetCurrentContainer(fallback);
            }
        }

        DetachGroupDocumentSubscriptions(document);

        SyncGroupDocument(document);

        if (collapse && document.Owner is IDock && DockFactory is FactoryBase factory)
        {
            using (SuppressDocumentDockCreation())
            {
                factory.RemoveDockable(document, true);
            }
        }
    }

    private void AttachGroupDocument(BlockDocumentViewModel document)
    {
        if (!_documentToGroup.ContainsKey(document))
        {
            return;
        }

        DetachGroupDocumentSubscriptions(document);

        if (document.Container is null)
        {
            return;
        }

        var tracker = new GroupDocumentTracker();
        _groupDocumentTrackers[document] = tracker;

        tracker.ContainerSubscription = document.Container.Subscribe(new DelegatingObserver(args =>
        {
            if (args.e.PropertyName == nameof(PageContainerViewModel.Layers))
            {
                SubscribeGroupLayers(document, tracker);
            }

            SyncGroupDocument(document);
        }));

        SubscribeGroupLayers(document, tracker);
        SyncGroupDocument(document);
    }

    private void SubscribeGroupLayers(BlockDocumentViewModel document, GroupDocumentTracker tracker)
    {
        foreach (var subscription in tracker.LayerSubscriptions)
        {
            subscription.Dispose();
        }

        tracker.LayerSubscriptions.Clear();

        if (document.Container is null)
        {
            return;
        }

        var observer = new DelegatingObserver(_ => SyncGroupDocument(document));

        foreach (var layer in document.Container.Layers)
        {
            var subscription = layer.Subscribe(observer);
            tracker.LayerSubscriptions.Add(subscription);
        }
    }

    private void DetachGroupDocumentSubscriptions(BlockDocumentViewModel document)
    {
        if (_groupDocumentTrackers.TryGetValue(document, out var tracker))
        {
            tracker.Dispose();
            _groupDocumentTrackers.Remove(document);
        }
    }

    private void SyncGroupDocument(BlockDocumentViewModel document)
    {
        if (!_documentToGroup.TryGetValue(document, out var group))
        {
            return;
        }

        var container = document.Container;
        if (container is null)
        {
            return;
        }

        group.Shapes = ImmutableArray<BaseShapeViewModel>.Empty;
        group.Connectors = ImmutableArray<PointShapeViewModel>.Empty;

        var shared = new Dictionary<object, object>();
        var shapes = new List<BaseShapeViewModel>();
        var connectors = new List<PointShapeViewModel>();

        foreach (var layer in container.Layers)
        {
            foreach (var shape in layer.Shapes)
            {
                var copy = shape.CopyShared(shared);
                if (copy is null)
                {
                    continue;
                }

                if (copy is PointShapeViewModel point && point.State.HasFlag(ShapeStateFlags.Connector))
                {
                    connectors.Add(point);
                }
                else
                {
                    shapes.Add(copy);
                }
            }
        }

        foreach (var shape in shapes)
        {
            group.AddShape(shape);
        }

        foreach (var connector in connectors)
        {
            if (connector.State.HasFlag(ShapeStateFlags.Input))
            {
                group.AddConnectorAsInput(connector);
            }
            else if (connector.State.HasFlag(ShapeStateFlags.Output))
            {
                group.AddConnectorAsOutput(connector);
            }
            else
            {
                group.AddConnectorAsNone(connector);
            }
        }

        group.Invalidate();
    }

    private void PrunePageDocuments()
    {
        if (Project is null)
        {
            ClearPageDocuments();
            return;
        }

        var pages = Project.Documents.SelectMany(d => d.Pages).ToHashSet();

        foreach (var pair in _pageDocuments.ToArray())
        {
            if (!pages.Contains(pair.Key))
            {
                DetachDocument(pair.Value, collapse: true);
            }
            else
            {
                UpdatePageDocument(pair.Value, pair.Key);
            }
        }
    }

    private void PruneTemplateDocuments()
    {
        if (Project is null)
        {
            ClearTemplateDocuments();
            return;
        }

        var templates = Project.Templates.ToHashSet();

        foreach (var pair in _templateDocuments.ToArray())
        {
            if (!templates.Contains(pair.Key))
            {
                DetachTemplateDocument(pair.Value, collapse: true);
            }
            else
            {
                UpdateTemplateDocument(pair.Value, pair.Key);
            }
        }
    }

    private void PruneGroupDocuments()
    {
        if (Project is null)
        {
            ClearGroupDocuments();
            return;
        }

        var groups = Project.GroupLibraries
            .SelectMany(l => l.Items.OfType<BlockShapeViewModel>())
            .ToHashSet();

        foreach (var pair in _groupDocuments.ToArray())
        {
            if (!groups.Contains(pair.Key))
            {
                DetachGroupDocument(pair.Value, collapse: true);
            }
            else
            {
                UpdateGroupDocument(pair.Value, pair.Key);
            }
        }
    }

    private void ClearPageDocuments()
    {
        foreach (var document in _documentToPage.Keys.ToArray())
        {
            DetachDocument(document, collapse: true);
        }

        _pageDocuments.Clear();
        _documentToPage.Clear();

        ClearTemplateDocuments();
        ClearGroupDocuments();
    }

    private void ClearTemplateDocuments()
    {
        foreach (var document in _documentToTemplate.Keys.ToArray())
        {
            DetachTemplateDocument(document, collapse: true);
        }

        _templateDocuments.Clear();
        _documentToTemplate.Clear();
    }

    private void ClearGroupDocuments()
    {
        foreach (var document in _documentToGroup.Keys.ToArray())
        {
            DetachGroupDocument(document, collapse: true);
        }

        _groupDocuments.Clear();
        _documentToGroup.Clear();
        _containerToGroupDocument.Clear();
        _groupDocumentTrackers.Clear();
    }

    private void OnEditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RootDock) || e.PropertyName == nameof(DockFactory))
        {
            EnsurePagesDock();
        }
        else if (e.PropertyName == nameof(Project))
        {
            OnProjectAssigned();
        }
    }

    private void OnProjectAssigned()
    {
        if (Project is null)
        {
            ClearPageDocuments();
            return;
        }

        EnsurePagesDock();
        RemoveUntrackedDocuments();
        PrunePageDocuments();
        PruneTemplateDocuments();
        PruneGroupDocuments();
        SynchronizeDocumentsWithDock();

        switch (Project.CurrentContainer)
        {
            case PageContainerViewModel page:
                OpenPage(page);
                break;
            case TemplateContainerViewModel template:
                OpenTemplate(template);
                break;
        }
    }

    private void SynchronizeDocumentsWithDock()
    {
        if (_pagesDock?.VisibleDockables is not { } visibleDockables)
        {
            return;
        }

        var visible = visibleDockables.OfType<PageViewModel>().ToHashSet();

        foreach (var document in _documentToPage.Keys.ToArray())
        {
            if (!visible.Contains(document))
            {
                DetachDocument(document, collapse: false);
            }
        }

        var visibleTemplates = visibleDockables.OfType<TemplateViewModel>().ToHashSet();

        foreach (var document in _documentToTemplate.Keys.ToArray())
        {
            if (!visibleTemplates.Contains(document))
            {
                DetachTemplateDocument(document, collapse: false);
            }
        }

        var visibleGroups = visibleDockables.OfType<BlockDocumentViewModel>().ToHashSet();

        foreach (var document in _documentToGroup.Keys.ToArray())
        {
            if (!visibleGroups.Contains(document))
            {
                DetachGroupDocument(document, collapse: false);
            }
        }
    }

    private void RemoveUntrackedDocuments()
    {
        if (_pagesDock?.VisibleDockables is not { } visibleDockables)
        {
            return;
        }

        if (DockFactory is not FactoryBase factory)
        {
            return;
        }

        foreach (var document in visibleDockables.OfType<PageViewModel>().ToArray())
        {
            if (_documentToPage.ContainsKey(document))
            {
                continue;
            }

            DetachDocument(document, collapse: true);
        }

        foreach (var document in visibleDockables.OfType<TemplateViewModel>().ToArray())
        {
            if (_documentToTemplate.ContainsKey(document))
            {
                continue;
            }

            DetachTemplateDocument(document, collapse: true);
        }

        foreach (var document in visibleDockables.OfType<BlockDocumentViewModel>().ToArray())
        {
            if (_documentToGroup.ContainsKey(document))
            {
                continue;
            }

            DetachGroupDocument(document, collapse: true);
        }
    }

    private void HandleProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (sender)
        {
            case ProjectContainerViewModel project:
                if (e.PropertyName == nameof(ProjectContainerViewModel.CurrentContainer))
                {
                    switch (project.CurrentContainer)
                    {
                        case PageContainerViewModel currentPage when _containerToGroupDocument.TryGetValue(currentPage, out var currentGroupDocument) &&
                                                                     _documentToGroup.TryGetValue(currentGroupDocument, out var currentGroup):
                            OpenGroup(currentGroup);
                            break;
                        case PageContainerViewModel currentPage:
                            OpenPage(currentPage);
                            break;
                        case TemplateContainerViewModel currentTemplate:
                            OpenTemplate(currentTemplate);
                            break;
                    }
                }
                else if (e.PropertyName == nameof(ProjectContainerViewModel.Selected))
                {
                    switch (project.Selected)
                    {
                        case PageContainerViewModel selectedPage when _containerToGroupDocument.TryGetValue(selectedPage, out var selectedGroupDocument) &&
                                                                     _documentToGroup.TryGetValue(selectedGroupDocument, out var containerGroup):
                            OpenGroup(containerGroup);
                            break;
                        case PageContainerViewModel selectedPage:
                            OpenPage(selectedPage);
                            break;
                        case TemplateContainerViewModel selectedTemplate:
                            OpenTemplate(selectedTemplate);
                            break;
                        case BlockShapeViewModel selectedGroup:
                            OpenGroup(selectedGroup);
                            break;
                    }
                }
                else if (e.PropertyName == nameof(ProjectContainerViewModel.Documents))
                {
                    PrunePageDocuments();
                }
                else if (e.PropertyName == nameof(ProjectContainerViewModel.Templates))
                {
                    PruneTemplateDocuments();
                }
                else if (e.PropertyName == nameof(ProjectContainerViewModel.GroupLibraries))
                {
                    PruneGroupDocuments();
                }

                break;

            case DocumentContainerViewModel when e.PropertyName == nameof(DocumentContainerViewModel.Pages):
                PrunePageDocuments();
                break;

            case PageContainerViewModel page when e.PropertyName == nameof(PageContainerViewModel.Name):
                UpdatePageDocument(page);
                break;

            case TemplateContainerViewModel template when e.PropertyName == nameof(TemplateContainerViewModel.Name):
                UpdateTemplateDocument(template);
                break;

            case LibraryViewModel library when e.PropertyName == nameof(LibraryViewModel.Selected):
                if (library.Selected is BlockShapeViewModel librarySelectedGroup)
                {
                    OpenGroup(librarySelectedGroup);
                }

                break;

            case LibraryViewModel library when e.PropertyName == nameof(LibraryViewModel.Items):
                PruneGroupDocuments();
                break;
        }
    }

    private void UpdatePageDocument(PageContainerViewModel page)
    {
        if (_pageDocuments.TryGetValue(page, out var document))
        {
            UpdatePageDocument(document, page);
        }
    }

    private void UpdateTemplateDocument(TemplateContainerViewModel template)
    {
        if (_templateDocuments.TryGetValue(template, out var document))
        {
            UpdateTemplateDocument(document, template);
        }
    }

    internal bool IsDocumentDockCreationSuppressed => _suppressDocumentDockCreate > 0;

    private IDisposable SuppressDocumentDockCreation()
    {
        _suppressDocumentDockCreate++;
        return new DocumentDockSuppression(this);
    }

    private sealed class DocumentDockSuppression : IDisposable
    {
        private ProjectEditorViewModel? _owner;

        public DocumentDockSuppression(ProjectEditorViewModel owner)
        {
            _owner = owner;
        }

        public void Dispose()
        {
            if (_owner is null)
            {
                return;
            }

            if (_owner._suppressDocumentDockCreate > 0)
            {
                _owner._suppressDocumentDockCreate--;
            }

            _owner = null;
        }
    }

    private sealed class DelegatingObserver : IObserver<(object? sender, PropertyChangedEventArgs e)>
    {
        private readonly Action<(object? sender, PropertyChangedEventArgs e)> _onNext;

        public DelegatingObserver(Action<(object? sender, PropertyChangedEventArgs e)> onNext)
        {
            _onNext = onNext;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext((object? sender, PropertyChangedEventArgs e) value)
        {
            _onNext(value);
        }
    }

    private sealed class GroupDocumentTracker : IDisposable
    {
        public IDisposable? ContainerSubscription { get; set; }
        public List<IDisposable> LayerSubscriptions { get; } = new();

        public void Dispose()
        {
            ContainerSubscription?.Dispose();
            ContainerSubscription = null;

            foreach (var subscription in LayerSubscriptions)
            {
                subscription.Dispose();
            }

            LayerSubscriptions.Clear();
        }
    }
}
