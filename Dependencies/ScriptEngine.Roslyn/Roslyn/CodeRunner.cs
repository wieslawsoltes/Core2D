// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CodeRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shapes"></param>
        /// <param name="context"></param>
        public abstract void Run(int id, BaseShape[] shapes, EditorContext context);
    }
}
