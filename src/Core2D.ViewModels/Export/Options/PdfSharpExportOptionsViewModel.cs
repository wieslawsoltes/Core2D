// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export.Options;

public partial class PdfSharpExportOptionsViewModel : ExportOptionsBase
{
    private const double DefaultDpi = 96.0;

    [AutoNotify] private bool _useTemplatePageSize = true;
    [AutoNotify] private double _dpi = DefaultDpi;
    [AutoNotify] private bool _landscape;
    [AutoNotify] private bool _embedFonts = true;
    [AutoNotify] private string _title = string.Empty;
    [AutoNotify] private string _author = string.Empty;

    public PdfSharpExportOptionsViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, "PDF (PdfSharp)", "Vector PDF export powered by PdfSharp.")
    {
    }

    public override void Validate()
    {
        var errors = new List<string>();
        if (Dpi <= 0)
        {
            errors.Add("DPI must be greater than zero.");
        }

        SetValidationResult(errors.Count == 0, errors);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => new PdfSharpExportOptionsViewModel(ServiceProvider)
        {
            DisplayName = DisplayName,
            Description = Description,
            UseTemplatePageSize = UseTemplatePageSize,
            Dpi = Dpi,
            Landscape = Landscape,
            EmbedFonts = EmbedFonts,
            Title = Title,
            Author = Author
        };
}
