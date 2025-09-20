// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

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

public partial class LineToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { Start, End }
    private State _currentState = State.Start;
    private LineShapeViewModel? _line;
    private LineSelection? _selection;

    public string Title => "Line";

    public LineToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
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

                _line = factory.CreateLineShape(
                    (double) sx, (double) sy,
                    (ShapeStyleViewModel) style.Copy(null),
                    editor.Project.Options.DefaultIsStroked);

                editor.SetShapeName(_line);

                if (editor.Project.Options.TryToConnect)
                {
                    var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (result is { })
                    {
                        _line.Start = result;
                        selection.RememberConnectionPoint(result);
                    }
                    else
                    {
                        if (_line.Start is { })
                        {
                            selection.TryToSplitLine(x, y, _line.Start);
                        }
                    }
                }

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_line);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }

                ToStateEnd();
                Move(_line);
                _currentState = State.End;
                break;
            }
            case State.End:
            {
                if (_line is { })
                {
                    if (_line.End is { })
                    {
                        _line.End.X = (double) sx;
                        _line.End.Y = (double) sy;
                    }

                    if (editor.Project.Options.TryToConnect)
                    {
                        var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                        if (result is { })
                        {
                            _line.End = result;
                            selection.RememberConnectionPoint(result);
                        }
                        else
                        {
                            if (_line.End is { })
                            {
                                selection.TryToSplitLine(x, y, _line.End);
                            }
                        }
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
                    }

                    Finalize(_line);

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _line);
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
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                break;
            }
            case State.End:
            {
                if (_line is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }

                    if (_line.End is { })
                    {
                        _line.End.X = (double)sx;
                        _line.End.Y = (double)sy;
                    }

                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_line);
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
            && _line is { } 
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
                if (editor.Project.CurrentContainer?.WorkingLayer is { } && _line is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_line);
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
