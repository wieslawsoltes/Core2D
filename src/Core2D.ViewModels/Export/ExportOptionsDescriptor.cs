// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export;

public sealed record ExportOptionsDescriptor(
    string WriterName,
    string DisplayName,
    string Description,
    string Category,
    IReadOnlyList<string> Capabilities,
    string Extension,
    Func<IServiceProvider?, ExportOptionsBase> Factory);
