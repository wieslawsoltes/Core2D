using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    public class MovePathToolViewModel : ViewModelBase, IPathTool
    {
        public enum State { Move }
        private readonly IServiceProvider _serviceProvider;
        private readonly State _currentState = State.Move;

        public string Title => "Move";

        public MovePathToolViewModel(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
                                editor.Project.OptionsViewModel.DefaultIsClosed);

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
                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
            }
        }

        public void Move(BaseShapeViewModel shapeViewModel)
        {
        }

        public void Finalize(BaseShapeViewModel shapeViewModel)
        {
        }

        public void Reset()
        {
        }
    }
}
