// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public interface ILayer
    {
        string Name { get; set; }
        bool IsVisible { get; set; }
        IList<BaseShape> Shapes { get; set; }
        void SetInvalidate(Action invalidate);
        void Invalidate();
    }
}
