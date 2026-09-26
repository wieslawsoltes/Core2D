// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Globalization;
using System.Collections.Generic;
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

    /// <summary>Controls optional edge labels for the selected interval.</summary>
    public static readonly StyledProperty<bool> ShowSelectionLabelsProperty =
        AvaloniaProperty.Register<Ruler, bool>(nameof(ShowSelectionLabels), true);

    /// <summary>Gets or sets whether selected bounds display numeric edge labels.</summary>
    public bool ShowSelectionLabels { get => GetValue(ShowSelectionLabelsProperty); set => SetValue(ShowSelectionLabelsProperty, value); }

    private readonly Dictionary<string, FormattedText> _labels = new();

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
            SelectionOpacityProperty, ShowSelectionLabelsProperty);
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
        if (change.Property != MarkerProperty && change.Property != SelectionStartProperty
            && change.Property != SelectionLengthProperty && change.Property != OffsetProperty
            && change.Property != HighlightStartProperty && change.Property != HighlightLengthProperty)
            _labels.Clear();

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

        double zoom = Zoom, offset = Offset;
        double axisLength = Orientation == Orientation.Horizontal ? Bounds.Width : Bounds.Height;
        double thickness = Orientation == Orientation.Horizontal ? Bounds.Height : Bounds.Width;
        RulerScale scale = RulerScale.Create(zoom, offset, axisLength, DesiredMajorTickSpacing, MinorTickCount);
        if (scale.Count == 0) return;
        var tickPen = new Pen(TickBrush ?? Foreground ?? Brushes.Gray, 1);
        var accentPen = new Pen(AccentBrush ?? tickPen.Brush, 1);
        IBrush textBrush = TextBrush ?? tickPen.Brush ?? Brushes.Gray;
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        CultureInfo culture = CultureInfo.CurrentUICulture;
        using (context.PushClip(new Rect(Bounds.Size)))
        {
            DrawBorderLine(context, tickPen, axisLength, thickness);
            DrawHighlight(context, axisLength, thickness, zoom, offset);
            DrawSelectionHighlight(context, axisLength, thickness, zoom, offset, typeface, culture);
            double previousLabelEnd = double.NegativeInfinity;
            // Integer-bounded iteration cannot stall at very large world coordinates.
            for (int majorIndex = 0; majorIndex < scale.Count; majorIndex++)
            {
                double major = scale.First + majorIndex * scale.Step;
                double position = major * zoom + offset;
                if (!double.IsFinite(position)) continue;
                if (position >= 0 && position <= axisLength)
                {
                    DrawTick(context, tickPen, position, thickness, Math.Min(MajorTickLength, thickness));
                    string text;
                    try { text = LabelFormat == "Auto" ? scale.Format(major, culture) : major.ToString(LabelFormat, culture); }
                    catch (FormatException) { text = scale.Format(major, culture); }
                    if (!_labels.TryGetValue(text, out FormattedText? formatted))
                    {
                        if (_labels.Count >= 512) _labels.Clear();
                        formatted = new FormattedText(text, culture, FlowDirection.LeftToRight, typeface, FontSize, textBrush);
                        _labels[text] = formatted;
                    }
                    double labelStart = position + 4;
                    if (labelStart >= previousLabelEnd && labelStart + formatted.Width <= axisLength - 2)
                    {
                        if (Orientation == Orientation.Horizontal)
                            context.DrawText(formatted, new Point(labelStart, 1));
                        else
                        {
                            // Rotate long vertical labels rather than clipping them to ruler thickness.
                            using (context.PushTransform(Matrix.CreateRotation(-Math.PI / 2)
                                * Matrix.CreateTranslation(1, labelStart + formatted.Width)))
                                context.DrawText(formatted, default);
                        }
                        previousLabelEnd = labelStart + formatted.Width + 8;
                    }
                }
                for (int i = 1; i < scale.Subdivisions; i++)
                {
                    double minor = position + i * scale.Step * zoom / scale.Subdivisions;
                    if (minor >= 0 && minor <= axisLength)
                        DrawTick(context, tickPen, minor, thickness, Math.Min(MinorTickLength, thickness));
                }
            }
            DrawZeroLine(context, accentPen, axisLength, thickness, zoom, offset);
            DrawMarker(context, new Pen(SelectionBrush ?? accentPen.Brush, 1), axisLength, thickness, zoom, offset);
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

        if (!ShowSelectionLabels) return;

        var startText = SelectionStartText;
        var endText = SelectionEndText;
        var format = string.IsNullOrWhiteSpace(SelectionLabelFormat) ? LabelFormat : SelectionLabelFormat;

        if (string.IsNullOrWhiteSpace(startText))
        {
            startText = format == "Auto" ? RulerScale.Create(zoom, offset, axisLength).Format(SelectionStart, culture) : SelectionStart.ToString(format, culture);
        }

        var endValue = SelectionStart + SelectionLength;
        if (string.IsNullOrWhiteSpace(endText))
        {
            endText = format == "Auto" ? RulerScale.Create(zoom, offset, axisLength).Format(endValue, culture) : endValue.ToString(format, culture);
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
        var aligned = Math.Floor(position) + 0.5;
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
