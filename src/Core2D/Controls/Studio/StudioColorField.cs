// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A swatch, editable RGB hex and alpha field sharing one color value.</summary>
public class StudioColorField : TemplatedControl
{
    /// <summary>Defines the edited color.</summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<StudioColorField, Color>(nameof(Color), Colors.Black, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Defines the picker popup state.</summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<StudioColorField, bool>(nameof(IsOpen));

    /// <summary>Defines the presentation brush derived from Color.</summary>
    public static readonly DirectProperty<StudioColorField, IBrush> SwatchBrushProperty =
        AvaloniaProperty.RegisterDirect<StudioColorField, IBrush>(nameof(SwatchBrush), x => x.SwatchBrush);

    /// <summary>Defines the validated hex editor value.</summary>
    public static readonly DirectProperty<StudioColorField, string> HexProperty =
        AvaloniaProperty.RegisterDirect<StudioColorField, string>(nameof(Hex), x => x.Hex, (x, v) => x.Hex = v,
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    /// <summary>Defines opacity in percent.</summary>
    public static readonly DirectProperty<StudioColorField, decimal> OpacityPercentProperty =
        AvaloniaProperty.RegisterDirect<StudioColorField, decimal>(nameof(OpacityPercent), x => x.OpacityPercent,
            (x, v) => x.OpacityPercent = v, defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    private IBrush _swatchBrush = Brushes.Black;
    private string _hex = "000000";
    private decimal _opacityPercent = 100m;

    /// <summary>Gets the presentation brush, derived from the edited color.</summary>
    public IBrush SwatchBrush => _swatchBrush;

    /// <summary>Gets or sets the edited color.</summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>Gets or sets whether the picker is open.</summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>Gets or sets RGB hex. Six digits preserve alpha; eight explicitly specify ARGB.</summary>
    public string Hex
    {
        get => _hex;
        set
        {
            var text = (value ?? string.Empty).Trim().TrimStart('#');
            if (text.Length is not (6 or 8) || !uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var packed))
            {
                throw new FormatException("Enter six RGB or eight ARGB hexadecimal digits.");
            }
            var alpha = text.Length == 8 ? (byte)(packed >> 24) : Color.A;
            SetCurrentValue(ColorProperty, Color.FromArgb(alpha, (byte)(packed >> 16), (byte)(packed >> 8), (byte)packed));
        }
    }

    /// <summary>Gets or sets alpha in the inclusive range 0–100 percent.</summary>
    public decimal OpacityPercent
    {
        get => _opacityPercent;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Opacity must be between 0 and 100.");
            }
            var alpha = (byte)decimal.Round(value * 255m / 100m, 0, MidpointRounding.AwayFromZero);
            SetCurrentValue(ColorProperty, Color.FromArgb(alpha, Color.R, Color.G, Color.B));
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ColorProperty)
        {
            SetAndRaise(SwatchBrushProperty, ref _swatchBrush, new SolidColorBrush(Color));
            SetAndRaise(HexProperty, ref _hex, $"{Color.R:X2}{Color.G:X2}{Color.B:X2}");
            SetAndRaise(OpacityPercentProperty, ref _opacityPercent, decimal.Round(Color.A * 100m / 255m, 2));
        }
    }
}
