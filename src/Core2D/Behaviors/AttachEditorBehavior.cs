using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Editor;

namespace Core2D.Behaviors
{
    public class AttachEditorBehavior : Behavior<Control>
    {
        private ProjectEditorInput _input = null;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                _input = new ProjectEditorInput(AssociatedObject);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                _input?.Detach();
            }
        }
    }
}
