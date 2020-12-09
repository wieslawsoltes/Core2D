using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class PointToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { Point }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point;
        private PointShapeViewModel _point;

        public string Title => "Point";

        public PointToolViewModel(IServiceProvider serviceProvider) : base()
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
                case State.Point:
                    {
                        _point = factory.CreatePointShape((double)sx, (double)sy);

                        if (editor.Project.OptionsViewModel.TryToConnect)
                        {
                            if (!editor.TryToSplitLine(args.X, args.Y, _point, true))
                            {
                                editor.Project.AddShape(editor.Project.CurrentContainerViewModel.CurrentLayer, _point);
                            }
                        }
                        else
                        {
                            editor.Project.AddShape(editor.Project.CurrentContainerViewModel.CurrentLayer, _point);
                        }
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
                case State.Point:
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
