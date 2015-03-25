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
        IList<XStyle> Styles { get; set; }
        XStyle CurrentStyle { get; set; }
        XShape PointShape { get; set; }
        IList<ILayer> Layers { get; set; }
        ILayer CurrentLayer { get; set; }
        ILayer WorkingLayer { get; set; }
        XShape CurrentShape { get; set; }
        void Clear();
        void Invalidate();
    }
}
