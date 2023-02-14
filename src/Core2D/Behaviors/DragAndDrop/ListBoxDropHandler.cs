#nullable enable
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.Behaviors.DragAndDrop;

public abstract class ListBoxDropHandler : DefaultDropHandler
{
    internal bool ValidateLibrary<T>(ListBox listBox, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute) where T : ViewModelBase
    {
        if (!(sourceContext is T sourceItem)
            || !(targetContext is LibraryViewModel library)
            || !(library.Items.All(x => x is T))
            || !(listBox.GetVisualAt(e.GetPosition(listBox)) is Control targetControl)
            || !(listBox.GetVisualRoot() is Control rootControl)
            || !(rootControl.DataContext is ProjectEditorViewModel editor)
            || !(targetControl.DataContext is T targetItem))
        {
            return false;
        }

        int sourceIndex = library.Items.IndexOf(sourceItem);
        int targetIndex = library.Items.IndexOf(targetItem);

        if (sourceIndex < 0 || targetIndex < 0)
        {
            return false;
        }

        if (e.DragEffects == DragDropEffects.Copy)
        {
            if (bExecute)
            {
                var clone = sourceItem.CopyShared(null);
                if (clone is { })
                {
                    clone.Name += "-copy";
                    editor.ShapeService?.InsertItem(library, clone, targetIndex + 1);
                }
            }
            return true;
        }
        else if (e.DragEffects == DragDropEffects.Move)
        {
            if (bExecute)
            {
                editor.ShapeService?.MoveItem(library, sourceIndex, targetIndex);
            }
            return true;
        }
        else if (e.DragEffects == DragDropEffects.Link)
        {
            if (bExecute)
            {
                editor.ShapeService?.SwapItem(library, sourceIndex, targetIndex);
            }
            return true;
        }

        return false;
    }
}
