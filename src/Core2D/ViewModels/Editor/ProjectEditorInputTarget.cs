using Core2D.Input;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor input target.
    /// </summary>
    public class ProjectEditorInputTarget : IInputTarget
    {
        private readonly ProjectEditor _editor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorInputTarget"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ProjectEditorInputTarget(ProjectEditor editor)
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public void LeftDown(InputArgs args) => _editor?.CurrentTool?.LeftDown(args);

        /// <inheritdoc/>
        public void LeftUp(InputArgs args) => _editor?.CurrentTool?.LeftUp(args);

        /// <inheritdoc/>
        public void RightDown(InputArgs args) => _editor?.CurrentTool?.RightDown(args);

        /// <inheritdoc/>
        public void RightUp(InputArgs args) => _editor?.CurrentTool?.RightUp(args);

        /// <inheritdoc/>
        public void Move(InputArgs args) => _editor?.CurrentTool?.Move(args);

        /// <inheritdoc/>
        public bool IsLeftDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        /// <inheritdoc/>
        public bool IsLeftUpAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        /// <inheritdoc/>
        public bool IsRightDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        /// <inheritdoc/>
        public bool IsRightUpAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        /// <inheritdoc/>
        public bool IsMoveAvailable()
        {
            return _editor.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        /// <inheritdoc/>
        public bool IsSelectionAvailable()
        {
            return _editor?.PageState?.SelectedShapes != null;
        }
    }
}
