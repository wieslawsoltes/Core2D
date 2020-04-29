using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Core2D.UI.DragAndDrop
{
    /// <summary>
    /// Drop behavior.
    /// </summary>
    public sealed class ContextDropBehavior : Behavior<Control>
    {
        /// <summary>
        /// Context data format.
        /// </summary>
        public static string DataFormat = nameof(Context);

        /// <summary>
        /// Define <see cref="Context"/> property.
        /// </summary>
        public static readonly AvaloniaProperty ContextProperty =
            AvaloniaProperty.Register<ContextDropBehavior, object>(nameof(Context));

        /// <summary>
        /// Define <see cref="Handler"/> property.
        /// </summary>
        public static readonly AvaloniaProperty HandlerProperty =
            AvaloniaProperty.Register<ContextDropBehavior, IDropHandler>(nameof(Handler));

        /// <summary>
        /// Gets or sets drag behavior context.
        /// </summary>
        public object Context
        {
            get => GetValue(ContextProperty);
            set => SetValue(ContextProperty, value);
        }

        /// <summary>
        /// Gets or sets drop handler.
        /// </summary>
        public IDropHandler Handler
        {
            get => (IDropHandler)GetValue(HandlerProperty);
            set => SetValue(HandlerProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            DragDrop.SetAllowDrop(AssociatedObject, false);
            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            object sourceContext = e.Data.Get(ContextDropBehavior.DataFormat);
            object targetContext = Context;
            Handler?.Enter(sender, e, sourceContext, targetContext);
        }

        private void DragLeave(object sender, RoutedEventArgs e)
        {
            Handler?.Leave(sender, e);
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            object sourceContext = e.Data.Get(ContextDropBehavior.DataFormat);
            object targetContext = Context;
            Handler?.Over(sender, e, sourceContext, targetContext);
        }

        private void Drop(object sender, DragEventArgs e)
        {
            object sourceContext = e.Data.Get(ContextDropBehavior.DataFormat);
            object targetContext = Context;
            Handler?.Drop(sender, e, sourceContext, targetContext);
        }
    }
}
