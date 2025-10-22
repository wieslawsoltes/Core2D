// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections.Generic;
using System.IO;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model;

public interface IMetafileExporter
{
    MemoryStream? MakeMetafileStream(object bitmap, IEnumerable<BaseShapeViewModel> shapes, IImageCache ic);
    MemoryStream? MakeMetafileStream(object bitmap, FrameContainerViewModel container, IImageCache ic);
}
