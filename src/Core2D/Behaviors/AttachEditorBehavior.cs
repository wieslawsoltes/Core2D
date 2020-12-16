using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors
{
    public class AttachEditorBehavior : Behavior<Control>
    {
        private ProjectEditorInput _input;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is { })
            {
                _input = new ProjectEditorInput(AssociatedObject);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject is { })
            {
                _input?.Detach();
            }
        }
    }
}
