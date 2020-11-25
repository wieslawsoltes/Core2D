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

        public void BeginDown(InputArgs args) => _editor?.CurrentTool?.BeginDown(args);

        public void BeginUp(InputArgs args) => _editor?.CurrentTool?.BeginUp(args);

        public void EndDown(InputArgs args) => _editor?.CurrentTool?.EndDown(args);

        public void EndUp(InputArgs args) => _editor?.CurrentTool?.EndUp(args);

        public void Move(InputArgs args) => _editor?.CurrentTool?.Move(args);

        public bool IsBeginDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsBeginUpAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsEndDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer != null
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsEndUpAvailable()
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
