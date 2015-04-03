// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public abstract class XShape : ObservableObject
    {
        public abstract void Draw(object dc, IRenderer renderer, double dx, double dy);
    }
}
