using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Spatial;
using Spatial.Arc;

namespace Core2D.ViewModels.Editor.Tools
{
    public class ArcToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { Point1, Point2, Point3, Point4 }
        private readonly IServiceProvider _serviceProvider;
        private State _currentState = State.Point1;
        private ArcShapeViewModelViewModel _arc;
        private bool _connectedPoint3;
        private bool _connectedPoint4;
        private ArcSelection _selection;

        public string Title => "Arc";

        public ArcToolViewModel(IServiceProvider serviceProvider) : base()
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
                case State.Point1:
                    {
                        editor.IsToolIdle = false;
                        var style = editor.Project.CurrentStyleLibrary?.Selected != null ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            editor.Factory.CreateShapeStyle(ProjectEditorConfigurationViewModel.DefaulStyleName);
                        _connectedPoint3 = false;
                        _connectedPoint4 = false;
                        _arc = factory.CreateArcShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result != null)
                        {
                            _arc.Point1 = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                        ToStatePoint2();
                        Move(_arc);
                        _currentState = State.Point2;
                    }
                    break;
                case State.Point2:
                    {
                        if (_arc != null)
                        {
                            _arc.Point2.X = (double)sx;
                            _arc.Point2.Y = (double)sy;
                            _arc.Point3.X = (double)sx;
                            _arc.Point3.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _arc.Point2 = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            ToStatePoint3();
                            Move(_arc);
                            _currentState = State.Point3;
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_arc != null)
                        {
                            _arc.Point3.X = (double)sx;
                            _arc.Point3.Y = (double)sy;
                            _arc.Point4.X = (double)sx;
                            _arc.Point4.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _arc.Point3 = result;
                                _connectedPoint3 = true;
                            }
                            else
                            {
                                _connectedPoint3 = false;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_arc);
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            ToStatePoint4();
                            Move(_arc);
                            _currentState = State.Point4;
                        }
                    }
                    break;
                case State.Point4:
                    {
                        if (_arc != null)
                        {
                            _arc.Point4.X = (double)sx;
                            _arc.Point4.Y = (double)sy;

                            var result = editor.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result != null)
                            {
                                _arc.Point4 = result;
                                _connectedPoint4 = true;
                            }
                            else
                            {
                                _connectedPoint4 = false;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
                            Finalize(_arc);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _arc);

                            Reset();
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
            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point2:
                case State.Point3:
                case State.Point4:
                    Reset();
                    break;
            }
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
                case State.Point1:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape((double)sx, (double)sy);
                        }
                    }
                    break;
                case State.Point2:
                    {
                        if (_arc != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _arc.Point2.X = (double)sx;
                            _arc.Point2.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_arc);
                        }
                    }
                    break;
                case State.Point3:
                    {
                        if (_arc != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _arc.Point3.X = (double)sx;
                            _arc.Point3.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_arc);
                        }
                    }
                    break;
                case State.Point4:
                    {
                        if (_arc != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape((double)sx, (double)sy);
                            }
                            _arc.Point4.X = (double)sx;
                            _arc.Point4.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                            Move(_arc);
                        }
                    }
                    break;
            }
        }

        public void ToStatePoint2()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            _selection = new ArcSelection(
                _serviceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _arc,
                editor.PageState.HelperStyle);

            _selection.ToStatePoint2();
        }

        public void ToStatePoint3()
        {
            _selection.ToStatePoint3();
        }

        public void ToStatePoint4()
        {
            _selection.ToStatePoint4();
        }

        public void Move(BaseShapeViewModel shape)
        {
            _selection.Move();
        }

        public void Finalize(BaseShapeViewModel shape)
        {
            var arc = shape as ArcShapeViewModelViewModel;
            var a = new WpfArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            if (!_connectedPoint3)
            {
                arc.Point3.X = a.Start.X;
                arc.Point3.Y = a.Start.Y;
            }

            if (!_connectedPoint4)
            {
                arc.Point4.X = a.End.X;
                arc.Point4.Y = a.End.Y;
            }
        }

        public void Reset()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            switch (_currentState)
            {
                case State.Point1:
                    break;
                case State.Point2:
                case State.Point3:
                case State.Point4:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
                        editor.Project.CurrentContainer.WorkingLayer.InvalidateLayer();
                    }
                    break;
            }

            _currentState = State.Point1;

            if (_selection != null)
            {
                _selection.Reset();
                _selection = null;
            }
            
            editor.IsToolIdle = true;
        }
    }
}
