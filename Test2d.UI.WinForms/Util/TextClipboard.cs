// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test2d;

namespace TestWinForms
{
    /// <summary>
    /// Wrapper class for System.Windows.Forms.Clipboard clipboard class.
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
                Clipboard.SetText(text, TextDataFormat.UnicodeText);
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
                return Clipboard.GetText(TextDataFormat.UnicodeText);
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
                return Clipboard.ContainsText(TextDataFormat.UnicodeText);
            });
        }
    }
}
