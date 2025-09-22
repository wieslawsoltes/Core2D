// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using System.IO;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model;

public sealed class DwgImportResult
{
    public DwgImportResult(IList<BaseShapeViewModel> shapes, IList<BlockShapeViewModel> blocks)
    {
        Shapes = shapes;
        Blocks = blocks;
    }

    public IList<BaseShapeViewModel> Shapes { get; }

    public IList<BlockShapeViewModel> Blocks { get; }
}

public interface IDwgImporter
{
    DwgImportResult? Import(Stream stream);
}
