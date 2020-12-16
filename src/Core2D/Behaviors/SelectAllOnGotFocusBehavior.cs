using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors
{
    public class SelectAllOnGotFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject?.AddHandler(InputElement.GotFocusEvent, AssociatedObject_GotFocus, RoutingStrategies.Bubble);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject?.RemoveHandler(InputElement.GotFocusEvent, AssociatedObject_GotFocus);
        }

        private void AssociatedObject_GotFocus(object sender, GotFocusEventArgs e)
        {
            AssociatedObject?.SelectAll();
        }
    }
}
