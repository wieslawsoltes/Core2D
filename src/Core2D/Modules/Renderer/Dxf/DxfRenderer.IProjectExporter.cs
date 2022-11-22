﻿#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using DXF = netDxf;
using DXFH = netDxf.Header;
using DXFO = netDxf.Objects;
using DXFT = netDxf.Tables;

namespace Core2D.Modules.Renderer.Dxf;

internal class DxfExportPresenter : IContainerPresenter
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
        if (dc is not DXF.DxfDocument dxf)
        {
            return;
        }

        foreach (var layer in container.Layers)
        {
            var dxfLayer = new DXFT.Layer(layer.Name)
            {
                IsVisible = layer.IsVisible
            };

            dxf.Layers.Add(dxfLayer);

            if (renderer is DxfRenderer dxfRenderer)
            {
                dxfRenderer._currentLayer = dxfLayer;
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

public partial class DxfRenderer : IProjectExporter
{
    public void Save(Stream stream, PageContainerViewModel container)
    {
        var presenter = new DxfExportPresenter();

        if (stream is FileStream fileStream)
        {
            _outputPath = Path.GetDirectoryName(fileStream.Name);
        }
        else
        {
            _outputPath = string.Empty;
        }

        var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

        Add(dxf, container, presenter);

        dxf.Save(stream);
        ClearCache();
    }

    public void Save(Stream stream, DocumentContainerViewModel document)
    {
        var presenter = new DxfExportPresenter();

        if (stream is FileStream fileStream)
        {
            _outputPath = Path.GetDirectoryName(fileStream.Name);
        }
        else
        {
            _outputPath = string.Empty;
        }

        var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

        Add(dxf, document, presenter);

        dxf.Save(stream);
        ClearCache();
    }

    public void Save(Stream stream, ProjectContainerViewModel project)
    {
        var presenter = new DxfExportPresenter();

        if (stream is FileStream fileStream)
        {
            _outputPath = Path.GetDirectoryName(fileStream.Name);
        }
        else
        {
            _outputPath = string.Empty;
        }

        var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

        Add(dxf, project, presenter);

        dxf.Save(stream);
        ClearCache();
    }

    private void Add(DXF.DxfDocument dxf, PageContainerViewModel container, IContainerPresenter presenter)
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
            // _pageWidth = container.Template.Width;
            _pageHeight = container.Template.Height;
            presenter.Render(dxf, this, null, container.Template, 0, 0);
        }
        else
        {
            throw new NullReferenceException("Container template must be set.");
        }

        presenter.Render(dxf, this, null, container, 0, 0);
    }

    private void Add(DXF.DxfDocument dxf, DocumentContainerViewModel document, IContainerPresenter presenter)
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
 
            var layout = new DXFO.Layout(page.Name)
            {
                
                PlotSettings = new DXFO.PlotSettings
                {
                    PaperSizeName = $"{name}_({width}_x_{height}_MM)",
                    PaperMargin = new DXFO.PaperMargin(0, 0, 0, 0),
                    PaperSize = new DXF.Vector2(width, height),
                    Origin = new DXF.Vector2(0.0, 0.0),
                    PaperUnits = DXFO.PlotPaperUnits.Milimeters,
                    PaperRotation = DXFO.PlotRotation.NoRotation
                }
            };

            dxf.Layouts.Add(layout);
            dxf.ActiveLayout = layout.Name;

            Add(dxf, page, presenter);
        }
    }

    private void Add(DXF.DxfDocument dxf, ProjectContainerViewModel project, IContainerPresenter presenter)
    {
        foreach (var document in project.Documents)
        {
            Add(dxf, document, presenter);
        }
    }
}
