// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;

namespace Core2D
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
        Task<bool> ContainsText();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> GetText();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task SetText(string text);
    }
}
