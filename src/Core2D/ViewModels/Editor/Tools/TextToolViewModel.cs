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

public partial class TextToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { TopLeft, BottomRight }
    private State _currentState = State.TopLeft;
    private TextShapeViewModel? _text;
    private TextSelection? _selection;

    public string Title => "Text";

    public TextToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
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
            case State.TopLeft:
            {
                editor.IsToolIdle = false;
                var style = editor.Project.CurrentStyleLibrary?.Selected is { }
                    ? editor.Project.CurrentStyleLibrary.Selected
                    : viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                selection.ClearConnectionPoints();

                _text = factory.CreateTextShape(
                    (double) sx, (double) sy,
                    (ShapeStyleViewModel) style.Copy(null),
                    "Text",
                    editor.Project.Options.DefaultIsStroked);

                editor.SetShapeName(_text);

                var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                if (result is { })
                {
                    _text.TopLeft = result;
                }

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes =
                        editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_text);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }

                ToStateBottomRight();
                Move(_text);
                _currentState = State.BottomRight;
                break;
            }
            case State.BottomRight:
            {
                if (_text is { })
                {
                    if (_text.BottomRight is { })
                    {
                        _text.BottomRight.X = (double)sx;
                        _text.BottomRight.Y = (double)sy;
                    }

                    var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (result is { })
                    {
                        _text.BottomRight = result;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes =
                            editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
                    }

                    Finalize(_text);

                    if (editor.Project.CurrentContainer?.CurrentLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _text);
                    }

                    Reset();
                }

                break;
            }
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
            if (_currentState != State.TopLeft)
            {
                NextPoint(args);
            }
        }
    }

    public void BeginDown(InputArgs args)
    {
        NextPoint(args);
    }

    public void EndDown(InputArgs args)
    {
        switch (_currentState)
        {
            case State.TopLeft:
                break;
            case State.BottomRight:
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
            case State.TopLeft:
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                break;
            case State.BottomRight:
            {
                if (_text is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    if (_text.BottomRight is { })
                    {
                        _text.BottomRight.X = (double)sx;
                        _text.BottomRight.Y = (double)sy;
                    }
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_text);
                }
                break;
            }
        }
    }

    public void ToStateBottomRight()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();

        if (editor is { }
            && editor.Project?.CurrentContainer?.HelperLayer is { }
            && editor.PageState?.HelperStyle is { }
            && _text is { })
        {
            _selection = new TextSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _text,
                editor.PageState.HelperStyle);

            _selection.ToStateBottomRight();
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
            case State.TopLeft:
                break;
            case State.BottomRight:
            {
                if (editor.Project.CurrentContainer?.WorkingLayer is { } && _text is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_text);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }
                break;
            }
        }

        _currentState = State.TopLeft;

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
