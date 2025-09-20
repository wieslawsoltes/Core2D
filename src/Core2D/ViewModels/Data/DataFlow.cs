// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Immutable;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data.Bindings;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Data;

public class DataFlow
{
    public void Bind(ProjectContainerViewModel? project)
    {
        if (project is null)
        {
            return;
        }

        foreach (var document in project.Documents)
        {
            Bind(document);
        }
    }

    public void Bind(DocumentContainerViewModel? document)
    {
        if (document is null)
        {
            return;
        }

        foreach (var container in document.Pages)
        {
            var db = container.Properties;
            var r = container.Record;

            Bind(container.Template, db, r);
            Bind(container, db, r);
        }
    }

    public void Bind(FrameContainerViewModel? container, object? db, object? r)
    {
        if (container is null)
        {
            return;
        }

        foreach (var layer in container.Layers)
        {
            Bind(layer, db, r);
        }
    }

    public void Bind(LayerContainerViewModel? layer, object? db, object? r)
    {
        if (layer is null)
        {
            return;
        }

        foreach (var shape in layer.Shapes)
        {
            shape.Bind(this, db, r);
        }
    }

    public void Bind(LineShapeViewModel? line, object? db, object? r)
    {
        if (line is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind LineShapeViewModel
    }

    public void Bind(RectangleShapeViewModel? rectangle, object? db, object? r)
    {
        if (rectangle is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind RectangleShapeViewModel
    }

    public void Bind(EllipseShapeViewModel? ellipse, object? db, object? r)
    {
        if (ellipse is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind EllipseShapeViewModel
    }

    public void Bind(ArcShapeViewModel? arc, object? db, object? r)
    {
        if (arc is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind ArcShapeViewModel
    }

    public void Bind(CubicBezierShapeViewModel? cubicBezier, object? db, object? r)
    {
        if (cubicBezier is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind CubicBezierShapeViewModel
    }

    public void Bind(QuadraticBezierShapeViewModel? quadraticBezier, object? db, object? r)
    {
        if (quadraticBezier is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind QuadraticBezierShapeViewModel
    }

    public void Bind(TextShapeViewModel? text, object? db, object? r)
    {
        if (text is null)
        {
            return;
        }

        var properties = (ImmutableArray<PropertyViewModel>?)db;
        var record = (RecordViewModel?)r;
        var value = TextBinding.Bind(text, properties, record);
        text.SetProperty(nameof(TextShapeViewModel.Text), value);
    }

    public void Bind(ImageShapeViewModel? image, object? db, object? r)
    {
        if (image is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind ImageShapeViewModel
    }

    public void Bind(PathShapeViewModel? path, object? db, object? r)
    {
        if (path is null)
        {
            // ReSharper disable once RedundantJumpStatement
            return;
        }

        // TODO: Bind PathShapeViewModel
    }
}