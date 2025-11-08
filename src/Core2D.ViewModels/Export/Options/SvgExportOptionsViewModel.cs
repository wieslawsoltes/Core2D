// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export.Options;

public enum SvgViewBoxMode
{
    TemplateBounds,
    PageBounds
}

public partial class SvgExportOptionsViewModel : ExportOptionsBase
{
    [AutoNotify] private SvgViewBoxMode _viewBox = SvgViewBoxMode.TemplateBounds;
    [AutoNotify] private int _decimalPrecision = 3;
    [AutoNotify] private bool _embedFonts;
    [AutoNotify] private bool _emitCss;

    public SvgExportOptionsViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, "SVG Export", "Scalable Vector Graphics export.")
    {
    }

    public override void Validate()
    {
        var errors = new List<string>();

        if (DecimalPrecision is < 0 or > 6)
        {
            errors.Add("Precision must be between 0 and 6.");
        }

        SetValidationResult(errors.Count == 0, errors);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => new SvgExportOptionsViewModel(ServiceProvider)
        {
            DisplayName = DisplayName,
            Description = Description,
            ViewBox = ViewBox,
            DecimalPrecision = DecimalPrecision,
            EmbedFonts = EmbedFonts,
            EmitCss = EmitCss
        };
}
