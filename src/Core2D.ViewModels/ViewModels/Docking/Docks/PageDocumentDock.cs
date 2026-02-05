// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels.Docking.Documents;
using Core2D.ViewModels.Editor;
using Dock.Model.ReactiveUI.Controls;

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
