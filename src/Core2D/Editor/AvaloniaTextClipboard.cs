// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Core2D.Model;

namespace Core2D.Editor;

public sealed class AvaloniaTextClipboard : ITextClipboard
{
    async Task ITextClipboard.SetText(string? text)
    {
        if (text is null)
        {
            return;
        }

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            if (lifetime.MainWindow?.Clipboard is {} clipboard)
            {
                await clipboard.SetTextAsync(text);
            }
        }
    }

    private async Task<string?> GetTextAsync()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            if (lifetime.MainWindow?.Clipboard is {} clipboard)
            {
                return await clipboard.GetTextAsync();
            }
        }
        return default;
    }

    Task<string?> ITextClipboard.GetText()
    {
        return GetTextAsync();
    }

    private async Task<bool> ContainsTextAsync()
    {
        return !string.IsNullOrEmpty(await GetTextAsync());
    }

    async Task<bool> ITextClipboard.ContainsText()
    {
        return await ContainsTextAsync();
    }
}
