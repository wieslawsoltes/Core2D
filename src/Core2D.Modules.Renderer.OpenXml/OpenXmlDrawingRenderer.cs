// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Modules.Renderer.OpenXml;

public sealed class OpenXmlDrawingRenderer
{
    public IEnumerable<OpenXmlShapeDescriptor> Render(PageContainerViewModel page)
    {
        if (page.Layers.IsDefault)
        {
            yield break;
        }

        foreach (var layer in page.Layers)
        {
            foreach (var shape in layer.Shapes)
            {
                var descriptor = CreateDescriptor(shape);
                if (descriptor is { })
                {
                    yield return descriptor;
                }
            }
        }
    }

    private OpenXmlShapeDescriptor? CreateDescriptor(BaseShapeViewModel shape)
    {
        return shape switch
        {
            RectangleShapeViewModel rectangle => CreatePresetDescriptor(rectangle, OpenXmlShapeKind.Rectangle),
            EllipseShapeViewModel ellipse => CreatePresetDescriptor(ellipse, OpenXmlShapeKind.Ellipse),
            LineShapeViewModel line => CreateLineDescriptor(line),
            TextShapeViewModel text => CreateTextDescriptor(text),
            _ => null
        };
    }

    private OpenXmlShapeDescriptor? CreatePresetDescriptor(BaseShapeViewModel shape, OpenXmlShapeKind kind)
    {
        if (!TryGetBounds(shape, out var bounds))
        {
            return null;
        }

        return CreateDescriptorCore(shape, kind, bounds);
    }

    private OpenXmlShapeDescriptor? CreateLineDescriptor(LineShapeViewModel line)
    {
        if (line.Start is not { } start || line.End is not { } end)
        {
            return null;
        }

        var left = Math.Min(start.X, end.X);
        var top = Math.Min(start.Y, end.Y);
        var width = Math.Max(Math.Abs(end.X - start.X), 1.0);
        var height = Math.Max(Math.Abs(end.Y - start.Y), 1.0);

        var descriptor = CreateDescriptorCore(line, OpenXmlShapeKind.Line, (left, top, width, height));
        if (descriptor is null)
        {
            return null;
        }

        return descriptor with
        {
            LineX1 = start.X,
            LineY1 = start.Y,
            LineX2 = end.X,
            LineY2 = end.Y
        };
    }

    private OpenXmlShapeDescriptor? CreateTextDescriptor(TextShapeViewModel text)
    {
        if (!TryGetBounds(text, out var bounds))
        {
            return null;
        }

        var descriptor = CreateDescriptorCore(text, OpenXmlShapeKind.Text, bounds);
        return descriptor is null ? null : descriptor with { Text = text.Text };
    }

    private OpenXmlShapeDescriptor? CreateDescriptorCore(BaseShapeViewModel shape, OpenXmlShapeKind kind, (double Left, double Top, double Width, double Height) bounds)
    {
        var descriptor = new OpenXmlShapeDescriptor
        {
            Kind = kind,
            Name = string.IsNullOrWhiteSpace(shape.Name) ? "Shape" : shape.Name,
            Left = bounds.Left,
            Top = bounds.Top,
            Width = Math.Max(bounds.Width, 1.0),
            Height = Math.Max(bounds.Height, 1.0),
            IsFilled = shape.IsFilled,
            IsStroked = shape.IsStroked,
            StrokeThickness = shape.Style?.Stroke?.Thickness ?? 1.0
        };

        if (shape.Style?.Fill?.Color is ArgbColorViewModel fillColor)
        {
            descriptor = descriptor with
            {
                Fill = OpenXmlColor.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B)
            };
        }

        if (shape.Style?.Stroke?.Color is ArgbColorViewModel strokeColor)
        {
            descriptor = descriptor with
            {
                Stroke = OpenXmlColor.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B)
            };
        }

        return descriptor;
    }

    private static bool TryGetBounds(BaseShapeViewModel shape, out (double Left, double Top, double Width, double Height) bounds)
    {
        try
        {
            var box = new ShapeBox(shape);
            var rect = box.Bounds;
            var width = (double)Math.Max(1m, rect.Width);
            var height = (double)Math.Max(1m, rect.Height);
            bounds = ((double)rect.Left, (double)rect.Top, width, height);
            return true;
        }
        catch
        {
            bounds = default;
            return false;
        }
    }
}
