// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model;

public interface IConnectable
{
    bool Connect(PointShapeViewModel? point, PointShapeViewModel? target);

    bool Disconnect(PointShapeViewModel? point, out PointShapeViewModel? result);

    bool Disconnect();
}