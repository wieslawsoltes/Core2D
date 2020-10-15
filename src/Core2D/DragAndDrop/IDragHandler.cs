using Avalonia.Input;

namespace Core2D.UI.DragAndDrop
{
    /// <summary>
    /// Drag handler contract.
    /// </summary>
    public interface IDragHandler
    {
        /// <summary>
        /// Called before drag starts.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Pointer event arguments.</param>
        /// <param name="context">Drag behavior context.</param>
        void BeforeDragDrop(object sender, PointerEventArgs e, object context);

        /// <summary>
        /// Called after drag finished.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Pointer event arguments.</param>
        /// <param name="context">Drag behavior context.</param>
        void AfterDragDrop(object sender, PointerEventArgs e, object context);
    }
}
