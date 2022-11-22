﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using static System.Math;

namespace Core2D.ViewModels.Editor.Tools.Path;

public partial class ArcPathToolViewModel : ViewModelBase, IPathTool
{
    public enum State { Start, End }
    private State _currentState;
    private readonly LineShapeViewModel _arc;
    private LineSelection? _selection;

    public string Title => "Arc";

    public ArcPathToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _currentState = State.Start;
        _arc = new LineShapeViewModel(serviceProvider);
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
                _arc.Start = selection.TryToGetConnectionPoint((double) sx, (double) sy) ??
                             factory.CreatePointShape((double) sx, (double) sy);
                if (!pathTool.IsInitialized)
                {
                    pathTool.InitializeWorkingPath(_arc.Start);
                }
                else
                {
                    _arc.Start = pathTool.GetLastPathPoint();
                }

                _arc.End = factory.CreatePointShape((double) sx, (double) sy);
                if (_arc.Start is { } && _arc.End is { })
                {
                    pathTool.GeometryContext?.ArcTo(
                        _arc.End,
                        factory.CreatePathSize(
                            Abs(_arc.Start.X - _arc.End.X),
                            Abs(_arc.Start.Y - _arc.End.Y)));
                }

                editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                ToStateEnd();
                Move(null);
                _currentState = State.End;
                break;
            }
            case State.End:
            {
                if (_arc.End is { })
                {
                    _arc.End.X = (double)sx;
                    _arc.End.Y = (double)sy;
                }
                if (editor.Project.Options.TryToConnect)
                {
                    var end = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (end is { })
                    {
                        _arc.End = end;
                    }
                }

                _arc.Start = _arc.End;
                _arc.End = factory.CreatePointShape((double) sx, (double) sy);
                if (_arc.Start is { } && _arc.End is { })
                {
                    pathTool.GeometryContext?.ArcTo(
                        _arc.End,
                        factory.CreatePathSize(
                            Abs(_arc.Start.X - _arc.End.X),
                            Abs(_arc.Start.Y - _arc.End.Y)));
                }
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
            {
                break;
            }
            case State.End:
            {
                Reset();
                Finalize(null);
                break;
            }
        }
    }

    public void EndUp(InputArgs args)
    {
    }

    public void Move(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        var pathTool = ServiceProvider.GetService<PathToolViewModel>();
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
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                break;
            }
            case State.End:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                if (_arc.End is { })
                {
                    _arc.End.X = (double)sx;
                    _arc.End.Y = (double)sy;
                }
                var figure = pathTool?.Path?.Figures.LastOrDefault();
                if (figure is { })
                {
                    var arc = figure.Segments.LastOrDefault() as ArcSegmentViewModel;
                    if (arc is { } && arc.Size is { } && _arc.Start is { } && _arc.End is { })
                    {
                        arc.Point = _arc.End;
                        arc.Size.Width = Abs(_arc.Start.X - _arc.End.X);
                        arc.Size.Height = Abs(_arc.Start.Y - _arc.End.Y);
                        editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    }
                }
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
                _arc,
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
                pathTool?.RemoveLastSegment<ArcSegmentViewModel>();
                break;
            }
        }

        _currentState = State.Start;

        if (_selection is { })
        {
            _selection.Reset();
            _selection = null;
        }

        editor.IsToolIdle = true;
    }
}
