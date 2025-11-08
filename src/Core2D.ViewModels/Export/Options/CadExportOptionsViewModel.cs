// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Export.Options;

public enum CadExportUnits
{
    ModelUnits,
    Millimeters,
    Inches
}

public partial class CadExportOptionsViewModel : ExportOptionsBase
{
    [AutoNotify] private CadExportUnits _units = CadExportUnits.ModelUnits;
    [AutoNotify] private bool _includeTemplateLayers = true;
    [AutoNotify] private bool _mergeLayers;
    [AutoNotify] private bool _explodeGroups;

    public CadExportOptionsViewModel(IServiceProvider? serviceProvider)
        : base(serviceProvider, "CAD Export", "DWG/DXF export configuration.")
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
        => new CadExportOptionsViewModel(ServiceProvider)
        {
            DisplayName = DisplayName,
            Description = Description,
            Units = Units,
            IncludeTemplateLayers = IncludeTemplateLayers,
            MergeLayers = MergeLayers,
            ExplodeGroups = ExplodeGroups
        };
}
