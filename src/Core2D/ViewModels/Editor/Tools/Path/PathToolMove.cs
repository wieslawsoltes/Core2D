using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    public class PathToolMove : ViewModelBase, IPathTool
    {
        public enum State { Move }
        private readonly IServiceProvider _serviceProvider;
        private readonly State _currentState = State.Move;

        public string Title => "Move";

        public PathToolMove(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void LeftDown(InputArgs args)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (decimal sx, decimal sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.Move:
                    {
                        var pathTool = _serviceProvider.GetService<ToolPath>();
                        editor.CurrentPathTool = pathTool.PreviousPathTool;

                        var start = editor.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                        pathTool.GeometryContext.BeginFigure(
                                start,
                                editor.Project.Options.DefaultIsClosed);

                        editor.CurrentPathTool.LeftDown(args);
                    }
                    break;
            }
        }

        public void LeftUp(InputArgs args)
        {
        }

        public void RightDown(InputArgs args)
        {
        }

        public void RightUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
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

        public void Move(BaseShape shape)
        {
        }

        public void Finalize(BaseShape shape)
        {
        }

        public void Reset()
        {
        }
    }
}
