// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using ACadSharp;
using ACadSharp.IO;
using ACadSharp.Objects;
using ACadSharp.Tables;

namespace Core2D.Modules.Renderer.Dwg;

internal class DwgExportPresenter : IContainerPresenter
{
    public void Render(object? dc, IShapeRenderer? renderer, ISelection? selection, FrameContainerViewModel? container, double dx, double dy)
    {
        if (dc is null || renderer?.State is null || container is null)
        {
            return;
        }

        var flags = renderer.State.DrawShapeState;
        renderer.State.DrawShapeState = ShapeStateFlags.Printable;

        if (container is PageContainerViewModel page && page.Template is { })
        {
            renderer.Fill(dc, dx, dy, page.Template.Width, page.Template.Height, page.Template.Background);
            DrawContainer(dc, renderer, selection, page.Template);
        }

        DrawContainer(dc, renderer, selection, container);

        renderer.State.DrawShapeState = flags;
    }

    private void DrawContainer(object dc, IShapeRenderer renderer, ISelection? selection, FrameContainerViewModel container)
    {
        if (dc is not CadDocument doc)
        {
            return;
        }

        foreach (var layer in container.Layers)
        {
            // Reuse existing layer if already present to avoid duplicate key exceptions
            var candidate = new Layer(layer.Name) { IsOn = layer.IsVisible };
            var acadLayer = doc.Layers.TryAdd(candidate);
            // Ensure visibility reflects the current container's layer setting
            acadLayer.IsOn = layer.IsVisible;

            if (renderer is DwgRenderer dwgRenderer)
            {
                dwgRenderer._currentLayer = acadLayer;
            }

            DrawLayer(dc, renderer, selection, layer);
        }
    }

    private void DrawLayer(object dc, IShapeRenderer renderer, ISelection? selection, LayerContainerViewModel layer)
    {
        if (renderer.State is null)
        {
            return;
        }

        foreach (var shape in layer.Shapes)
        {
            if (shape.State.HasFlag(renderer.State.DrawShapeState))
            {
                shape.DrawShape(dc, renderer, selection);
            }
        }

        foreach (var shape in layer.Shapes)
        {
            if (shape.State.HasFlag(renderer.State.DrawShapeState))
            {
                shape.DrawPoints(dc, renderer, selection);
            }
        }
    }
}

public partial class DwgRenderer : IProjectExporter
{
    public void Save(Stream stream, PageContainerViewModel container)
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
        doc.Header.InsUnits = ACadSharp.Types.Units.UnitsType.Millimeters;

        // Single page export: route to model space
        _targetBlock = null;

        Add(doc, container, presenter);

        using var writer = new DwgWriter(stream, doc);
        writer.Write();
        ClearCache();
    }

    public void Save(Stream stream, DocumentContainerViewModel document)
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
        doc.Header.InsUnits = ACadSharp.Types.Units.UnitsType.Millimeters;

        Add(doc, document, presenter);

        using var writer = new DwgWriter(stream, doc);
        writer.Write();
        ClearCache();
    }

    public void Save(Stream stream, ProjectContainerViewModel project)
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
        doc.Header.InsUnits = ACadSharp.Types.Units.UnitsType.Millimeters;

        Add(doc, project, presenter);

        using var writer = new DwgWriter(stream, doc);
        writer.Write();
        ClearCache();
    }

    private void Add(CadDocument doc, PageContainerViewModel container, IContainerPresenter presenter)
    {
        var dataFlow = ServiceProvider.GetService<DataFlow>();
        var db = (object)container.Properties;
        var record = (object?)container.Record;

        if (dataFlow is { })
        {
            dataFlow.Bind(container.Template, db, record);
            dataFlow.Bind(container, db, record);
        }

        if (container.Template is { })
        {
            _pageHeight = container.Template.Height;
            presenter.Render(doc, this, null, container.Template, 0, 0);
        }
        else
        {
            throw new NullReferenceException("Container template must be set.");
        }

        presenter.Render(doc, this, null, container, 0, 0);
    }

    private void Add(CadDocument doc, DocumentContainerViewModel document, IContainerPresenter presenter)
    {
        foreach (var page in document.Pages)
        {
            if (page.Template is null)
            {
                continue;
            }

            var name = page.Template.Name;
            var width = page.Template.Width;
            var height = page.Template.Height;

            var layout = new Layout(page.Name)
            {
                PaperSize = $"{name}_({width}_x_{height}_MM)",
                UnprintableMargin = new PaperMargin(0, 0, 0, 0),
                PaperWidth = width,
                PaperHeight = height,
                PaperUnits = PlotPaperUnits.Millimeters,
                PaperRotation = PlotRotation.NoRotation
            };

            doc.Layouts.Add(layout);
            // Route subsequent entities to this layout's paper space block
            _targetBlock = layout.AssociatedBlock;
            Add(doc, page, presenter);
            _targetBlock = null;
        }
    }

    private void Add(CadDocument doc, ProjectContainerViewModel project, IContainerPresenter presenter)
    {
        foreach (var document in project.Documents)
        {
            Add(doc, document, presenter);
        }
    }
}
