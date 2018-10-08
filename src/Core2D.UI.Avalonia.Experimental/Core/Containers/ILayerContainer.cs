// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public interface ILayerContainer : IDrawable
    {
        double Width { get; set; }
        double Height { get; set; }
        ArgbColor PrintBackground { get; set; }
        ArgbColor WorkBackground { get; set; }
        ArgbColor InputBackground { get; set; }
        ObservableCollection<LineShape> Guides { get; set; }
        ObservableCollection<BaseShape> Shapes { get; set; }
        IEnumerable<PointShape> GetPoints();
    }
}
