// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using System.IO;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Model;

public sealed class WmfImportResult
{
    public WmfImportResult(IList<BaseShapeViewModel> shapes, IList<ShapeStyleViewModel> styles)
    {
        Shapes = shapes;
        Styles = styles;
    }

    public IList<BaseShapeViewModel> Shapes { get; }

    public IList<ShapeStyleViewModel> Styles { get; }
}

public interface IWmfImporter
{
    WmfImportResult? Import(Stream stream);
}
