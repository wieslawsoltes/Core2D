// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model;

public interface IPathConverter
{
    PathShapeViewModel? ToPathShape(ISet<BaseShapeViewModel>? shapes);

    PathShapeViewModel? ToPathShape(BaseShapeViewModel? shape);

    PathShapeViewModel? ToStrokePathShape(BaseShapeViewModel? shape);

    PathShapeViewModel? ToFillPathShape(BaseShapeViewModel? shape);

    PathShapeViewModel? ToWindingPathShape(BaseShapeViewModel? shape);

    PathShapeViewModel? Simplify(BaseShapeViewModel? shape);

    PathShapeViewModel? Op(ISet<BaseShapeViewModel>? shapes, PathOp op);

    public PathShapeViewModel? FromSvgPathData(string? svgPath, bool isStroked, bool isFilled);

    public string? ToSvgPathData(BaseShapeViewModel? shape);
}
