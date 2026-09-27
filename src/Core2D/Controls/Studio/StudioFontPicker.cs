// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A searchable, editable system-font selector using Avalonia's native autocomplete.</summary>
public class StudioFontPicker : AutoCompleteBox
{
    /// <summary>Loads available font names once per picker without constraining externally supplied names.</summary>
    public StudioFontPicker()
    {
        var names = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase);
        foreach (FontFamily family in FontManager.Current.SystemFonts) names.Add(family.Name);
        ItemsSource = names;
        MinimumPrefixLength = 0;
        MinimumPopulateDelay = TimeSpan.Zero;
        FilterMode = AutoCompleteFilterMode.Contains;
        IsTextCompletionEnabled = false;
    }
}
