// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// 
        /// </summary>
        object DataContext { get; set; }
        /// <summary>
        /// 
        /// </summary>
        void Close();
    }
}
