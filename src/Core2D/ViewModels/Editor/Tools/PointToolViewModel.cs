using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Tools
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
