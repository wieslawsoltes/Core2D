using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors
{
    public class ShowOnKeyDownTappedBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<Control> TargetControlProperty =
            AvaloniaProperty.Register<ShowOnKeyDownTappedBehavior, Control>(nameof(TargetControl));

        public static readonly StyledProperty<Key> KeyProperty =
            AvaloniaProperty.Register<ShowOnKeyDownTappedBehavior, Key>(nameof(Key), Key.F2);

        public Control TargetControl
        {
            get => GetValue(TargetControlProperty);
            set => SetValue(TargetControlProperty, value);
        }
        
        public Key Key
        {
            get => GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject?.AddHandler(InputElement.KeyDownEvent, AssociatedObject_KeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, AssociatedObject_KeyDown);
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key)
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
}
