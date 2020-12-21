#nullable disable
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors
{
    public sealed class EnableContextMenuBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<bool> IsEnabledProperty =
            AvaloniaProperty.Register<EnableContextMenuBehavior, bool>(nameof(IsEnabled), true);

        public static readonly StyledProperty<ContextMenu> ContextMenuProperty =
            AvaloniaProperty.Register<EnableContextMenuBehavior, ContextMenu>(nameof(ContextMenu));

        public bool IsEnabled
        {
            get => GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public ContextMenu ContextMenu
        {
            get => GetValue(ContextMenuProperty);
            set => SetValue(ContextMenuProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if ((ContextMenu ?? AssociatedObject?.ContextMenu) is { } contextMenu)
            {
                contextMenu.ContextMenuOpening += ContextMenu_ContextMenuOpening; 
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if ((ContextMenu ?? AssociatedObject?.ContextMenu) is { } contextMenu)
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
