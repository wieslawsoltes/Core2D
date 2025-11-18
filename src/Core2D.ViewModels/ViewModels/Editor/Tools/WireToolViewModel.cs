// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

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

public partial class WireToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { Start, End }
    private State _currentState = State.Start;
    private WireShapeViewModel? _wire;
    private WireSelection? _selection;

    [AutoNotify] private string _rendererKey = WireRendererKeys.Bezier;

    public string Title => "Wire";

    public WireToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
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

        var (x, y) = args;
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.Start:
            {
                editor.IsToolIdle = false;
                var style = editor.Project.CurrentStyleLibrary?.Selected is { }
                    ? editor.Project.CurrentStyleLibrary.Selected
                    : viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                selection.ClearConnectionPoints();

                _wire = factory.CreateWireShape(
                    (double)sx, (double)sy,
                    (ShapeStyleViewModel)style.Copy(null),
                    editor.Project.Options.DefaultIsStroked,
                    rendererKey: _rendererKey);

                _wire.RendererKey = _rendererKey;

                editor.SetShapeName(_wire);

                if (editor.Project.Options.TryToConnect)
                {
                    var result = selection.TryToGetConnectionPoint((double)sx, (double)sy, _wire?.Start);
                    if (result is { })
                    {
                        _wire.Start = result;
                        selection.RememberConnectionPoint(result);
                    }
                    else
                    {
                        if (_wire.Start is { })
                        {
                            selection.TryToSplitLine(x, y, _wire.Start);
                        }
                    }
                }

                if (editor.Project.Options.DefaultIsStroked && !_wire.IsStroked)
                {
                    _wire.IsStroked = true;
                }

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_wire);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }

                ToStateEnd();
                Move(_wire);
                _currentState = State.End;
                break;
            }
            case State.End:
            {
                if (_wire is { })
                {
                    if (_wire.End is { })
                    {
                        _wire.End.X = (double)sx;
                        _wire.End.Y = (double)sy;
                    }

                    if (editor.Project.Options.TryToConnect)
                    {
                        var result = selection.TryToGetConnectionPoint((double)sx, (double)sy, _wire?.Start);
                        if (result is { })
                        {
                            _wire.End = result;
                            selection.RememberConnectionPoint(result);
                        }
                        else
                        {
                            if (_wire.End is { })
                            {
                                selection.TryToSplitLine(x, y, _wire.End);
                            }
                        }
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_wire);
                    }

                    Finalize(_wire);

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _wire);
                    }

                    Reset();
                }

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
                if (_wire is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy, true, _wire?.Start);
                    }

                    if (_wire.End is { })
                    {
                        _wire.End.X = (double)sx;
                        _wire.End.Y = (double)sy;
                    }

                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_wire);
                }
                break;
            }
        }
    }

    public void ToStateEnd()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is { }
            && editor.Project?.CurrentContainer?.HelperLayer is { }
            && _wire is { }
            && editor.PageState?.HelperStyle is { })
        {
            _selection = new WireSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _wire,
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
        if (editor?.Project is null)
        {
            return;
        }

        switch (_currentState)
        {
            case State.Start:
                break;
            case State.End:
            {
                if (editor.Project.CurrentContainer?.WorkingLayer is { } && _wire is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_wire);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }
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
