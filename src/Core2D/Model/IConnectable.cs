// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model;

public interface IConnectable
{
    bool Connect(PointShapeViewModel? point, PointShapeViewModel? target);

    bool Disconnect(PointShapeViewModel? point, out PointShapeViewModel? result);

    bool Disconnect();
}
