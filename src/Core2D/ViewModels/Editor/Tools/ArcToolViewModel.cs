#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;
using Core2D.Spatial.Arc;

namespace Core2D.ViewModels.Editor.Tools;

public partial class ArcToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { Point1, Point2, Point3, Point4 }
    private State _currentState = State.Point1;
    private ArcShapeViewModel? _arc;
    private bool _connectedPoint3;
    private bool _connectedPoint4;
    private ArcSelection? _selection;

    public string Title => "Arc";

    public ArcToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
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
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        if (factory is null || editor?.Project?.Options is null || selection is null || viewModelFactory is null)
        {
            return;
        }
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.Point1:
            {
                editor.IsToolIdle = false;
                var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                    editor.Project.CurrentStyleLibrary.Selected :
                    viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
                _connectedPoint3 = false;
                _connectedPoint4 = false;
                _arc = factory.CreateArcShape(
                    (double)sx, (double)sy,
                    (ShapeStyleViewModel)style.Copy(null),
                    editor.Project.Options.DefaultIsStroked,
                    editor.Project.Options.DefaultIsFilled);

                editor.SetShapeName(_arc);

                var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                if (result is { })
                {
                    _arc.Point1 = result;
                }

                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                ToStatePoint2();
                Move(_arc);
                _currentState = State.Point2;
                break;
            }
            case State.Point2:
            {
                if (_arc is { })
                {
                    _arc.Point2.X = (double)sx;
                    _arc.Point2.Y = (double)sy;
                    _arc.Point3.X = (double)sx;
                    _arc.Point3.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _arc.Point2 = result;
                    }

                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    ToStatePoint3();
                    Move(_arc);
                    _currentState = State.Point3;
                }
                break;
            }
            case State.Point3:
            {
                if (_arc is { })
                {
                    _arc.Point3.X = (double)sx;
                    _arc.Point3.Y = (double)sy;
                    _arc.Point4.X = (double)sx;
                    _arc.Point4.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _arc.Point3 = result;
                        _connectedPoint3 = true;
                    }
                    else
                    {
                        _connectedPoint3 = false;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_arc);
                        editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    }
                    ToStatePoint4();
                    Move(_arc);
                    _currentState = State.Point4;
                }
                break;
            }
            case State.Point4:
            {
                if (_arc is { })
                {
                    _arc.Point4.X = (double)sx;
                    _arc.Point4.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _arc.Point4 = result;
                        _connectedPoint4 = true;
                    }
                    else
                    {
                        _connectedPoint4 = false;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
                    }

                    Finalize(_arc);

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _arc);
                    }

                    Reset();
                }
                break;
            }
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
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        if (editor?.Project?.Options is null || selection is null)
        {
            return;
        }
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.Point1:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                break;
            }
            case State.Point2:
            {
                if (_arc is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _arc.Point2.X = (double)sx;
                    _arc.Point2.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_arc);
                }
                break;
            }
            case State.Point3:
            {
                if (_arc is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _arc.Point3.X = (double)sx;
                    _arc.Point3.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_arc);
                }
                break;
            }
            case State.Point4:
            {
                if (_arc is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _arc.Point4.X = (double)sx;
                    _arc.Point4.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_arc);
                }
                break;
            }
        }
    }

    public void ToStatePoint2()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            
        if (editor?.Project?.Options is null)
        {
            return;
        }
            
        _selection = new ArcSelection(
            ServiceProvider,
            editor.Project.CurrentContainer.HelperLayer,
            _arc,
            editor.PageState.HelperStyle);

        _selection.ToStatePoint2();
    }

    public void ToStatePoint3()
    {
        _selection?.ToStatePoint3();
    }

    public void ToStatePoint4()
    {
        _selection?.ToStatePoint4();
    }

    public void Move(BaseShapeViewModel? shape)
    {
        _selection?.Move();
    }

    public void Finalize(BaseShapeViewModel? shape)
    {
        var arc = shape as ArcShapeViewModel;
        if (arc?.Point1 is null || arc.Point2 is null || arc.Point3 is null || arc.Point4 is null)
        {
            return;
        }
        
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
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.Project is null)
        {
            return;
        }

        switch (_currentState)
        {
            case State.Point1:
                break;
            case State.Point2:
            case State.Point3:
            case State.Point4:
            {
                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_arc);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }
                break;
            }
        }

        _currentState = State.Point1;

        if (_selection is { })
        {
            _selection.Reset();
            _selection = null;
        }

        editor.IsToolIdle = true;
    }
}
