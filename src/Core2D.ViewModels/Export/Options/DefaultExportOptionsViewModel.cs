// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export.Options;

public partial class DefaultExportOptionsViewModel : ExportOptionsBase
{
    [AutoNotify] private string _notes = string.Empty;

    public DefaultExportOptionsViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, "Exporter", "Default exporter options.")
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
        => new DefaultExportOptionsViewModel(ServiceProvider)
        {
            DisplayName = DisplayName,
            Description = Description,
            Notes = Notes
        };
}
