using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.Containers;
using Core2D.Editor;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.DragAndDrop.Handlers
{
    public abstract class ListBoxDropHandler : DefaultDropHandler
    {
        internal bool ValidateLibrary<T>(ListBox listBox, DragEventArgs e, object sourceContext, object targetContext, bool bExecute) where T : ViewModelBase
        {
            if (!(sourceContext is T sourceItem) 
                || !(targetContext is Library<T> library) 
                || !(listBox.GetVisualAt(e.GetPosition(listBox)) is IControl targetControl)
                || !(listBox.GetVisualRoot() is IControl rootControl)
                || !(rootControl.DataContext is ProjectEditor editor)
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
                    var clone = (T)sourceItem.Copy(null);
                    clone.Name += "-copy";
                    editor.InsertItem(library, clone, targetIndex + 1);
                }
                return true;
            }
            else if (e.DragEffects == DragDropEffects.Move)
            {
                if (bExecute)
                {
                    editor.MoveItem(library, sourceIndex, targetIndex);
                }
                return true;
            }
            else if (e.DragEffects == DragDropEffects.Link)
            {
                if (bExecute)
                {
                    editor.SwapItem(library, sourceIndex, targetIndex);
                }
                return true;
            }

            return false;
        }
    }

    public class StylesListBoxDropHandler : ListBoxDropHandler
    {
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                return ValidateLibrary<ShapeStyle>(listBox, e, sourceContext, targetContext, false);
            }
            return false;
        }

        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                return ValidateLibrary<ShapeStyle>(listBox, e, sourceContext, targetContext, true);
            }
            return false;
        }
    }

    public class GroupsListBoxDropHandler : ListBoxDropHandler
    {
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                return ValidateLibrary<GroupShape>(listBox, e, sourceContext, targetContext, false);
            }
            return false;
        }

        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                return ValidateLibrary<GroupShape>(listBox, e, sourceContext, targetContext, true);
            }
            return false;
        }
    }

    public class TemplatesListBoxDropHandler : ListBoxDropHandler
    {
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                // TODO:
            }
            return false;
        }

        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                // TODO:
            }
            return false;
        }
    }

    public class ScriptsListBoxDropHandler : ListBoxDropHandler
    {
        public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                // TODO:
            }
            return false;
        }

        public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            if (e.Source is IControl && sender is ListBox listBox)
            {
                // TODO:
            }
            return false;
        }
    }
}
