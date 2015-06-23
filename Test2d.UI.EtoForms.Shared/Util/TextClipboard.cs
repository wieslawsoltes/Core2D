// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Eto.Forms;
using System;
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
        public void SetText(string text)
        {
            new Clipboard().Text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return new Clipboard().Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ContainsText()
        {
            return !string.IsNullOrEmpty(new Clipboard().Text);
        }
    }
}
