// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Core2D;

namespace Test.Uwp
{
    /// <summary>
    /// Wrapper class for Windows.ApplicationModel.DataTransfer.Clipboard clipboard class.
    /// </summary>
    internal class TextClipboard : ITextClipboard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public async Task SetText(string text)
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> GetText()
        {
            // TODO: Do not use Result.
            return GetTextAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ContainsTextAsync()
        {
            return !string.IsNullOrEmpty(await GetTextAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<bool> ContainsText()
        {
            // TODO: Do not use Result.
            return ContainsTextAsync();
        }
    }
}
