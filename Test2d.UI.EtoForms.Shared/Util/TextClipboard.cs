// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Eto.Forms;
using System;
using System.Threading.Tasks;
using Test2d;

namespace TestEtoForms
{
    /// <summary>
    /// Wrapper class for Eto.Forms.Clipboard clipboard class.
    /// </summary>
    internal class TextClipboard : ITextClipboard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public Task SetText(string text)
        {
            return Task.Run(() =>
            {
                Application.Instance.Invoke(() =>
                {
                    new Clipboard().Text = text;
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> GetText()
        {
            return Task.Run(() =>
            {
                return Application.Instance.Invoke(() =>
                {
                    return new Clipboard().Text;
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<bool> ContainsText()
        {
            return Task.Run(() =>
            {
                return Application.Instance.Invoke(() =>
                {
                    return !string.IsNullOrEmpty(new Clipboard().Text);
                });
            });
        }
    }
}
