#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Docking.Documents;
using Dock.Model;
using Dock.Model.Controls;
using Dock.Model.Core;

namespace Core2D.ViewModels.Editor;

public partial class ProjectEditorViewModel
{
    private readonly Dictionary<PageContainerViewModel, PageViewModel> _pageDocuments = new();
    private readonly Dictionary<PageViewModel, PageContainerViewModel> _documentToPage = new();
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

        SynchronizeDocumentsWithDock();
        PrunePageDocuments();

        if (_pagesDock is { } && Project?.CurrentContainer is PageContainerViewModel page)
        {
            OpenPage(page);
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
            if (dock.ActiveDockable is PageViewModel document &&
                _documentToPage.TryGetValue(document, out var page))
            {
                SyncProjectSelection(page);
            }
        }
        else if (e.PropertyName == nameof(IDock.VisibleDockables))
        {
            SynchronizeDocumentsWithDock();
        }
    }

    private void SyncProjectSelection(PageContainerViewModel page)
    {
        if (Project is null)
        {
            return;
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

    private void ClearPageDocuments()
    {
        foreach (var document in _documentToPage.Keys.ToArray())
        {
            DetachDocument(document, collapse: true);
        }

        _pageDocuments.Clear();
        _documentToPage.Clear();
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
        SynchronizeDocumentsWithDock();

        if (Project.CurrentContainer is PageContainerViewModel page)
        {
            OpenPage(page);
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
    }

    private void HandleProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (sender)
        {
            case ProjectContainerViewModel project:
                if (e.PropertyName == nameof(ProjectContainerViewModel.CurrentContainer) &&
                    project.CurrentContainer is PageContainerViewModel currentPage)
                {
                    OpenPage(currentPage);
                }
                else if (e.PropertyName == nameof(ProjectContainerViewModel.Selected) &&
                         project.Selected is PageContainerViewModel selectedPage)
                {
                    OpenPage(selectedPage);
                }
                else if (e.PropertyName == nameof(ProjectContainerViewModel.Documents))
                {
                    PrunePageDocuments();
                }

                break;

            case DocumentContainerViewModel when e.PropertyName == nameof(DocumentContainerViewModel.Pages):
                PrunePageDocuments();
                break;

            case PageContainerViewModel page when e.PropertyName == nameof(PageContainerViewModel.Name):
                UpdatePageDocument(page);
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
}
