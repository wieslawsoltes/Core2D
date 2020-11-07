using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class ToolPoint : ViewModelBase, IEditorTool
    {
        public enum State { Point }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point;
        private PointShape _point;

        public string Title => "Point";

        public ToolPoint(IServiceProvider serviceProvider) : base()
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
                case State.Point:
                    {
                        _point = factory.CreatePointShape((double)sx, (double)sy);

                        if (editor.Project.Options.TryToConnect)
                        {
                            if (!editor.TryToSplitLine(args.X, args.Y, _point, true))
                            {
                                editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _point);
                            }
                        }
                        else
                        {
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _point);
                        }
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
                case State.Point:
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
