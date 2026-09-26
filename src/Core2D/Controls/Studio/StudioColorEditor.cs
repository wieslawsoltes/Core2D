// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A native solid-color workbench with an HSV plane and lossless RGB/HSL/HSB presentation switching.</summary>
public class StudioColorEditor : TemplatedControl
{
    private static readonly ReadOnlyCollection<string> Models = Array.AsReadOnly(new[] { "RGB", "HSL", "HSB" });
    /// <summary>Defines the canonical, byte-accurate color value.</summary>
    public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<StudioColorEditor, Color>(nameof(Color), Colors.Black, defaultBindingMode: BindingMode.TwoWay);
    /// <summary>Defines the floating-point HSV editing state; achromatic colors retain the last hue.</summary>
    public static readonly DirectProperty<StudioColorEditor, HsvColor> HsvProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, HsvColor>(nameof(Hsv), x => x.Hsv, (x, value) => x.Hsv = value);
    /// <summary>Defines the selected color representation: RGB, HSL or HSB.</summary>
    public static readonly DirectProperty<StudioColorEditor, int> ModelIndexProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, int>(nameof(ModelIndex), x => x.ModelIndex, (x, value) => x.ModelIndex = value);
    /// <summary>Defines the first component of the current representation.</summary>
    public static readonly DirectProperty<StudioColorEditor, decimal> FirstProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, decimal>(nameof(First), x => x.First, (x, value) => x.First = value);
    /// <summary>Defines the second component.</summary>
    public static readonly DirectProperty<StudioColorEditor, decimal> SecondProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, decimal>(nameof(Second), x => x.Second, (x, value) => x.Second = value);
    /// <summary>Defines the third component.</summary>
    public static readonly DirectProperty<StudioColorEditor, decimal> ThirdProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, decimal>(nameof(Third), x => x.Third, (x, value) => x.Third = value);
    /// <summary>Defines the hue slider value.</summary>
    public static readonly DirectProperty<StudioColorEditor, double> HueProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, double>(nameof(Hue), x => x.Hue, (x, value) => x.Hue = value);
    /// <summary>Defines opacity in percent.</summary>
    public static readonly DirectProperty<StudioColorEditor, decimal> AlphaProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, decimal>(nameof(Alpha), x => x.Alpha, (x, value) => x.Alpha = value);
    /// <summary>Defines the RGB hexadecimal presentation.</summary>
    public static readonly DirectProperty<StudioColorEditor, string> HexProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, string>(nameof(Hex), x => x.Hex, (x, value) => x.Hex = value);
    /// <summary>Defines the alpha-ramp presentation brush.</summary>
    public static readonly DirectProperty<StudioColorEditor, IBrush?> AlphaBrushProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, IBrush?>(nameof(AlphaBrush), x => x.AlphaBrush);
    /// <summary>Defines the first component label.</summary>
    public static readonly DirectProperty<StudioColorEditor, string> FirstLabelProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, string>(nameof(FirstLabel), x => x.FirstLabel);
    /// <summary>Defines the second component label.</summary>
    public static readonly DirectProperty<StudioColorEditor, string> SecondLabelProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, string>(nameof(SecondLabel), x => x.SecondLabel);
    /// <summary>Defines the third component label.</summary>
    public static readonly DirectProperty<StudioColorEditor, string> ThirdLabelProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, string>(nameof(ThirdLabel), x => x.ThirdLabel);
    /// <summary>Defines the first component range.</summary>
    public static readonly DirectProperty<StudioColorEditor, decimal> FirstMaximumProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, decimal>(nameof(FirstMaximum), x => x.FirstMaximum);
    /// <summary>Defines the remaining component ranges.</summary>
    public static readonly DirectProperty<StudioColorEditor, decimal> OtherMaximumProperty = AvaloniaProperty.RegisterDirect<StudioColorEditor, decimal>(nameof(OtherMaximum), x => x.OtherMaximum);

    private HsvColor _hsv = new(1, 0, 0, 0);
    private bool _updating;
    private int _modelIndex;
    private decimal _first, _second, _third, _alpha = 100, _firstMaximum = 255, _otherMaximum = 255;
    private double _hue;
    private string _hex = "000000", _firstLabel = "R", _secondLabel = "G", _thirdLabel = "B";
    private IBrush? _alphaBrush;

    /// <summary>Initializes presentation without modifying the default color.</summary>
    public StudioColorEditor() => RefreshPresentation();
    /// <summary>Gets the immutable model names.</summary>
    public ReadOnlyCollection<string> ColorModels => Models;
    /// <summary>Gets or sets the canonical color.</summary>
    public Color Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
    /// <summary>Gets or sets the floating-point HSV editing state.</summary>
    public HsvColor Hsv
    {
        get => _hsv;
        set
        {
            if (!double.IsFinite(value.H) || !double.IsFinite(value.S) || !double.IsFinite(value.V) || !double.IsFinite(value.A)) return;
            var bounded = new HsvColor(Math.Clamp(value.A, 0, 1), Math.Clamp(value.H, 0, 360), Math.Clamp(value.S, 0, 1), Math.Clamp(value.V, 0, 1));
            if (!SetAndRaise(HsvProperty, ref _hsv, bounded)) return;
            _updating = true;
            try { SetCurrentValue(ColorProperty, bounded.ToRgb()); }
            finally { _updating = false; RefreshPresentation(); }
        }
    }
    /// <summary>Gets or sets only the displayed model; switching never round-trips the color.</summary>
    public int ModelIndex
    {
        get => _modelIndex;
        set { if (value is >= 0 and <= 2 && SetAndRaise(ModelIndexProperty, ref _modelIndex, value)) RefreshPresentation(); }
    }
    /// <summary>Gets or sets red or hue according to the model.</summary>
    public decimal First { get => _first; set => ChangeChannel(0, value); }
    /// <summary>Gets or sets green or saturation according to the model.</summary>
    public decimal Second { get => _second; set => ChangeChannel(1, value); }
    /// <summary>Gets or sets blue, lightness or brightness according to the model.</summary>
    public decimal Third { get => _third; set => ChangeChannel(2, value); }
    /// <summary>Gets or sets hue without losing an achromatic color's intended hue.</summary>
    public double Hue { get => _hue; set { if (double.IsFinite(value)) Hsv = new HsvColor(_hsv.A, Math.Clamp(value, 0, 360), _hsv.S, _hsv.V); } }
    /// <summary>Gets or sets opacity while keeping the RGB bytes unchanged.</summary>
    public decimal Alpha
    {
        get => _alpha;
        set
        {
            if (value < 0 || value > 100) return;
            byte a = (byte)decimal.Round(value * 255 / 100, 0, MidpointRounding.AwayFromZero);
            SetCurrentValue(ColorProperty, Color.FromArgb(a, Color.R, Color.G, Color.B));
        }
    }
    /// <summary>Gets or sets RGB hex; six digits preserve alpha, eight explicitly specify ARGB.</summary>
    public string Hex
    {
        get => _hex;
        set
        {
            string text = (value ?? string.Empty).Trim().TrimStart('#');
            if (text.Length is not (6 or 8) || !uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint packed)) return;
            SetCurrentValue(ColorProperty, Color.FromArgb(text.Length == 8 ? (byte)(packed >> 24) : Color.A, (byte)(packed >> 16), (byte)(packed >> 8), (byte)packed));
        }
    }
    /// <summary>Gets the alpha-ramp brush.</summary>
    public IBrush? AlphaBrush => _alphaBrush;
    /// <summary>Gets the first component label.</summary>
    public string FirstLabel => _firstLabel;
    /// <summary>Gets the second component label.</summary>
    public string SecondLabel => _secondLabel;
    /// <summary>Gets the third component label.</summary>
    public string ThirdLabel => _thirdLabel;
    /// <summary>Gets the first component maximum.</summary>
    public decimal FirstMaximum => _firstMaximum;
    /// <summary>Gets the other components' maximum.</summary>
    public decimal OtherMaximum => _otherMaximum;

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != ColorProperty || _updating) return;
        HsvColor next = Color.ToHsv();
        if (next.S == 0) next = new HsvColor(next.A, _hsv.H, next.S, next.V);
        SetAndRaise(HsvProperty, ref _hsv, next);
        RefreshPresentation();
    }

    private void ChangeChannel(int index, decimal value)
    {
        if (value < 0 || value > (index == 0 ? FirstMaximum : OtherMaximum)) return;
        if (ModelIndex == 0)
        {
            byte channel = (byte)decimal.Round(value, 0, MidpointRounding.AwayFromZero);
            SetCurrentValue(ColorProperty, Color.FromArgb(Color.A, index == 0 ? channel : Color.R, index == 1 ? channel : Color.G, index == 2 ? channel : Color.B));
        }
        else if (ModelIndex == 2)
        {
            Hsv = new HsvColor(_hsv.A, index == 0 ? (double)value : _hsv.H,
                index == 1 ? (double)value / 100 : _hsv.S, index == 2 ? (double)value / 100 : _hsv.V);
        }
        else
        {
            HslColor hsl = Color.ToHsl();
            double hue = index == 0 ? (double)value : _hsv.H;
            Color next = new HslColor(Color.A / 255d, hue, index == 1 ? (double)value / 100 : hsl.S,
                index == 2 ? (double)value / 100 : hsl.L).ToRgb();
            HsvColor hsv = next.ToHsv();
            Hsv = new HsvColor(hsv.A, hue, hsv.S, hsv.V);
        }
        RefreshPresentation();
    }

    private void RefreshPresentation()
    {
        HslColor hsl = Color.ToHsl();
        SetAndRaise(FirstLabelProperty, ref _firstLabel, ModelIndex == 0 ? "R" : "H");
        SetAndRaise(SecondLabelProperty, ref _secondLabel, ModelIndex == 0 ? "G" : "S");
        SetAndRaise(ThirdLabelProperty, ref _thirdLabel, ModelIndex == 0 ? "B" : ModelIndex == 1 ? "L" : "B");
        SetAndRaise(FirstMaximumProperty, ref _firstMaximum, ModelIndex == 0 ? 255m : 360m);
        SetAndRaise(OtherMaximumProperty, ref _otherMaximum, ModelIndex == 0 ? 255m : 100m);
        SetAndRaise(FirstProperty, ref _first, ModelIndex == 0 ? Color.R : decimal.Round((decimal)_hsv.H, 2));
        SetAndRaise(SecondProperty, ref _second, ModelIndex == 0 ? Color.G : decimal.Round((decimal)(ModelIndex == 1 ? hsl.S : _hsv.S) * 100, 2));
        SetAndRaise(ThirdProperty, ref _third, ModelIndex == 0 ? Color.B : decimal.Round((decimal)(ModelIndex == 1 ? hsl.L : _hsv.V) * 100, 2));
        SetAndRaise(HueProperty, ref _hue, _hsv.H);
        SetAndRaise(AlphaProperty, ref _alpha, decimal.Round(Color.A * 100m / 255m, 2));
        SetAndRaise(HexProperty, ref _hex, $"{Color.R:X2}{Color.G:X2}{Color.B:X2}");
        SetAndRaise(AlphaBrushProperty, ref _alphaBrush, new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
            GradientStops = new GradientStops { new(Color.FromArgb(0, Color.R, Color.G, Color.B), 0), new(Color.FromArgb(255, Color.R, Color.G, Color.B), 1) }
        });
    }
}
