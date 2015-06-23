// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using Test2d;

namespace Test.Windows
{
    /// <summary>
    /// Wrapper class for System.Windows.Clipboard clipboard class.
    /// </summary>
    internal class TextClipboard : ITextClipboard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return Clipboard.GetText(TextDataFormat.UnicodeText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ContainsText()
        {
            return Clipboard.ContainsText(TextDataFormat.UnicodeText);
        }
    }
}
