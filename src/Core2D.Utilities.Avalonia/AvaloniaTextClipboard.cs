// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System.Threading.Tasks;
using Avalonia;

namespace Core2D.Utilities.Avalonia
{
    /// <summary>
    /// Wrapper class for App.Current.Clipboard clipboard class.
    /// </summary>
    public sealed class AvaloniaTextClipboard : ITextClipboard
    {
        /// <summary>
        /// Set clipboard text.
        /// </summary>
        /// <param name="text">The text string.</param>
        Task ITextClipboard.SetText(string text)
        {
            return Application.Current.Clipboard.SetTextAsync(text);
        }

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        private async Task<string> GetTextAsync()
        {
            return await Application.Current.Clipboard.GetTextAsync();
        }

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        Task<string> ITextClipboard.GetText()
        {
            return GetTextAsync();
        }

        /// <summary>
        /// Return true if clipboard contains text string.
        /// </summary>
        /// <returns>True if clipboard contains text string.</returns>
        private async Task<bool> ContainsTextAsync()
        {
            return !string.IsNullOrEmpty(await GetTextAsync());
        }

        /// <summary>
        /// Return true if clipboard contains text string.
        /// </summary>
        /// <returns>True if clipboard contains text string.</returns>
        async Task<bool> ITextClipboard.ContainsText()
        {
            return await ContainsTextAsync();
        }
    }
}
