#nullable disable
using Core2D.Model.Input;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorInputTarget : InputTarget
    {
        private readonly ProjectEditorViewModel _editor;

        public ProjectEditorInputTarget(ProjectEditorViewModel editor)
        {
            _editor = editor;
        }

        public override void BeginDown(InputArgs args) => _editor?.CurrentTool?.BeginDown(args);

        public override void BeginUp(InputArgs args) => _editor?.CurrentTool?.BeginUp(args);

        public override void EndDown(InputArgs args) => _editor?.CurrentTool?.EndDown(args);

        public override void EndUp(InputArgs args) => _editor?.CurrentTool?.EndUp(args);

        public override void Move(InputArgs args) => _editor?.CurrentTool?.Move(args);

        public override bool IsBeginDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer is { }
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public override bool IsBeginUpAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer is { }
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public override bool IsEndDownAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer is { }
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public override bool IsEndUpAvailable()
        {
            return _editor?.Project?.CurrentContainer?.CurrentLayer is { }
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public override bool IsMoveAvailable()
        {
            return _editor.Project?.CurrentContainer?.CurrentLayer is { }
                && _editor.Project.CurrentContainer.CurrentLayer.IsVisible;
        }

        public bool IsSelectionAvailable()
        {
            return _editor?.Project?.SelectedShapes is { };
        }
    }
}
