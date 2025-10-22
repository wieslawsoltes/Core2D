// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Tools;

public partial class PointToolViewModel : ViewModelBase, IEditorTool
{
    public enum State { Point }
    private State _currentState = State.Point;
    private PointShapeViewModel? _point;

    public string Title => "Point";

    public PointToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
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
        if (factory is null || editor?.Project?.Options is null || selection is null)
        {
            return;
        }

        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.Point:
            {
                _point = factory.CreatePointShape(
                    (double) sx,
                    (double) sy);

                editor.SetShapeName(_point);

                if (editor.Project.Options.TryToConnect)
                {
                    if (!selection.TryToSplitLine(args.X, args.Y, _point, true))
                    {
                        if (editor.Project.CurrentContainer?.CurrentLayer is { })
                        {
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _point);
                        }
                    }
                }
                else
                {
                    if (editor.Project.CurrentContainer?.CurrentLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _point);
                    }
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
    }

    public void EndDown(InputArgs args)
    {
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
            case State.Point:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                break;
            }
        }
    }

    public void Move(BaseShapeViewModel? shape)
    {
    }

    public void Finalize(BaseShapeViewModel? shape)
    {
    }

    public void Reset()
    {
    }
}
