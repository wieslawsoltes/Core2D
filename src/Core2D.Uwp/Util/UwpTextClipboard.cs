// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;

namespace Core2D.Uwp
{
    /// <summary>
    /// Wrapper class for <see cref="Windows.ApplicationModel.DataTransfer.Clipboard"/> clipboard class.
    /// </summary>
    public sealed class UwpTextClipboard : ITextClipboard
    {
        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        private async Task<string> GetTextAsync()
        {
            var view = Clipboard.GetContent();
            if (view.Contains(StandardDataFormats.Text))
            {
                return await view.GetTextAsync();
            }
            return null;
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
        /// Set clipboard text.
        /// </summary>
        /// <param name="text">The text string.</param>
        async Task ITextClipboard.SetText(string text)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    var package = new DataPackage();
                    package.RequestedOperation = DataPackageOperation.Copy;
                    package.SetText(text);
                    Clipboard.SetContent(package);
                });
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
        Task<bool> ITextClipboard.ContainsText()
        {
            return ContainsTextAsync();
        }
    }
}
