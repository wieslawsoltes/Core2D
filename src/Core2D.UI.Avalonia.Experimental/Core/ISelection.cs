// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shape;

namespace Core2D
{
    public interface ISelection
    {
        BaseShape Hover { get; set; }
        ISet<BaseShape> Selected { get; set; }
    }
}
