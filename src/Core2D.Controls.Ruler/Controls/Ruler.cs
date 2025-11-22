// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Core2D.Controls;

/// <summary>
/// Owner drawn ruler control that reacts to zoom and pan.
/// </summary>
public class Ruler : TemplatedControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<Ruler, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(Zoom), 1d);

    public static readonly StyledProperty<double> OffsetProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(Offset), 0d);

    public static readonly StyledProperty<double> HighlightStartProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(HighlightStart), 0d);

    public static readonly StyledProperty<double> HighlightLengthProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(HighlightLength), 0d);

    public static readonly StyledProperty<double?> MarkerProperty =
        AvaloniaProperty.Register<Ruler, double?>(nameof(Marker));

    public static readonly StyledProperty<double> DesiredMajorTickSpacingProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(DesiredMajorTickSpacing), 80d);

    public static readonly StyledProperty<int> MinorTickCountProperty =
        AvaloniaProperty.Register<Ruler, int>(nameof(MinorTickCount), 10);

    public static readonly StyledProperty<double> MajorTickLengthProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(MajorTickLength), 12d);

    public static readonly StyledProperty<double> MinorTickLengthProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(MinorTickLength), 6d);

    public static readonly StyledProperty<string> LabelFormatProperty =
        AvaloniaProperty.Register<Ruler, string>(nameof(LabelFormat), "0");

    public static readonly StyledProperty<IBrush?> TickBrushProperty =
        AvaloniaProperty.Register<Ruler, IBrush?>(nameof(TickBrush));

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<Ruler, IBrush?>(nameof(AccentBrush));

    public static readonly StyledProperty<IBrush?> TextBrushProperty =
        AvaloniaProperty.Register<Ruler, IBrush?>(nameof(TextBrush));

    public static readonly StyledProperty<double> HighlightOpacityProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(HighlightOpacity), 0.08d);

    public static readonly StyledProperty<double> SelectionStartProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(SelectionStart), 0d);

    public static readonly StyledProperty<double> SelectionLengthProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(SelectionLength), 0d);

    public static readonly StyledProperty<string?> SelectionStartTextProperty =
        AvaloniaProperty.Register<Ruler, string?>(nameof(SelectionStartText));

    public static readonly StyledProperty<string?> SelectionEndTextProperty =
        AvaloniaProperty.Register<Ruler, string?>(nameof(SelectionEndText));

    public static readonly StyledProperty<string> SelectionLabelFormatProperty =
        AvaloniaProperty.Register<Ruler, string>(nameof(SelectionLabelFormat), "0");

    public static readonly StyledProperty<IBrush?> SelectionBrushProperty =
        AvaloniaProperty.Register<Ruler, IBrush?>(nameof(SelectionBrush));

    public static readonly StyledProperty<IBrush?> SelectionLabelBrushProperty =
        AvaloniaProperty.Register<Ruler, IBrush?>(nameof(SelectionLabelBrush));

    public static readonly StyledProperty<double> SelectionOpacityProperty =
        AvaloniaProperty.Register<Ruler, double>(nameof(SelectionOpacity), 0.18d);

    static Ruler()
    {
        AffectsRender<Ruler>(
            OrientationProperty,
            ZoomProperty,
            OffsetProperty,
            HighlightStartProperty,
            HighlightLengthProperty,
            MarkerProperty,
            DesiredMajorTickSpacingProperty,
            MinorTickCountProperty,
            MajorTickLengthProperty,
            MinorTickLengthProperty,
            LabelFormatProperty,
            TickBrushProperty,
            AccentBrushProperty,
            TextBrushProperty,
            HighlightOpacityProperty,
            BackgroundProperty,
            BorderBrushProperty,
            BorderThicknessProperty,
            FontFamilyProperty,
            FontSizeProperty,
            FontStyleProperty,
            FontWeightProperty,
            FontStretchProperty,
            SelectionStartProperty,
            SelectionLengthProperty,
            SelectionStartTextProperty,
            SelectionEndTextProperty,
            SelectionLabelFormatProperty,
            SelectionBrushProperty,
            SelectionLabelBrushProperty,
            SelectionOpacityProperty);
    }

    public Ruler()
    {
        UpdateOrientationPseudoClasses(Orientation);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public double Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public double HighlightStart
    {
        get => GetValue(HighlightStartProperty);
        set => SetValue(HighlightStartProperty, value);
    }

    public double HighlightLength
    {
        get => GetValue(HighlightLengthProperty);
        set => SetValue(HighlightLengthProperty, value);
    }

    public double? Marker
    {
        get => GetValue(MarkerProperty);
        set => SetValue(MarkerProperty, value);
    }

    public double DesiredMajorTickSpacing
    {
        get => GetValue(DesiredMajorTickSpacingProperty);
        set => SetValue(DesiredMajorTickSpacingProperty, value);
    }

    public int MinorTickCount
    {
        get => GetValue(MinorTickCountProperty);
        set => SetValue(MinorTickCountProperty, value);
    }

    public double MajorTickLength
    {
        get => GetValue(MajorTickLengthProperty);
        set => SetValue(MajorTickLengthProperty, value);
    }

    public double MinorTickLength
    {
        get => GetValue(MinorTickLengthProperty);
        set => SetValue(MinorTickLengthProperty, value);
    }

    public string LabelFormat
    {
        get => GetValue(LabelFormatProperty);
        set => SetValue(LabelFormatProperty, value);
    }

    public IBrush? TickBrush
    {
        get => GetValue(TickBrushProperty);
        set => SetValue(TickBrushProperty, value);
    }

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public IBrush? TextBrush
    {
        get => GetValue(TextBrushProperty);
        set => SetValue(TextBrushProperty, value);
    }

    public double HighlightOpacity
    {
        get => GetValue(HighlightOpacityProperty);
        set => SetValue(HighlightOpacityProperty, value);
    }

    public double SelectionStart
    {
        get => GetValue(SelectionStartProperty);
        set => SetValue(SelectionStartProperty, value);
    }

    public double SelectionLength
    {
        get => GetValue(SelectionLengthProperty);
        set => SetValue(SelectionLengthProperty, value);
    }

    public string? SelectionStartText
    {
        get => GetValue(SelectionStartTextProperty);
        set => SetValue(SelectionStartTextProperty, value);
    }

    public string? SelectionEndText
    {
        get => GetValue(SelectionEndTextProperty);
        set => SetValue(SelectionEndTextProperty, value);
    }

    public string SelectionLabelFormat
    {
        get => GetValue(SelectionLabelFormatProperty);
        set => SetValue(SelectionLabelFormatProperty, value);
    }

    public IBrush? SelectionBrush
    {
        get => GetValue(SelectionBrushProperty);
        set => SetValue(SelectionBrushProperty, value);
    }

    public IBrush? SelectionLabelBrush
    {
        get => GetValue(SelectionLabelBrushProperty);
        set => SetValue(SelectionLabelBrushProperty, value);
    }

    public double SelectionOpacity
    {
        get => GetValue(SelectionOpacityProperty);
        set => SetValue(SelectionOpacityProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OrientationProperty)
        {
            UpdateOrientationPseudoClasses(change.GetNewValue<Orientation>());
        }
    }

    private void UpdateOrientationPseudoClasses(Orientation orientation)
    {
        if (orientation == Orientation.Horizontal)
        {
            PseudoClasses.Add(":horizontal");
            PseudoClasses.Remove(":vertical");
        }
        else
        {
            PseudoClasses.Add(":vertical");
            PseudoClasses.Remove(":horizontal");
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        var background = Background;
        if (background is { })
        {
            context.FillRectangle(background, new Rect(Bounds.Size));
        }

        var zoom = Math.Max(Zoom, double.Epsilon);
        var offset = Offset;
        var desiredSpacing = DesiredMajorTickSpacing > double.Epsilon ? DesiredMajorTickSpacing : 80d;
        var majorStep = CalculateStep(zoom, desiredSpacing);
        if (!double.IsFinite(majorStep) || majorStep <= double.Epsilon)
        {
            return;
        }

        majorStep = NormalizeStepToPixels(majorStep, zoom, desiredSpacing);

        var minorCount = Math.Max(2, Math.Min(10, (int)Math.Round((majorStep * zoom) / 12d)));
        var minorStep = majorStep / minorCount;
        var axisLength = Orientation == Orientation.Horizontal ? Bounds.Width : Bounds.Height;
        var thickness = Orientation == Orientation.Horizontal ? Bounds.Height : Bounds.Width;
        var startWorld = (-offset) / zoom;
        var endWorld = (axisLength - offset) / zoom;
        var firstMajor = Math.Floor(startWorld / majorStep) * majorStep;

        var tickPen = new Pen(TickBrush ?? Foreground ?? Brushes.Gray, 1);
        var accentPen = new Pen(AccentBrush ?? tickPen.Brush, 1);
        var textBrush = TextBrush ?? tickPen.Brush ?? Brushes.Gray;

        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        var culture = CultureInfo.CurrentUICulture;
        var format = string.IsNullOrWhiteSpace(LabelFormat) ? "0" : LabelFormat;

        DrawBorderLine(context, accentPen, axisLength, thickness);
        DrawHighlight(context, axisLength, thickness, zoom, offset);
        DrawSelectionHighlight(context, axisLength, thickness, zoom, offset, typeface, culture);
        DrawMarker(context, accentPen, axisLength, thickness, zoom, offset);
        DrawZeroLine(context, accentPen, axisLength, thickness, zoom, offset);

        for (var major = firstMajor; major <= endWorld; major += majorStep)
        {
            var position = (major * zoom) + offset;
            if (position < -majorStep * zoom || position > axisLength + majorStep * zoom)
            {
                continue;
            }

            DrawTick(context, tickPen, position, thickness, MajorTickLength);
            DrawLabel(context, textBrush, typeface, culture, format, position, thickness, major);

            for (var i = 1; i < MinorTickCount; i++)
            {
                var minorValue = major + (i * minorStep);
                var minorPosition = (minorValue * zoom) + offset;
                if (minorPosition < 0 || minorPosition > axisLength)
                {
                    continue;
                }

                DrawTick(context, tickPen, minorPosition, thickness, MinorTickLength);
            }
        }
    }

    private void DrawSelectionHighlight(
        DrawingContext context,
        double axisLength,
        double thickness,
        double zoom,
        double offset,
        Typeface typeface,
        CultureInfo culture)
    {
        if (SelectionLength <= double.Epsilon || !double.IsFinite(SelectionLength))
        {
            return;
        }

        var start = (SelectionStart * zoom) + offset;
        var size = SelectionLength * zoom;
        if (!double.IsFinite(start) || !double.IsFinite(size) || size <= 0)
        {
            return;
        }

        var selectionBrush = SelectionBrush ?? AccentBrush ?? TickBrush;
        if (selectionBrush is null || SelectionOpacity <= 0)
        {
            return;
        }

        var area = Orientation == Orientation.Horizontal
            ? new Rect(start, 0, size, thickness)
            : new Rect(0, start, thickness, size);

        var bounds = new Rect(new Size(
            Orientation == Orientation.Horizontal ? axisLength : thickness,
            Orientation == Orientation.Horizontal ? thickness : axisLength));

        var selectionRect = area.Intersect(bounds);
        if (selectionRect.Width <= 0 || selectionRect.Height <= 0)
        {
            return;
        }

        if (selectionBrush is ISolidColorBrush solid)
        {
            var alpha = (byte)Math.Clamp(SelectionOpacity * 255d, 0d, 255d);
            var color = solid.Color;
            var brush = new ImmutableSolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
            context.FillRectangle(brush, selectionRect);
        }
        else
        {
            context.FillRectangle(selectionBrush, selectionRect);
        }

        var startText = SelectionStartText;
        var endText = SelectionEndText;
        var format = string.IsNullOrWhiteSpace(SelectionLabelFormat) ? LabelFormat : SelectionLabelFormat;

        if (string.IsNullOrWhiteSpace(startText))
        {
            startText = SelectionStart.ToString(format, culture);
        }

        var endValue = SelectionStart + SelectionLength;
        if (string.IsNullOrWhiteSpace(endText))
        {
            endText = endValue.ToString(format, culture);
        }

        var labelBrush = SelectionLabelBrush ?? selectionBrush ?? Brushes.Gray;
        DrawSelectionLabel(context, labelBrush, typeface, startText, start, thickness, placeBefore: true);
        DrawSelectionLabel(context, labelBrush, typeface, endText, start + size, thickness, placeBefore: false);
    }

    private void DrawSelectionLabel(
        DrawingContext context,
        IBrush brush,
        Typeface typeface,
        string text,
        double position,
        double thickness,
        bool placeBefore)
    {
        var culture = CultureInfo.CurrentUICulture;
        var formatted = new FormattedText(
            text,
            culture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            brush);

        if (Orientation == Orientation.Horizontal)
        {
            var y = Math.Max(2, thickness - formatted.Height - 4);
            var x = placeBefore
                ? Math.Clamp(position - formatted.Width - 6, 0, Math.Max(0, Bounds.Width - formatted.Width - 2))
                : Math.Clamp(position + 6, 0, Math.Max(0, Bounds.Width - formatted.Width - 2));
            context.DrawText(formatted, new Point(x, y));
        }
        else
        {
            var x = Math.Max(2, thickness - formatted.Width - 4);
            var y = placeBefore
                ? Math.Clamp(position - formatted.Height - 6, 0, Math.Max(0, Bounds.Height - formatted.Height - 2))
                : Math.Clamp(position + 6, 0, Math.Max(0, Bounds.Height - formatted.Height - 2));
            context.DrawText(formatted, new Point(x, y));
        }
    }

    private static double CalculateStep(double pixelsPerUnit, double desiredPixels)
    {
        var rawStep = desiredPixels / Math.Max(pixelsPerUnit, double.Epsilon);
        if (!double.IsFinite(rawStep) || rawStep <= double.Epsilon)
        {
            return 0d;
        }

        var exponent = Math.Floor(Math.Log10(rawStep));
        var magnitude = Math.Pow(10, exponent);
        var normalized = rawStep / magnitude;
        var snapped = normalized switch
        {
            < 2d => 1d,
            < 5d => 2d,
            < 10d => 5d,
            _ => 10d
        };

        return snapped * magnitude;
    }

    private static double NormalizeStepToPixels(double step, double zoom, double desiredPixels)
    {
        var min = desiredPixels * 0.6;
        var max = desiredPixels * 1.6;
        var stepPixels = step * zoom;

        while (stepPixels < min)
        {
            step *= 2.0;
            stepPixels *= 2.0;
        }

        while (stepPixels > max)
        {
            step *= 0.5;
            stepPixels *= 0.5;
        }

        return step;
    }

    private void DrawBorderLine(DrawingContext context, Pen pen, double axisLength, double thickness)
    {
        var offset = thickness - 0.5;
        if (Orientation == Orientation.Horizontal)
        {
            context.DrawLine(pen, new Point(0, offset), new Point(axisLength, offset));
        }
        else
        {
            context.DrawLine(pen, new Point(offset, 0), new Point(offset, axisLength));
        }
    }

    private void DrawHighlight(DrawingContext context, double axisLength, double thickness, double zoom, double offset)
    {
        if (HighlightLength <= double.Epsilon || !double.IsFinite(HighlightLength))
        {
            return;
        }

        var start = (HighlightStart * zoom) + offset;
        var size = HighlightLength * zoom;
        if (!double.IsFinite(start) || !double.IsFinite(size) || size <= 0)
        {
            return;
        }

        var highlightBrush = AccentBrush ?? TickBrush;
        if (highlightBrush is null || HighlightOpacity <= 0)
        {
            return;
        }

        var area = Orientation == Orientation.Horizontal
            ? new Rect(start, 0, size, thickness)
            : new Rect(0, start, thickness, size);

        var bounds = new Rect(new Size(
            Orientation == Orientation.Horizontal ? axisLength : thickness,
            Orientation == Orientation.Horizontal ? thickness : axisLength));

        var highlightRect = area.Intersect(bounds);
        if (highlightRect.Width <= 0 || highlightRect.Height <= 0)
        {
            return;
        }

        if (highlightBrush is ISolidColorBrush solid)
        {
            var alpha = (byte)Math.Clamp(HighlightOpacity * 255d, 0d, 255d);
            var color = solid.Color;
            var brush = new ImmutableSolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
            context.FillRectangle(brush, highlightRect);
        }
        else
        {
            context.FillRectangle(highlightBrush, highlightRect);
        }
    }

    private void DrawMarker(DrawingContext context, Pen pen, double axisLength, double thickness, double zoom, double offset)
    {
        if (Marker is not { } markerValue || !double.IsFinite(markerValue))
        {
            return;
        }

        var position = (markerValue * zoom) + offset;
        if (position < 0 || position > axisLength)
        {
            return;
        }

        DrawTick(context, pen, position, thickness, thickness);
    }

    private void DrawZeroLine(DrawingContext context, Pen pen, double axisLength, double thickness, double zoom, double offset)
    {
        var position = offset;
        if (position < 0 || position > axisLength)
        {
            return;
        }

        DrawTick(context, pen, position, thickness, thickness);
    }

    private void DrawTick(DrawingContext context, Pen pen, double position, double thickness, double length)
    {
        var aligned = position + 0.5;
        if (Orientation == Orientation.Horizontal)
        {
            var y = thickness - length;
            context.DrawLine(pen, new Point(aligned, y), new Point(aligned, thickness));
        }
        else
        {
            var x = thickness - length;
            context.DrawLine(pen, new Point(x, aligned), new Point(thickness, aligned));
        }
    }

    private void DrawLabel(
        DrawingContext context,
        IBrush brush,
        Typeface typeface,
        CultureInfo culture,
        string format,
        double position,
        double thickness,
        double value)
    {
        var formatted = new FormattedText(
            value.ToString(format, culture),
            culture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            brush);

        if (Orientation == Orientation.Horizontal)
        {
            var x = Math.Clamp(position + 4, 0, Math.Max(0, Bounds.Width - formatted.Width - 2));
            var y = Math.Max(2, thickness - formatted.Height - 2);
            context.DrawText(formatted, new Point(x, y));
        }
        else
        {
            var x = Math.Max(2, thickness - formatted.Width - 2);
            var y = Math.Clamp(position + 4, 0, Math.Max(0, Bounds.Height - formatted.Height - 2));
            context.DrawText(formatted, new Point(x, y));
        }
    }
}
