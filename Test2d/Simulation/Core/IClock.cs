// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSIM
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// 
        /// </summary>
        long Cycle { get; }
        /// <summary>
        /// 
        /// </summary>
        int Resolution { get; }
    }
}
