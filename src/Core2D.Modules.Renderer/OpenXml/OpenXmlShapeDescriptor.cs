// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.Modules.Renderer.OpenXml;

public enum OpenXmlShapeKind
{
    Rectangle,
    Ellipse,
    Line,
    Text
}

public readonly record struct OpenXmlColor(byte A, byte R, byte G, byte B)
{
    public static OpenXmlColor FromArgb(byte a, byte r, byte g, byte b)
        => new(a, r, g, b);
}

public sealed record class OpenXmlShapeDescriptor
{
    public OpenXmlShapeKind Kind { get; init; }
    public string Name { get; init; } = string.Empty;
    public double Left { get; init; }
    public double Top { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    public bool IsFilled { get; init; }
    public OpenXmlColor? Fill { get; init; }
    public bool IsStroked { get; init; }
    public OpenXmlColor? Stroke { get; init; }
    public double StrokeThickness { get; init; }
    public string? Text { get; init; }
    public double LineX1 { get; init; }
    public double LineY1 { get; init; }
    public double LineX2 { get; init; }
    public double LineY2 { get; init; }
}
