// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes;

public abstract partial class ConnectableShapeViewModel : BaseShapeViewModel
{
    // ReSharper disable once InconsistentNaming
    [AutoNotify] protected ImmutableArray<PointShapeViewModel> _connectors;

    protected ConnectableShapeViewModel(IServiceProvider? serviceProvider, Type targetType) : base(serviceProvider, targetType)
    {
    }

    public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
    }

    public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (selection?.SelectedShapes is null)
        {
            return;
        }

        if (_connectors.IsDefault)
        {
            return;
        }

        if (selection.SelectedShapes.Contains(this))
        {
            foreach (var connector in _connectors)
            {
                connector.DrawShape(dc, renderer, selection);
            }
        }
        else
        {
            foreach (var connector in _connectors)
            {
                if (selection.SelectedShapes.Contains(connector))
                {
                    connector.DrawShape(dc, renderer, selection);
                }
            }
        }
    }

    public override void Bind(DataFlow dataFlow, object? db, object? r)
    {
        var record = Record ?? r;

        if (_connectors.IsDefault) return;
        foreach (var connector in _connectors)
        {
            connector.Bind(dataFlow, db, record);
        }
    }

    public override void Move(ISelection? selection, decimal dx, decimal dy)
    {
        if (_connectors.IsDefault) return;
        foreach (var connector in _connectors)
        {
            connector.Move(selection, dx, dy);
        }
    }

    public override void Select(ISelection? selection)
    {
        base.Select(selection);

        if (_connectors.IsDefault) return;
        foreach (var connector in _connectors)
        {
            connector.Select(selection);
        }
    }

    public override void Deselect(ISelection? selection)
    {
        base.Deselect(selection);

        if (_connectors.IsDefault) return;
        foreach (var connector in _connectors)
        {
            connector.Deselect(selection);
        }
    }

    public override void GetPoints(IList<PointShapeViewModel> points)
    {
        if (_connectors.IsDefault) return;
        foreach (var connector in _connectors)
        {
            points.Add(connector);
        }
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_connectors.IsDefault) return isDirty;
        foreach (var connector in _connectors)
        {
            isDirty |= connector.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        if (_connectors.IsDefault) return;
        foreach (var connector in _connectors)
        {
            connector.Invalidate();
        }
    }
}
