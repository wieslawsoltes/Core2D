using Core2D.Input;

namespace Core2D.Editor
{
    public class ProjectEditorInputTarget : IInputTarget
    {
        private readonly ProjectEditor _editor;

        public ProjectEditorInputTarget(ProjectEditor editor)
        {
            _editor = editor;
        }

        public void LeftDown(InputArgs args) => _editor?.CurrentTool?.LeftDown(args);

        public void LeftUp(InputArgs args) => _editor?.CurrentTool?.LeftUp(args);

        public void RightDown(InputArgs args) => _editor?.CurrentTool?.RightDown(args);

        public void RightUp(InputArgs args) => _editor?.CurrentTool?.RightUp(args);

        public void Move(InputArgs args) => _editor?.CurrentTool?.Move(args);

        public bool IsLeftDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsLeftUpAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsRightDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsRightUpAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsMoveAvailable()
        {
            return _editor.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsSelectionAvailable()
        {
            return _editor?.PageState?.SelectedShapes != null;
        }
    }
}
