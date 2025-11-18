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

public partial class LinePathToolViewModel : ViewModelBase, IPathTool
{
    public enum State { Start, End }
    private State _currentState;
    private readonly LineShapeViewModel _line;
    private LineSelection? _selection;

    public string Title => "Line";

    public LinePathToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _currentState = State.Start;
        _line = new LineShapeViewModel(serviceProvider);
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
            case State.Start:
            {
                editor.IsToolIdle = false;

                selection.ClearConnectionPoints();

                _line.Start = selection.TryToGetConnectionPoint((double) sx, (double) sy, _line.Start) ??
                              factory.CreatePointShape((double) sx, (double) sy);
                if (!pathTool.IsInitialized)
                {
                    pathTool.InitializeWorkingPath(_line.Start);
                }
                else
                {
                    _line.Start = pathTool.GetLastPathPoint();
                }

                _line.End = factory.CreatePointShape((double) sx, (double) sy);
                pathTool.GeometryContext?.LineTo(_line.End);
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                ToStateEnd();
                Move(null);
                _currentState = State.End;
                break;
            }
            case State.End:
            {
                if (_line.End is { })
                {
                    _line.End.X = (double)sx;
                    _line.End.Y = (double)sy;
                }
                
                if (editor.Project.Options.TryToConnect)
                {
                    var end = selection.TryToGetConnectionPoint((double) sx, (double) sy, _line.Start);
                    if (end is { })
                    {
                        var figure = pathTool.Path?.Figures.LastOrDefault();
                        if (figure is { })
                        {
                            var line = figure.Segments.LastOrDefault() as LineSegmentViewModel;
                            if (line is { })
                            {
                                line.Point = end;
                            }
                        }
                    }
                }

                _line.Start = _line.End;
                _line.End = factory.CreatePointShape((double) sx, (double) sy);
                pathTool.GeometryContext?.LineTo(_line.End);
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                Move(null);
                _currentState = State.End;
                break;
            }
        }
    }

    public void BeginDown(InputArgs args)
    {
        NextPoint(args);
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
            if (_currentState != State.Start)
            {
                NextPoint(args);
            }
        }
    }

    public void EndDown(InputArgs args)
    {
        switch (_currentState)
        {
            case State.Start:
                break;
            case State.End:
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
            case State.Start:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy, true);
                }
                break;
            }
            case State.End:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy, true, _line.Start);
                }
                if (_line.End is { })
                {
                    _line.End.X = (double)sx;
                    _line.End.Y = (double)sy;
                }
                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                Move(null);
                break;
            }
        }
    }

    public void ToStateEnd()
    {
        _selection?.Reset();
        
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is { }
            && editor.Project?.CurrentContainer?.HelperLayer is { }
            && editor.PageState?.HelperStyle is { })
        {
            _selection = new LineSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _line,
                editor.PageState.HelperStyle);
            _selection.ToStateEnd();
        }
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
        if (editor is null)
        {
            return;
        }

        switch (_currentState)
        {
            case State.Start:
                break;
            case State.End:
            {
                pathTool?.RemoveLastSegment<LineSegmentViewModel>();
                break;
            }
        }

        _currentState = State.Start;

        if (_selection is { })
        {
            _selection.Reset();
            _selection = null;
        }

        var selection = ServiceProvider.GetService<ISelectionService>();
        selection?.ClearConnectionPoints();

        editor.IsToolIdle = true;
    }
}
