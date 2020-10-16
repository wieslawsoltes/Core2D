using Avalonia.Input;
using Avalonia.Interactivity;

namespace Core2D.DragAndDrop
{
    public interface IDropHandler
    {
        void Enter(object sender, DragEventArgs e, object sourceContext, object targetContext);

        void Over(object sender, DragEventArgs e, object sourceContext, object targetContext);

        void Drop(object sender, DragEventArgs e, object sourceContext, object targetContext);

        void Leave(object sender, RoutedEventArgs e);

        bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state);

        bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state);

        void Cancel(object sender, RoutedEventArgs e);
    }
}
