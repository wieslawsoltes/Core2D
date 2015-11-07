// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Core2D.UI.Wpf
{
    /// <summary>
    /// Wrapper class for <see cref="System.Windows.Clipboard"/> clipboard class.
    /// </summary>
    internal class TextClipboard : ITextClipboard
    {
        /// <summary>
        /// Set clipboard text.
        /// </summary>
        /// <param name="text">The text string.</param>
        public Task SetText(string text)
        {
            return Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Clipboard.SetText(text, TextDataFormat.UnicodeText);
                });
            });
        }

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        public Task<string> GetText()
        {
            return Task.Run(() =>
            {
                return App.Current.Dispatcher.Invoke(() =>
                {
                    return Clipboard.GetText(TextDataFormat.UnicodeText);
                });
            });
        }

        /// <summary>
        /// Return true if clipboard contains text string.
        /// </summary>
        /// <returns>True if clipboard contains text string.</returns>
        public Task<bool> ContainsText()
        {
            return Task.Run(() =>
            {
                return App.Current.Dispatcher.Invoke(() =>
                {
                    return Clipboard.ContainsText(TextDataFormat.UnicodeText);
                });
            });
        }
    }
}
