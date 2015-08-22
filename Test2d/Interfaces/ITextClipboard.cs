// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITextClipboard
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ContainsText();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetText();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        void SetText(string text);
    }
}
