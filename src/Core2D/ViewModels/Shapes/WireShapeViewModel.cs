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

public static class WireRendererKeys
{
    public const string Line = "Line";
    public const string Bezier = "Bezier";
}

public partial class WireShapeViewModel : LineShapeViewModel
{
    public const string BezierControl1Property = "Wire.BezierControl1";
    public const string BezierControl2Property = "Wire.BezierControl2";

    [AutoNotify] private string _rendererKey = WireRendererKeys.Bezier;

    public WireShapeViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, typeof(WireShapeViewModel))
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new WireShapeViewModel(ServiceProvider)
        {
            Name = Name,
            State = State,
            Style = _style?.CopyShared(shared),
            IsStroked = IsStroked,
            IsFilled = IsFilled,
            Properties = _properties.CopyShared(shared).ToImmutable(),
            Record = _record,
            RendererKey = _rendererKey,
            Start = Start?.CopyShared(shared),
            End = End?.CopyShared(shared)
        };

        return copy;
    }

    public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (!State.HasFlag(ShapeStateFlags.Visible))
        {
            return;
        }

        renderer?.DrawWire(dc, this, Style);
    }
}
