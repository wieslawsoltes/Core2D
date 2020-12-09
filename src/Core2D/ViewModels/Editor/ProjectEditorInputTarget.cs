using Core2D.Input;

namespace Core2D.Editor
{
    public class ProjectEditorInputTarget : InputTarget
    {
        private readonly ProjectEditorViewModel _editorViewModel;

        public ProjectEditorInputTarget(ProjectEditorViewModel editorViewModel)
        {
            _editorViewModel = editorViewModel;
        }

        public override void BeginDown(InputArgs args) => _editorViewModel?.CurrentTool?.BeginDown(args);

        public override void BeginUp(InputArgs args) => _editorViewModel?.CurrentTool?.BeginUp(args);

        public override void EndDown(InputArgs args) => _editorViewModel?.CurrentTool?.EndDown(args);

        public override void EndUp(InputArgs args) => _editorViewModel?.CurrentTool?.EndUp(args);

        public override void Move(InputArgs args) => _editorViewModel?.CurrentTool?.Move(args);

        public override bool IsBeginDownAvailable()
        {
            return _editorViewModel?.Project?.CurrentContainerViewModel?.CurrentLayer != null
                && _editorViewModel.Project.CurrentContainerViewModel.CurrentLayer.IsVisible;
        }

        public override bool IsBeginUpAvailable()
        {
            return _editorViewModel?.Project?.CurrentContainerViewModel?.CurrentLayer != null
                && _editorViewModel.Project.CurrentContainerViewModel.CurrentLayer.IsVisible;
        }

        public override bool IsEndDownAvailable()
        {
            return _editorViewModel?.Project?.CurrentContainerViewModel?.CurrentLayer != null
                && _editorViewModel.Project.CurrentContainerViewModel.CurrentLayer.IsVisible;
        }

        public override bool IsEndUpAvailable()
        {
            return _editorViewModel?.Project?.CurrentContainerViewModel?.CurrentLayer != null
                && _editorViewModel.Project.CurrentContainerViewModel.CurrentLayer.IsVisible;
        }

        public override bool IsMoveAvailable()
        {
            return _editorViewModel.Project?.CurrentContainerViewModel?.CurrentLayer != null
                && _editorViewModel.Project.CurrentContainerViewModel.CurrentLayer.IsVisible;
        }

        public bool IsSelectionAvailable()
        {
            return _editorViewModel?.PageStateViewModel?.SelectedShapes != null;
        }
    }
}
