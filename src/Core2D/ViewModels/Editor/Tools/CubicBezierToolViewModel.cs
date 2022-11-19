#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools;

public partial class CubicBezierToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { Point1, Point4, Point2, Point3 }
    private State _currentState = State.Point1;
    private CubicBezierShapeViewModel? _cubicBezier;
    private BezierSelectionSelection? _selectionSelection;

    public string Title => "CubicBezier";

    public CubicBezierToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    private void NextPoint(InputArgs args)
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
                _cubicBezier = factory.CreateCubicBezierShape(
                    (double)sx, (double)sy,
                    (ShapeStyleViewModel)style.Copy(null),
                    editor.Project.Options.DefaultIsStroked,
                    editor.Project.Options.DefaultIsFilled);

                editor.SetShapeName(_cubicBezier);

                var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                if (result is { })
                {
                    _cubicBezier.Point1 = result;
                }

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_cubicBezier);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }
                ToStatePoint4();
                Move(_cubicBezier);
                _currentState = State.Point4;
                break;
            }
            case State.Point4:
            {
                if (_cubicBezier?.Point3 is { } && _cubicBezier?.Point4 is { })
                {
                    _cubicBezier.Point3.X = (double)sx;
                    _cubicBezier.Point3.Y = (double)sy;
                    _cubicBezier.Point4.X = (double)sx;
                    _cubicBezier.Point4.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _cubicBezier.Point4 = result;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    }

                    ToStatePoint2();
                    Move(_cubicBezier);
                    _currentState = State.Point2;
                }
                break;
            }
            case State.Point2:
            {
                if (_cubicBezier is { })
                {
                    _cubicBezier.Point2.X = (double)sx;
                    _cubicBezier.Point2.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _cubicBezier.Point2 = result;
                    }

                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    ToStatePoint3();
                    Move(_cubicBezier);
                    _currentState = State.Point3;
                }
                break;
            }
            case State.Point3:
            {
                if (_cubicBezier is { })
                {
                    _cubicBezier.Point3.X = (double)sx;
                    _cubicBezier.Point3.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _cubicBezier.Point3 = result;
                    }

                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                    Finalize(_cubicBezier);
                    editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _cubicBezier);

                    Reset();
                }
                break;
            }
        }
    }
    
    public void BeginDown(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.Project is null)
        {
            return;
        }

        if (editor.Project.Options?.SinglePressMode ?? true)
        {
            if (_currentState == State.Point1 || _currentState == State.Point4 || _currentState == State.Point2)
            {
                NextPoint(args);
            }
        }
        else
        {
            NextPoint(args);
        }
    }

    public void BeginUp(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.Project is null)
        {
            return;
        }

        if (editor.Project.Options?.SinglePressMode ?? true)
        {
            if (_currentState == State.Point4 || _currentState == State.Point2 || _currentState == State.Point3)
            {
                NextPoint(args);
            }
        }
    }

    public void EndDown(InputArgs args)
    {
        switch (_currentState)
        {
            case State.Point1:
                break;
            case State.Point4:
            case State.Point2:
            case State.Point3:
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
            case State.Point4:
            {
                if (_cubicBezier is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _cubicBezier.Point2.X = (double)sx;
                    _cubicBezier.Point2.Y = (double)sy;
                    _cubicBezier.Point3.X = (double)sx;
                    _cubicBezier.Point3.Y = (double)sy;
                    _cubicBezier.Point4.X = (double)sx;
                    _cubicBezier.Point4.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_cubicBezier);
                }
                break;
            }
            case State.Point2:
            {
                if (_cubicBezier is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _cubicBezier.Point2.X = (double)sx;
                    _cubicBezier.Point2.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_cubicBezier);
                }
                break;
            }
            case State.Point3:
            {
                if (_cubicBezier is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _cubicBezier.Point3.X = (double)sx;
                    _cubicBezier.Point3.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_cubicBezier);
                }
                break;
            }
        }
    }

    public void ToStatePoint4()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        _selectionSelection = new BezierSelectionSelection(
            ServiceProvider,
            editor.Project.CurrentContainer.HelperLayer,
            _cubicBezier,
            editor.PageState.HelperStyle);

        _selectionSelection.ToStatePoint4();
    }

    public void ToStatePoint2()
    {
        _selectionSelection?.ToStatePoint2();
    }

    public void ToStatePoint3()
    {
        _selectionSelection?.ToStatePoint3();
    }

    public void Move(BaseShapeViewModel? shape)
    {
        _selectionSelection?.Move();
    }

    public void Finalize(BaseShapeViewModel? shape)
    {
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
            case State.Point4:
            case State.Point2:
            case State.Point3:
            {
                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                    editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                }
                break;
            }
        }

        _currentState = State.Point1;

        if (_selectionSelection is { })
        {
            _selectionSelection.Reset();
            _selectionSelection = null;
        }

        editor.IsToolIdle = true;
    }
}
