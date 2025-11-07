// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using System.IO;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Model;

public sealed class PdfImportedImage
{
    public PdfImportedImage(string key, byte[] data)
    {
        Key = key;
        Data = data;
    }

    public string Key { get; }

    public byte[] Data { get; }
}

public sealed class PdfImportResult
{
    public PdfImportResult(
        IList<BaseShapeViewModel> shapes,
        IList<ShapeStyleViewModel> styles,
        IList<PdfImportedImage> images,
        double pageWidth,
        double pageHeight)
    {
        Shapes = shapes;
        Styles = styles;
        Images = images;
        PageWidth = pageWidth;
        PageHeight = pageHeight;
    }

    public IList<BaseShapeViewModel> Shapes { get; }

    public IList<ShapeStyleViewModel> Styles { get; }

    public IList<PdfImportedImage> Images { get; }

    public double PageWidth { get; }

    public double PageHeight { get; }
}

public interface IPdfImporter
{
    PdfImportResult? Import(Stream stream);
}
