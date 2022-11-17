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

public partial class QuadraticBezierToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { Point1, Point3, Point2 }
    private State _currentState = State.Point1;
    private QuadraticBezierShapeViewModel? _quadraticBezier;
    private QuadraticBezierSelection? _selection;

    public string Title => "QuadraticBezier";

    public QuadraticBezierToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
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
                _quadraticBezier = factory.CreateQuadraticBezierShape(
                    (double)sx, (double)sy,
                    (ShapeStyleViewModel)style.Copy(null),
                    editor.Project.Options.DefaultIsStroked,
                    editor.Project.Options.DefaultIsFilled);

                editor.SetShapeName(_quadraticBezier);

                var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                if (result is { })
                {
                    _quadraticBezier.Point1 = result;
                }

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_quadraticBezier);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }
                
                ToStatePoint3();
                Move(_quadraticBezier);
                _currentState = State.Point3;
            }
                break;
            case State.Point3:
            {
                if (_quadraticBezier is { })
                {
                    _quadraticBezier.Point2.X = (double)sx;
                    _quadraticBezier.Point2.Y = (double)sy;
                    _quadraticBezier.Point3.X = (double)sx;
                    _quadraticBezier.Point3.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _quadraticBezier.Point3 = result;
                    }

                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    ToStatePoint2();
                    Move(_quadraticBezier);
                    _currentState = State.Point2;
                }
            }
                break;
            case State.Point2:
            {
                if (_quadraticBezier is { })
                {
                    _quadraticBezier.Point2.X = (double)sx;
                    _quadraticBezier.Point2.Y = (double)sy;

                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (result is { })
                    {
                        _quadraticBezier.Point2 = result;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_quadraticBezier);
                    }
                    
                    Finalize(_quadraticBezier);

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _quadraticBezier);
                    }

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
            case State.Point3:
            case State.Point2:
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
            }
                break;
            case State.Point3:
            {
                if (_quadraticBezier is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _quadraticBezier.Point2.X = (double)sx;
                    _quadraticBezier.Point2.Y = (double)sy;
                    _quadraticBezier.Point3.X = (double)sx;
                    _quadraticBezier.Point3.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_quadraticBezier);
                }
            }
                break;
            case State.Point2:
            {
                if (_quadraticBezier is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    _quadraticBezier.Point2.X = (double)sx;
                    _quadraticBezier.Point2.Y = (double)sy;
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_quadraticBezier);
                }
            }
                break;
        }
    }

    public void ToStatePoint3()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        _selection = new QuadraticBezierSelection(
            ServiceProvider,
            editor.Project.CurrentContainer.HelperLayer,
            _quadraticBezier,
            editor.PageState.HelperStyle);

        _selection.ToStatePoint3();
    }

    public void ToStatePoint2()
    {
        _selection?.ToStatePoint2();
    }

    public void Move(BaseShapeViewModel? shape)
    {
        _selection?.Move();
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
            case State.Point3:
            case State.Point2:
            {
                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_quadraticBezier);
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
