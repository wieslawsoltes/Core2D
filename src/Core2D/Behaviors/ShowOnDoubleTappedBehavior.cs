#nullable disable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors
{
    public class ShowOnDoubleTappedBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<Control> TargetControlProperty =
            AvaloniaProperty.Register<ShowOnDoubleTappedBehavior, Control>(nameof(TargetControl));

        public Control TargetControl
        {
            get => GetValue(TargetControlProperty);
            set => SetValue(TargetControlProperty, value);
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject?.AddHandler(Gestures.DoubleTappedEvent, AssociatedObject_DoubleTapped, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject?.RemoveHandler(Gestures.DoubleTappedEvent, AssociatedObject_DoubleTapped);
        }

        private void AssociatedObject_DoubleTapped(object sender, RoutedEventArgs e)
        {
            if (TargetControl is { })
            {
                if (!TargetControl.IsVisible)
                {
                    TargetControl.IsVisible = true;
                    TargetControl.Focus();
                }
            }
        }
    }
}
