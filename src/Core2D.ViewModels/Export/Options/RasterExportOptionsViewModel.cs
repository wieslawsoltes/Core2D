// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export.Options;

public partial class RasterExportOptionsViewModel : ExportOptionsBase
{
    private const int DefaultWidth = 1920;
    private const int DefaultHeight = 1080;

    [AutoNotify] private bool _usePageBounds = true;
    [AutoNotify] private int _width = DefaultWidth;
    [AutoNotify] private int _height = DefaultHeight;
    [AutoNotify] private double _scale = 1.0;
    [AutoNotify] private bool _preserveAspectRatio = true;
    [AutoNotify] private bool _transparentBackground = true;
    [AutoNotify] private string _background = "#FFFFFFFF";

    public RasterExportOptionsViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, "Raster Image", "Raster export configuration.")
    {
    }

    public override void Validate()
    {
        var errors = new List<string>();

        if (!UsePageBounds)
        {
            if (Width <= 0 || Height <= 0)
            {
                errors.Add("Width and height must be positive when not using page bounds.");
            }
        }

        if (Scale <= 0)
        {
            errors.Add("Scale must be greater than zero.");
        }

        SetValidationResult(errors.Count == 0, errors);
    }

    public override object Copy(IDictionary<object, object>? shared)
        => new RasterExportOptionsViewModel(ServiceProvider)
        {
            DisplayName = DisplayName,
            Description = Description,
            UsePageBounds = UsePageBounds,
            Width = Width,
            Height = Height,
            Scale = Scale,
            PreserveAspectRatio = PreserveAspectRatio,
            TransparentBackground = TransparentBackground,
            Background = Background
        };
}
