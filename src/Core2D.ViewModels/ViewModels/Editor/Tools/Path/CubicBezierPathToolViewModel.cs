// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

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

public partial class CubicBezierPathToolViewModel : ViewModelBase, IPathTool
{
    public enum State { Point1, Point4, Point2, Point3 }
    private State _currentState;
    private readonly CubicBezierShapeViewModel _cubicBezier;
    private BezierSelectionSelection? _selectionSelection;

    public string Title => "CubicBezier";

    public CubicBezierPathToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _currentState = State.Point1;
        _cubicBezier = new CubicBezierShapeViewModel(serviceProvider);
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

                selection.ClearConnectionPoints();

                _cubicBezier.Point1 = selection.TryToGetConnectionPoint((double) sx, (double) sy) ??
                                      factory.CreatePointShape((double) sx, (double) sy);
                if (!pathTool.IsInitialized)
                {
                    pathTool.InitializeWorkingPath(_cubicBezier.Point1);
                }
                else
                {
                    _cubicBezier.Point1 = pathTool.GetLastPathPoint();
                }

                _cubicBezier.Point2 = factory.CreatePointShape((double) sx, (double) sy);
                _cubicBezier.Point3 = factory.CreatePointShape((double) sx, (double) sy);
                _cubicBezier.Point4 = factory.CreatePointShape((double) sx, (double) sy);
                pathTool.GeometryContext?.CubicBezierTo(
                    _cubicBezier.Point2,
                    _cubicBezier.Point3,
                    _cubicBezier.Point4);
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                ToStatePoint4();
                Move(null);
                _currentState = State.Point4;
                break;
            }
            case State.Point4:
            {
                if (_cubicBezier.Point4 is { })
                {
                    _cubicBezier.Point4.X = (double)sx;
                    _cubicBezier.Point4.Y = (double)sy;
                }
                if (editor.Project.Options.TryToConnect)
                {
                    var point3 = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (point3 is { })
                    {
                        var figure = pathTool.Path?.Figures.LastOrDefault();
                        if (figure is { })
                        {
                            var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegmentViewModel;
                            if (cubicBezier is { })
                            {
                                cubicBezier.Point3 = point3;
                            }
                        }
                        _cubicBezier.Point4 = point3;
                    }
                }

                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                ToStatePoint2();
                Move(null);
                _currentState = State.Point2;
                break;
            }
            case State.Point2:
            {
                if (_cubicBezier.Point2 is { })
                {
                    _cubicBezier.Point2.X = (double)sx;
                    _cubicBezier.Point2.Y = (double)sy;
                }
                if (editor.Project.Options.TryToConnect)
                {
                    var point1 = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (point1 is { })
                    {
                        var figure = pathTool.Path?.Figures.LastOrDefault();
                        if (figure is { })
                        {
                            var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegmentViewModel;
                            if (cubicBezier is { })
                            {
                                cubicBezier.Point1 = point1;
                            }
                        }
                        _cubicBezier.Point2 = point1;
                    }
                }

                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                ToStateThree();
                Move(null);
                _currentState = State.Point3;
                break;
            }
            case State.Point3:
            {
                if (_cubicBezier.Point3 is { })
                {
                    _cubicBezier.Point3.X = (double)sx;
                    _cubicBezier.Point3.Y = (double)sy;
                }
                if (editor.Project.Options.TryToConnect)
                {
                    var point2 = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (point2 is { })
                    {
                        var figure = pathTool.Path?.Figures.LastOrDefault();
                        if (figure is { })
                        {
                            var cubicBezier = figure.Segments.LastOrDefault() as CubicBezierSegmentViewModel;
                            if (cubicBezier is { })
                            {
                                cubicBezier.Point2 = point2;
                            }
                        }
                        _cubicBezier.Point3 = point2;
                    }
                }

                _cubicBezier.Point1 = _cubicBezier.Point4;
                _cubicBezier.Point2 = factory.CreatePointShape((double) sx, (double) sy);
                _cubicBezier.Point3 = factory.CreatePointShape((double) sx, (double) sy);
                _cubicBezier.Point4 = factory.CreatePointShape((double) sx, (double) sy);
                pathTool.GeometryContext?.CubicBezierTo(
                    _cubicBezier.Point2,
                    _cubicBezier.Point3,
                    _cubicBezier.Point4);
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                ToStatePoint4();
                Move(null);
                _currentState = State.Point4;
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
                    selection.TryToHoverShape((double)sx, (double)sy, true);
                }
                break;
            }
            case State.Point4:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy, true);
                }
                if (_cubicBezier.Point2 is { } && _cubicBezier.Point3 is { } && _cubicBezier.Point4 is { })
                {
                    _cubicBezier.Point2.X = (double)sx;
                    _cubicBezier.Point2.Y = (double)sy;
                    _cubicBezier.Point3.X = (double)sx;
                    _cubicBezier.Point3.Y = (double)sy;
                    _cubicBezier.Point4.X = (double)sx;
                    _cubicBezier.Point4.Y = (double)sy;
                }
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                Move(null);
                break;
            }
            case State.Point2:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy, true);
                }
                if (_cubicBezier.Point2 is { })
                {
                    _cubicBezier.Point2.X = (double)sx;
                    _cubicBezier.Point2.Y = (double)sy;
                }
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                Move(null);
                break;
            }
            case State.Point3:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy, true);
                }
                if (_cubicBezier.Point3 is { })
                {
                    _cubicBezier.Point3.X = (double)sx;
                    _cubicBezier.Point3.Y = (double)sy;
                }
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                Move(null);
                break;
            }
        }
    }

    public void ToStatePoint4()
    {
        _selectionSelection?.Reset();

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is { }
            && editor.Project?.CurrentContainer?.HelperLayer is { }
            && editor.PageState?.HelperStyle is { })
        {
            _selectionSelection = new BezierSelectionSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                editor.PageState.HelperStyle);
            _selectionSelection?.ToStatePoint4();
        }
    }

    public void ToStatePoint2()
    {
        _selectionSelection?.ToStatePoint2();
    }

    public void ToStateThree()
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
        var pathTool = ServiceProvider.GetService<PathToolViewModel>();
        if (editor is null)
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
                pathTool?.RemoveLastSegment<CubicBezierSegmentViewModel>();
                break;
            }
        }

        _currentState = State.Point1;

        if (_selectionSelection is { })
        {
            _selectionSelection.Reset();
            _selectionSelection = null;
        }

        var selection = ServiceProvider.GetService<ISelectionService>();
        selection?.ClearConnectionPoints();

        editor.IsToolIdle = true;
    }
}
