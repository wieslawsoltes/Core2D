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

public partial class RectangleToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { TopLeft, BottomRight }
    private State _currentState = State.TopLeft;
    private RectangleShapeViewModel? _rectangle;
    private RectangleSelection? _selection;

    public string Title => "Rectangle";

    public RectangleToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
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

                _rectangle = factory.CreateRectangleShape(
                    (double) sx, (double) sy,
                    (ShapeStyleViewModel) style.Copy(null),
                    editor.Project.Options.DefaultIsStroked,
                    editor.Project.Options.DefaultIsFilled);

                editor.SetShapeName(_rectangle);

                var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                if (result is { })
                {
                    _rectangle.TopLeft = result;
                }

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes =
                        editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangle);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }

                ToStateBottomRight();
                Move(_rectangle);
                _currentState = State.BottomRight;
                break;
            }
            case State.BottomRight:
            {
                if (_rectangle is { })
                {
                    if (_rectangle.BottomRight is { })
                    {
                        _rectangle.BottomRight.X = (double)sx;
                        _rectangle.BottomRight.Y = (double)sy;
                    }

                    var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (result is { })
                    {
                        _rectangle.BottomRight = result;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes =
                            editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                    }

                    Finalize(_rectangle);

                    if (editor.Project.CurrentContainer?.CurrentLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _rectangle);
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
            if (_currentState != State.TopLeft)
            {
                NextPoint(args);
            }
        }
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
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy, true);
                }
                break;
            }
            case State.BottomRight:
            {
                if (_rectangle is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy, true);
                    }

                    if (_rectangle.BottomRight is { })
                    {
                        _rectangle.BottomRight.X = (double)sx;
                        _rectangle.BottomRight.Y = (double)sy;
                    }
                    
                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    }
                    
                    Move(_rectangle);
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
            && _rectangle is { })
        {
            _selection = new RectangleSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _rectangle,
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
                if (editor.Project.CurrentContainer?.WorkingLayer is { } && _rectangle is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
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
