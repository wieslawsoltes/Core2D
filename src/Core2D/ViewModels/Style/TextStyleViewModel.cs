// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Style;

namespace Core2D.ViewModels.Style;

public partial class TextStyleViewModel : ViewModelBase
{
    public static TextHAlignment[] TextHAlignmentValues { get; } = (TextHAlignment[])Enum.GetValues(typeof(TextHAlignment));

    public static TextVAlignment[] TextVAlignmentValues { get; } = (TextVAlignment[])Enum.GetValues(typeof(TextVAlignment));

    [AutoNotify] private string? _fontName;
    [AutoNotify] private string? _fontFile;
    [AutoNotify] private double _fontSize;
    [AutoNotify] private FontStyleFlags _fontStyle;
    [AutoNotify] private TextHAlignment? _textHAlignment;
    [AutoNotify] private TextVAlignment? _textVAlignment;

    public TextStyleViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public void ToggleRegularFontStyle()
    {
        FontStyle ^= FontStyleFlags.Regular;
    }

    public void ToggleBoldFontStyle()
    {
        FontStyle ^= FontStyleFlags.Bold;
    }

    public void ToggleItalicFontStyle()
    {
        FontStyle ^= FontStyleFlags.Italic;
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new TextStyleViewModel(ServiceProvider)
        {
            Name = Name,
            FontName = _fontName,
            FontFile = _fontFile,
            FontSize = _fontSize,
            FontStyle = _fontStyle,
            TextHAlignment = _textHAlignment,
            TextVAlignment = _textVAlignment
        };

        return copy;
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();
        return isDirty;
    }
}