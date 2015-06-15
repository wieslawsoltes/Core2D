// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICodeEngine
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="context"></param>
        void Build(ImmutableArray<BaseShape> shapes, object context);

        /// <summary>
        /// 
        /// </summary>
        void Run();

        /// <summary>
        /// 
        /// </summary>
        void Reset();
    }
}
