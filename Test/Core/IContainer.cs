// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public interface IContainer
    {
        double Width { get; set; }
        double Height { get; set; }
        IList<ShapeStyle> Styles { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        BaseShape PointShape { get; set; }
        IList<ILayer> Layers { get; set; }
        ILayer CurrentLayer { get; set; }
        ILayer WorkingLayer { get; set; }
        BaseShape CurrentShape { get; set; }
        void Clear();
        void Invalidate();
    }
}
