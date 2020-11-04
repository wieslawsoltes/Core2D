using Avalonia.Input;

namespace Core2D.DragAndDrop
{
    public interface IDragHandler
    {
        void BeforeDragDrop(object sender, PointerEventArgs e, object context);

        void AfterDragDrop(object sender, PointerEventArgs e, object context);
    }
}
