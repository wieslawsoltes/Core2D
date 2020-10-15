using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Core2D.UI.DragAndDrop
{
    /// <summary>
    /// Default drop handler.
    /// </summary>
    public abstract class DefaultDropHandler : IDropHandler
    {
        /// <summary>
        /// Calculates fixed drag position relative to event source.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>The fixed drag position relative to event source.</returns>
        public static Point GetPosition(object sender, DragEventArgs e)
        {
            var relativeTo = e.Source as IControl;
            var point = e.GetPosition(relativeTo);
            return point;
        }

        /// <summary>
        /// Calculates fixed drag position relative to event source and translated to screen coordinates.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>The fixed drag position relative to event source and translated to screen coordinates.</returns>
        public static Point GetPositionScreen(object sender, DragEventArgs e)
        {
            var relativeTo = e.Source as IControl;
            var point = e.GetPosition(relativeTo);
            var visual = relativeTo as IVisual;
            var screenPoint = visual.PointToScreen(point).ToPoint(1.0);
            return screenPoint;
        }

        /// <inheritdoc/>
        public virtual void Enter(object sender, DragEventArgs e, object sourceContext, object targetContext)
        {
            if (Validate(sender, e, sourceContext, targetContext, null) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
                e.Handled = true;
            }
        }

        /// <inheritdoc/>
        public virtual void Over(object sender, DragEventArgs e, object sourceContext, object targetContext)
        {
            if (Validate(sender, e, sourceContext, targetContext, null) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
                e.Handled = true;
            }
        }

        /// <inheritdoc/>
        public virtual void Drop(object sender, DragEventArgs e, object sourceContext, object targetContext)
        {
            if (Execute(sender, e, sourceContext, targetContext, null) == false)
            {
                e.DragEffects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
                e.Handled = true;
            }
        }

        /// <inheritdoc/>
        public virtual void Leave(object sender, RoutedEventArgs e)
        {
            Cancel(sender, e);
        }

        /// <inheritdoc/>
        public virtual bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            return false;
        }

        /// <inheritdoc/>
        public virtual bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext, object state)
        {
            return false;
        }

        /// <inheritdoc/>
        public virtual void Cancel(object sender, RoutedEventArgs e)
        {
        }
    }
}
