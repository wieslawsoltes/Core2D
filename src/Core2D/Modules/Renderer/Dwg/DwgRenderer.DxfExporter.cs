// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using ACadSharp;
using ACadSharp.Objects;
using ACadSharp.Types.Units;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;

namespace Core2D.Modules.Renderer.Dwg;

public partial class DwgRenderer
{
    // DXF export using ACadSharp with the same rendering pipeline as DWG.

    public void SaveDxf(Stream stream, PageContainerViewModel container)
    {
        var presenter = new DwgExportPresenter();

        if (stream is FileStream fileStream)
        {
            _outputPath = Path.GetDirectoryName(fileStream.Name);
        }
        else
        {
            _outputPath = string.Empty;
        }

        var doc = new CadDocument();
        // Use millimeters as drawing insertion units for Core2D pages
        doc.Header.InsUnits = UnitsType.Millimeters;

        Add(doc, container, presenter);

        using var writer = new ACadSharp.IO.DxfWriter(stream, doc, binary: false);
        writer.Write();
        ClearCache();
    }

    public void SaveDxf(Stream stream, DocumentContainerViewModel document)
    {
        var presenter = new DwgExportPresenter();

        if (stream is FileStream fileStream)
        {
            _outputPath = Path.GetDirectoryName(fileStream.Name);
        }
        else
        {
            _outputPath = string.Empty;
        }

        var doc = new CadDocument();
        // Use millimeters as drawing insertion units for Core2D pages
        doc.Header.InsUnits = UnitsType.Millimeters;

        Add(doc, document, presenter);

        using var writer = new ACadSharp.IO.DxfWriter(stream, doc, binary: false);
        writer.Write();
        ClearCache();
    }

    public void SaveDxf(Stream stream, ProjectContainerViewModel project)
    {
        var presenter = new DwgExportPresenter();

        if (stream is FileStream fileStream)
        {
            _outputPath = Path.GetDirectoryName(fileStream.Name);
        }
        else
        {
            _outputPath = string.Empty;
        }

        var doc = new CadDocument();
        // Use millimeters as drawing insertion units for Core2D pages
        doc.Header.InsUnits = UnitsType.Millimeters;

        Add(doc, project, presenter);

        using var writer = new ACadSharp.IO.DxfWriter(stream, doc, binary: false);
        writer.Write();
        ClearCache();
    }
}
