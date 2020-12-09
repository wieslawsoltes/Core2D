using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.Containers;
using Core2D.Editor;

namespace Core2D.DragAndDrop.Handlers
{
    public class ProjectTreeViewDropHandler : DefaultDropHandler
    {
        private bool ValidateContainer(TreeView treeView, DragEventArgs e, object sourceContext, object targetContext, bool bExecute)
        {
            if (!(sourceContext is BaseContainerViewModel sourceItem)
                || !(targetContext is ProjectContainerViewModel)
                || !(treeView.GetVisualAt(e.GetPosition(treeView)) is IControl targetControl)
                || !(treeView.GetVisualRoot() is IControl rootControl)
                || !(rootControl.DataContext is ProjectEditorViewModel editor)
                || !(targetControl.DataContext is BaseContainerViewModel targetItem))
            {
                return false;
            }

            Debug.WriteLine($"{sourceItem} -> {targetItem}");

            switch (sourceItem)
            {
                case LayerContainerViewModel sourceLayer:
                    {
                        switch (targetItem)
                        {
                            case LayerContainerViewModel targetLayer:
                                {
                                    if (bExecute)
                                    {
                                        // TODO:
                                    }
                                    return true;
                                }
                            case PageContainerViewModel targetPage:
                                {
                                    if (e.DragEffects == DragDropEffects.Copy)
                                    {
                                        if (bExecute)
                                        {
                                            var layer = editor?.Clone(sourceLayer);
                                            editor?.Project.AddLayer(targetPage, layer);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Move)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project?.RemoveLayer(sourceLayer);
                                            editor?.Project.AddLayer(targetPage, sourceLayer);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Link)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project.AddLayer(targetPage, sourceLayer);
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
                case PageContainerViewModel sourcePage:
                    {
                        switch (targetItem)
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
                                            var page = editor?.Clone(sourcePage);
                                            editor?.Project.AddPage(targetDocument, page);
                                            editor?.Project?.SetCurrentContainer(page);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Move)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project?.RemovePage(sourcePage);
                                            editor?.Project.AddPage(targetDocument, sourcePage);
                                            editor?.Project?.SetCurrentContainer(sourcePage);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Link)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project.AddPage(targetDocument, sourcePage);
                                            editor?.Project?.SetCurrentContainer(sourcePage);
                                        }
                                        return true;
                                    }
                                    return false;
                                }
                        }

                        return false;
                    }
                case DocumentContainerViewModel sourceDocument:
                    {
                        switch (targetItem)
                        {
                            case LayerContainerViewModel targetLayer:
                                {
                                    return false;
                                }
                            case PageContainerViewModel targetPage:
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
            }

            return false;
        }

        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is TreeView treeView)
            {
                return ValidateContainer(treeView, e, sourceContext, targetContext, false);
            }
            return false;
        }

        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is TreeView treeView)
            {
                return ValidateContainer(treeView, e, sourceContext, targetContext, true);
            }
            return false;
        }
    }
}
