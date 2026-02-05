// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.Behaviors.DragAndDrop;

public class ProjectDataGridDropHandler : DefaultDropHandler
{
    private bool IsContainer(object? source)
    {
        return source switch
        {
            LayerContainerViewModel targetLayer => true,
            FrameContainerViewModel targetPage => true,
            DocumentContainerViewModel targetDocument => true,
            _ => false
        };
    }

    private static bool ValidateShape(DragEventArgs e, bool bExecute, Control targetControl, ProjectEditorViewModel editor, BaseShapeViewModel sourceShape)
    {
        switch (targetControl.DataContext)
        {
            case LayerContainerViewModel targetLayer:
            {
                if (e.DragEffects == DragDropEffects.Copy)
                {
                    if (bExecute)
                    {
                        var shape = sourceShape?.CopyShared(new Dictionary<object, object>());
                        editor.Project?.AddShape(targetLayer, shape);
                    }
                    return true;
                }
                else if (e.DragEffects == DragDropEffects.Move)
                {
                    if (bExecute)
                    {
                        editor.Project?.RemoveShape(sourceShape);
                        editor.Project?.AddShape(targetLayer, sourceShape);
                    }
                    return true;
                }
                else if (e.DragEffects == DragDropEffects.Link)
                {
                    if (bExecute)
                    {
                        editor.Project?.AddShape(targetLayer, sourceShape);
                        e.DragEffects = DragDropEffects.None;
                    }
                    return true;
                }
                return false;
            }
            case FrameContainerViewModel targetContainer:
            {
                if (bExecute)
                {
                    // TODO:
                }
                return true;
            }
            case DocumentContainerViewModel targetDocument:
            {
                if (bExecute)
                {
                    // TODO:
                }
                return true;
            }
        }

        return false;
    }

    private static bool ValidateLayer(DragEventArgs e, bool bExecute, Control targetControl, ProjectEditorViewModel editor, LayerContainerViewModel sourceLayer)
    {
        switch (targetControl.DataContext)
        {
            case LayerContainerViewModel targetLayer:
            {
                if (bExecute)
                {
                    // TODO:
                }
                return true;
            }
            case FrameContainerViewModel targetContainer:
            {
                if (e.DragEffects == DragDropEffects.Copy)
                {
                    if (bExecute)
                    {
                        var layer = sourceLayer?.CopyShared(new Dictionary<object, object>());
                        editor.Project?.AddLayer(targetContainer, layer);
                    }
                    return true;
                }
                else if (e.DragEffects == DragDropEffects.Move)
                {
                    if (bExecute)
                    {
                        editor.Project?.RemoveLayer(sourceLayer);
                        editor.Project?.AddLayer(targetContainer, sourceLayer);
                    }
                    return true;
                }
                else if (e.DragEffects == DragDropEffects.Link)
                {
                    if (bExecute)
                    {
                        editor.Project?.AddLayer(targetContainer, sourceLayer);
                        e.DragEffects = DragDropEffects.None;
                    }
                    return true;
                }
                return false;
            }
            case DocumentContainerViewModel targetDocument:
            {
                return false;
            }
        }

        return false;
    }

    private static bool ValidatePage(DragEventArgs e, bool bExecute, Control targetControl, ProjectEditorViewModel editor, PageContainerViewModel sourceContainer)
    {
        switch (targetControl.DataContext)
        {
            case LayerContainerViewModel targetLayer:
            {
                return false;
            }
            case PageContainerViewModel targetPage:
            {
                if (bExecute)
                {
                    // TODO:
                }
                return true;
            }
            case DocumentContainerViewModel targetDocument:
            {
                if (e.DragEffects == DragDropEffects.Copy)
                {
                    if (bExecute)
                    {
                        var page = sourceContainer?.CopyShared(new Dictionary<object, object>());
                        editor.Project?.AddPage(targetDocument, page);
                        editor.Project?.SetCurrentContainer(page);
                    }
                    return true;
                }
                else if (e.DragEffects == DragDropEffects.Move)
                {
                    if (bExecute)
                    {
                        editor.Project?.RemovePage(sourceContainer);
                        editor.Project?.AddPage(targetDocument, sourceContainer);
                        editor.Project?.SetCurrentContainer(sourceContainer);
                    }
                    return true;
                }
                else if (e.DragEffects == DragDropEffects.Link)
                {
                    if (bExecute)
                    {
                        editor.Project?.AddPage(targetDocument, sourceContainer);
                        editor.Project?.SetCurrentContainer(sourceContainer);
                    }
                    return true;
                }
                return false;
            }
        }

        return false;
    }

    private static bool ValidateDocument(bool bExecute, Control targetControl, ProjectEditorViewModel editor, DocumentContainerViewModel sourceDocument)
    {
        switch (targetControl.DataContext)
        {
            case LayerContainerViewModel targetLayer:
            {
                return false;
            }
            case FrameContainerViewModel targetContainer:
            {
                return false;
            }
            case DocumentContainerViewModel targetDocument:
            {
                if (bExecute)
                {
                    // TODO:
                }
                return true;
            }
        }
        return false;
    }

    private bool ValidateContainer(DataGrid dataGrid, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute)
    {
        var targetControl = GetTargetControl(dataGrid, e.GetPosition(dataGrid));
        if ((!IsContainer(sourceContext) && !(sourceContext is BaseShapeViewModel))
            || !(targetContext is ProjectContainerViewModel)
            || targetControl is null
            || !(dataGrid.GetVisualRoot() is Control rootControl)
            || !(rootControl.DataContext is ProjectEditorViewModel editor)
            || !(IsContainer(targetControl.DataContext)))
        {
            return false;
        }

        Debug.WriteLine($"{sourceContext} -> {targetControl.DataContext}");

        switch (sourceContext)
        {
            case BaseShapeViewModel sourceShape:
            {
                return ValidateShape(e, bExecute, targetControl, editor, sourceShape);
            }
            case LayerContainerViewModel sourceLayer:
            {
                return ValidateLayer(e, bExecute, targetControl, editor, sourceLayer);
            }
            case PageContainerViewModel sourcePage:
            {
                return ValidatePage(e, bExecute, targetControl, editor, sourcePage);
            }
            case DocumentContainerViewModel sourceDocument:
            {
                return ValidateDocument(bExecute, targetControl, editor, sourceDocument);
            }
        }

        return false;
    }

    private static Control? GetTargetControl(DataGrid dataGrid, Point position)
    {
        var visual = dataGrid.GetVisualAt(position);
        if (visual is null)
        {
            return null;
        }

        if (visual is DataGridRow row)
        {
            return row;
        }

        return visual.GetVisualAncestors().OfType<DataGridRow>().FirstOrDefault();
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is DataGrid dataGrid)
        {
            return ValidateContainer(dataGrid, e, sourceContext, targetContext, false);
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is DataGrid dataGrid)
        {
            return ValidateContainer(dataGrid, e, sourceContext, targetContext, true);
        }
        return false;
    }
}
