#nullable disable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors
{
    public class HideOnLostFocusBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<Control> TargetControlProperty =
            AvaloniaProperty.Register<HideOnLostFocusBehavior, Control>(nameof(TargetControl));

        public Control TargetControl
        {
            get => GetValue(TargetControlProperty);
            set => SetValue(TargetControlProperty, value);
        }


        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject?.AddHandler(InputElement.LostFocusEvent, AssociatedObject_LostFocus, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject?.RemoveHandler(InputElement.LostFocusEvent, AssociatedObject_LostFocus);
        }

        private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TargetControl is { })
            {
                TargetControl.IsVisible = false;
            }
        }
    }
}
