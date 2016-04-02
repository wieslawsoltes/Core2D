// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System.Threading.Tasks;

namespace Core2D.Perspex
{
    /// <summary>
    /// Wrapper class for App.Current.Clipboard clipboard class.
    /// </summary>
    public class PerspexTextClipboard : ITextClipboard
    {
        /// <summary>
        /// Set clipboard text.
        /// </summary>
        /// <param name="text">The text string.</param>
        public Task SetText(string text)
        {
            return App.Current.Clipboard.SetTextAsync(text);
        }

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        private async Task<string> GetTextAsync()
        {
            return await App.Current.Clipboard.GetTextAsync();
        }

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        public Task<string> GetText()
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
        public async Task<bool> ContainsText()
        {
            return await ContainsTextAsync();
        }
    }
}
