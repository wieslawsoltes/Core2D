// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public enum DxfDuplicateRecordCloningFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        NotApplicable = 0,
        /// <summary>
        /// 
        /// </summary>
        KeepExisting = 1,
        /// <summary>
        /// 
        /// </summary>
        UseClone = 2,
        /// <summary>
        /// 
        /// </summary>
        XrefPrefixName = 3,
        /// <summary>
        /// 
        /// </summary>
        PrefixName = 4,
        /// <summary>
        /// 
        /// </summary>
        UnmangleName = 5
    }
}
