using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.DragAndDrop.Handlers
{
    public class ProjectTreeViewDropHandler : DefaultDropHandler
    {
        private bool IsContainer(object source)
        {
            return source switch
            {
                LayerContainerViewModel targetLayer => true,
                BaseContainerViewModel targetPage => true,
                DocumentContainerViewModel targetDocument => true,
                _ => false
            };
        }

        private bool ValidateContainer(TreeView treeView, DragEventArgs e, object sourceContext, object targetContext, bool bExecute)
        {
            if (!(IsContainer(sourceContext))
                || !(targetContext is ProjectContainerViewModel)
                || !(treeView.GetVisualAt(e.GetPosition(treeView)) is IControl targetControl)
                || !(treeView.GetVisualRoot() is IControl rootControl)
                || !(rootControl.DataContext is ProjectEditorViewModel editor)
                || !(IsContainer(targetControl.DataContext)))
            {
                return false;
            }

            Debug.WriteLine($"{sourceContext} -> {targetControl.DataContext}");

            switch (sourceContext)
            {
                case LayerContainerViewModel sourceLayer:
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
                            case BaseContainerViewModel targetContainer:
                                {
                                    if (e.DragEffects == DragDropEffects.Copy)
                                    {
                                        if (bExecute)
                                        {
                                            var layer = editor?.Clone(sourceLayer);
                                            editor?.Project.AddLayer(targetContainer, layer);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Move)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project?.RemoveLayer(sourceLayer);
                                            editor?.Project.AddLayer(targetContainer, sourceLayer);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Link)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project.AddLayer(targetContainer, sourceLayer);
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
                case PageContainerViewModel sourceContainer:
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
                                            var page = editor?.Clone(sourceContainer);
                                            editor?.Project.AddPage(targetDocument, page);
                                            editor?.Project?.SetCurrentContainer(page);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Move)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project?.RemovePage(sourceContainer);
                                            editor?.Project.AddPage(targetDocument, sourceContainer);
                                            editor?.Project?.SetCurrentContainer(sourceContainer);
                                        }
                                        return true;
                                    }
                                    else if (e.DragEffects == DragDropEffects.Link)
                                    {
                                        if (bExecute)
                                        {
                                            editor?.Project.AddPage(targetDocument, sourceContainer);
                                            editor?.Project?.SetCurrentContainer(sourceContainer);
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
                        switch (targetControl.DataContext)
                        {
                            case LayerContainerViewModel targetLayer:
                                {
                                    return false;
                                }
                            case BaseContainerViewModel targetContainer:
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
