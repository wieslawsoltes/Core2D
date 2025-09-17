#nullable enable
using System;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels.Docking.Documents;
using Core2D.ViewModels.Editor;
using Dock.Model.Mvvm.Controls;

namespace Core2D.ViewModels.Docking.Docks;

public class PageDocumentDock : DocumentDock
{
    public PageDocumentDock()
    {
        CreateDocument = new RelayCommand(CreatePage);
    }

    private void CreatePage()
    {
        if (!CanCreateDocument)
        {
            return;
        }

        if (Context is ProjectEditorViewModel editor)
        {
            if (editor.IsDocumentDockCreationSuppressed)
            {
                return;
            }

            if (editor.Project is { })
            {
                editor.OnNew(null);
                return;
            }
        }

        var page = new PageViewModel()
        {
            Id = "PageDocument",
            Title = "Page"
        };

        Factory?.AddDockable(this, page);
        Factory?.SetActiveDockable(page);
        Factory?.SetFocusedDockable(this, page);
    }
}
