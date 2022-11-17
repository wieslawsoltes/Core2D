#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Tools.Path;

public partial class QuadraticBezierPathToolViewModel : ViewModelBase, IPathTool
{
    public enum State { Point1, Point3, Point2 }
    private State _currentState;
    private readonly QuadraticBezierShapeViewModel _quadraticBezier;
    private QuadraticBezierSelection? _selection;

    public string Title => "QuadraticBezier";

    public QuadraticBezierPathToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _currentState = State.Point1;
        _quadraticBezier = new QuadraticBezierShapeViewModel(serviceProvider);
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
        var pathTool = ServiceProvider.GetService<PathToolViewModel>();
        if (factory is null || editor?.Project?.Options is null || selection is null || pathTool is null)
        {
            return;
        }
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.Point1:
            {
                editor.IsToolIdle = false;
                _quadraticBezier.Point1 = selection.TryToGetConnectionPoint((double)sx, (double)sy) ?? factory.CreatePointShape((double)sx, (double)sy);
                if (!pathTool.IsInitialized)
                {
                    pathTool.InitializeWorkingPath(_quadraticBezier.Point1);
                }
                else
                {
                    _quadraticBezier.Point1 = pathTool.GetLastPathPoint();
                }

                _quadraticBezier.Point2 = factory.CreatePointShape((double)sx, (double)sy);
                _quadraticBezier.Point3 = factory.CreatePointShape((double)sx, (double)sy);
                pathTool.GeometryContext.QuadraticBezierTo(
                    _quadraticBezier.Point2,
                    _quadraticBezier.Point3);
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                ToStatePoint3();
                Move(null);
                _currentState = State.Point3;
            }
                break;
            case State.Point3:
            {
                _quadraticBezier.Point3.X = (double)sx;
                _quadraticBezier.Point3.Y = (double)sy;
                if (editor.Project.Options.TryToConnect)
                {
                    var point2 = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (point2 is { })
                    {
                        var figure = pathTool.Path.Figures.LastOrDefault();
                        var quadraticBezier = figure.Segments.LastOrDefault() as QuadraticBezierSegmentViewModel;
                        quadraticBezier.Point2 = point2;
                        _quadraticBezier.Point3 = point2;
                    }
                }
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                ToStatePoint2();
                Move(null);
                _currentState = State.Point2;
            }
                break;
            case State.Point2:
            {
                _quadraticBezier.Point2.X = (double)sx;
                _quadraticBezier.Point2.Y = (double)sy;
                if (editor.Project.Options.TryToConnect)
                {
                    var point1 = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                    if (point1 is { })
                    {
                        var figure = pathTool.Path.Figures.LastOrDefault();
                        var quadraticBezier = figure.Segments.LastOrDefault() as QuadraticBezierSegmentViewModel;
                        quadraticBezier.Point1 = point1;
                        _quadraticBezier.Point2 = point1;
                    }
                }

                _quadraticBezier.Point1 = _quadraticBezier.Point3;
                _quadraticBezier.Point2 = factory.CreatePointShape((double)sx, (double)sy);
                _quadraticBezier.Point3 = factory.CreatePointShape((double)sx, (double)sy);
                pathTool.GeometryContext.QuadraticBezierTo(
                    _quadraticBezier.Point2,
                    _quadraticBezier.Point3);
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                ToStatePoint3();
                Move(null);
                _currentState = State.Point3;
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
                Finalize(null);
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
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                _quadraticBezier.Point2.X = (double)sx;
                _quadraticBezier.Point2.Y = (double)sy;
                _quadraticBezier.Point3.X = (double)sx;
                _quadraticBezier.Point3.Y = (double)sy;
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                Move(null);
            }
                break;
            case State.Point2:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                _quadraticBezier.Point2.X = (double)sx;
                _quadraticBezier.Point2.Y = (double)sy;
                editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                Move(null);
            }
                break;
        }
    }

    public void ToStatePoint3()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        _selection?.Reset();
        _selection = new QuadraticBezierSelection(
            ServiceProvider,
            editor.Project.CurrentContainer.HelperLayer,
            _quadraticBezier,
            editor.PageState.HelperStyle);
        _selection.ToStatePoint3();
    }

    public void ToStatePoint2()
    {
        _selection.ToStatePoint2();
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
        var pathTool = ServiceProvider.GetService<PathToolViewModel>();

        switch (_currentState)
        {
            case State.Point1:
                break;
            case State.Point3:
            case State.Point2:
            {
                pathTool.RemoveLastSegment<QuadraticBezierSegmentViewModel>();
            }
                break;
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
