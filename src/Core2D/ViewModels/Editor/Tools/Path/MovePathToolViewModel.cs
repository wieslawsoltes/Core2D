#nullable disable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Tools.Path
{
    public partial class MovePathToolViewModel : ViewModelBase, IPathTool
    {
        public enum State { Move }
        private readonly State _currentState;

        public string Title => "Move";

        public MovePathToolViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentState = State.Move;
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        public void BeginDown(InputArgs args)
        {
            var factory = ServiceProvider.GetService<IViewModelFactory>();
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var selection = ServiceProvider.GetService<ISelectionService>();
            (decimal sx, decimal sy) = selection.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        var pathTool = ServiceProvider.GetService<PathToolViewModel>();
                        editor.CurrentPathTool = pathTool.PreviousPathTool;

                        var start = selection.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.BeginFigure(
                                start,
                                editor.Project.Options.DefaultIsClosed);

                        editor.CurrentPathTool.BeginDown(args);
                    }
                    break;
            }
        }

        public void BeginUp(InputArgs args)
        {
        }

        public void EndDown(InputArgs args)
        {
        }

        public void EndUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var selection = ServiceProvider.GetService<ISelectionService>();
            (decimal sx, decimal sy) = selection.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            selection.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
            }
        }

        public void Move(BaseShapeViewModel shape)
        {
        }

        public void Finalize(BaseShapeViewModel shape)
        {
        }

        public void Reset()
        {
        }
    }
}
