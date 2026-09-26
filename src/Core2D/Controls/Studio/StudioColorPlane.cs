// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A saturation/brightness plane drawn from three brush fills, with no per-pixel bitmap allocation.</summary>
public class StudioColorPlane : Control
{
    /// <summary>Defines the HSV edit state shared with the color workbench.</summary>
    public static readonly DirectProperty<StudioColorPlane, HsvColor> HsvProperty = AvaloniaProperty.RegisterDirect<StudioColorPlane, HsvColor>(nameof(Hsv), x => x.Hsv, (x, value) => x.Hsv = value);
    private HsvColor _hsv = new(1, 0, 0, 0);
    private IBrush _hueBrush = Brushes.Red;
    private readonly Pen _outline = new(Brushes.Black, 3);
    private readonly Pen _ring = new(Brushes.White, 1.5);
    private readonly LinearGradientBrush _white = new()
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
        GradientStops = new GradientStops { new(Colors.White, 0), new(Color.FromArgb(0, 255, 255, 255), 1) }
    };
    private readonly LinearGradientBrush _black = new()
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
        GradientStops = new GradientStops { new(Colors.Transparent, 0), new(Colors.Black, 1) }
    };
    /// <summary>Initializes pointer and keyboard color-plane interactions.</summary>
    public StudioColorPlane()
    {
        Focusable = true;
        Interaction.GetBehaviors(this).Add(new StudioColorPlaneBehavior());
    }
    /// <summary>Gets or sets HSV coordinates, retaining hue and alpha independently of the plane.</summary>
    public HsvColor Hsv
    {
        get => _hsv;
        set
        {
            if (!double.IsFinite(value.H) || !double.IsFinite(value.S) || !double.IsFinite(value.V) || !double.IsFinite(value.A)) return;
            if (SetAndRaise(HsvProperty, ref _hsv, value))
            {
                _hueBrush = new SolidColorBrush(new HsvColor(1, value.H, 1, 1).ToRgb());
                InvalidateVisual();
            }
        }
    }
    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        Rect rect = new(Bounds.Size);
        using (context.PushClip(rect))
        {
            context.DrawRectangle(_hueBrush, null, rect);
            context.DrawRectangle(_white, null, rect);
            context.DrawRectangle(_black, null, rect);
            Point point = new(Math.Clamp(_hsv.S, 0, 1) * rect.Width, (1 - Math.Clamp(_hsv.V, 0, 1)) * rect.Height);
            context.DrawEllipse(null, _outline, point, 5, 5);
            context.DrawEllipse(null, _ring, point, 5, 5);
        }
    }
}
