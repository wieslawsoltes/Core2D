// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;

namespace Core2D.ViewModels.Export;

public interface IExportOptionsCatalog
{
    ExportOptionsBase Create(string writerName);

    bool TryCreate(string writerName, out ExportOptionsBase options);

    IReadOnlyCollection<ExportOptionsDescriptor> Describe();
}
