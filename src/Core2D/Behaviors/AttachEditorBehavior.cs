using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Editor;

namespace Core2D.Behaviors
{
    /// <summary>
    /// Attaches <see cref="ProjectEditor"/> to a control.
    /// </summary>
    public class AttachEditorBehavior : Behavior<Control>
    {
        private ProjectEditorInput _input = null;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                _input = new ProjectEditorInput(AssociatedObject);
            }
        }

        /// <inheritdoc/>
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
