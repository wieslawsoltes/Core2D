// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export.Options;

public partial class SkiaPdfExportOptionsViewModel : ExportOptionsBase
{
    [AutoNotify] private bool _vectorOnly = true;
    [AutoNotify] private int _compressionLevel = 50;
    [AutoNotify] private string _colorProfile = "sRGB";

    public SkiaPdfExportOptionsViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, "PDF (SkiaSharp)", "SkiaSharp-backed PDF export.")
    {
    }

    public override void Validate()
    {
        var errors = new List<string>();
        if (CompressionLevel is < 0 or > 100)
        {
            errors.Add("Compression must be between 0 and 100.");
        }

        if (string.IsNullOrWhiteSpace(ColorProfile))
        {
            errors.Add("Color profile cannot be empty.");
        }

        SetValidationResult(errors.Count == 0, errors);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => new SkiaPdfExportOptionsViewModel(ServiceProvider)
        {
            DisplayName = DisplayName,
            Description = Description,
            VectorOnly = VectorOnly,
            CompressionLevel = CompressionLevel,
            ColorProfile = ColorProfile
        };
}
