// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export.Options;

public partial class OpenXmlExportOptionsViewModel : ExportOptionsBase
{
    private const string DefaultNamingTemplate = "{ProjectName}_{PageNumber}";

    [AutoNotify] private string _namingTemplate = DefaultNamingTemplate;
    [AutoNotify] private bool _preferVectorContent = true;
    [AutoNotify] private bool _includePayload = true;
    [AutoNotify] private bool _usePageSize = true;

    public OpenXmlExportOptionsViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, "OpenXML Export", "DrawingML-based export configuration.")
    {
    }

    public override void Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(NamingTemplate))
        {
            errors.Add("Naming template cannot be empty.");
        }

        SetValidationResult(errors.Count == 0, errors);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => new OpenXmlExportOptionsViewModel(ServiceProvider)
        {
            DisplayName = DisplayName,
            Description = Description,
            NamingTemplate = NamingTemplate,
            PreferVectorContent = PreferVectorContent,
            IncludePayload = IncludePayload,
            UsePageSize = UsePageSize
        };
}
