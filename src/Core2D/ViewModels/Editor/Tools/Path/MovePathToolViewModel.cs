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
        private readonly IServiceProvider _serviceProvider;
        private readonly State _currentState = State.Move;

        public string Title => "Move";

        public MovePathToolViewModel(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public void BeginDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        var pathTool = _serviceProvider.GetService<PathToolViewModel>();
                        editor.CurrentPathTool = pathTool.PreviousPathTool;

                        var start = editor.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
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
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
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
