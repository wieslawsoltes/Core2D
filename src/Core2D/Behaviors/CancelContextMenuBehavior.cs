using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors
{
    public sealed class EnableContextMenuBehavior : Behavior<ContextMenu>
    {
        public static readonly StyledProperty<bool> IsEnabledProperty =
            AvaloniaProperty.Register<EnableContextMenuBehavior, bool>(nameof(IsEnabled), true);

        public bool IsEnabled
        {
            get => GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is ContextMenu contextMenu)
            {
                contextMenu.ContextMenuOpening += ContextMenu_ContextMenuOpening; 
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject is ContextMenu contextMenu)
            {
                contextMenu.ContextMenuOpening -= ContextMenu_ContextMenuOpening; 
            }
        }

        private void ContextMenu_ContextMenuOpening(object sender, CancelEventArgs e)
        {
            e.Cancel = !IsEnabled;
        }
    }
}
