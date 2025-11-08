// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.OpenXml;
using Core2D.Modules.Renderer.OpenXml;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

namespace Core2D.Modules.FileWriter.OpenXml;

public sealed class PowerPointOpenXmlWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public PowerPointOpenXmlWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "PowerPoint (OpenXML)";

    public string Extension => "pptx";

    public void Save(Stream stream, object? item, object? options)
    {
        if (item is null || options is not ProjectContainerViewModel project)
        {
            return;
        }

        var jsonSerializer = _serviceProvider.GetService<IJsonSerializer>();
        if (jsonSerializer is null)
        {
            return;
        }

        var pages = OpenXmlExportUtilities.ExtractPages(item);
        if (pages.Count == 0)
        {
            return;
        }

        var bitmapRenderer = new OpenXmlRenderer(_serviceProvider);
        var drawingRenderer = new OpenXmlDrawingRenderer();
        var pptAdapter = new OpenXmlPowerPointShapeAdapter();
        var renderedPages = OpenXmlExportUtilities.RenderPages(bitmapRenderer, project, pages).ToList();
        if (renderedPages.Count == 0)
        {
            return;
        }

        var packageStream = stream.CanRead ? stream : new MemoryStream();
        var ownsPackageStream = !stream.CanRead;

        try
        {
            using var presentation = PresentationDocument.Create(packageStream, PresentationDocumentType.Presentation);
            var presentationPart = presentation.AddPresentationPart();
            presentationPart.Presentation = new Presentation();

            var slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();
            GenerateSlideMasterPart(slideMasterPart);

            var slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();
            GenerateSlideLayoutPart(slideLayoutPart);

            slideMasterPart.SlideMaster.AppendChild(new SlideLayoutIdList(
                new SlideLayoutId
                {
                    Id = 1U,
                    RelationshipId = slideMasterPart.GetIdOfPart(slideLayoutPart)
                }));
            slideMasterPart.SlideMaster.Save();

            presentationPart.Presentation.SlideMasterIdList = new SlideMasterIdList(
                new SlideMasterId
                {
                    Id = 2147483648U,
                    RelationshipId = presentationPart.GetIdOfPart(slideMasterPart)
                });

            var slideIdList = presentationPart.Presentation.AppendChild(new SlideIdList());
            uint slideId = 256U;

            foreach (var rendered in renderedPages)
            {
                var slidePart = presentationPart.AddNewPart<SlidePart>();
                GenerateSlidePart(slidePart, slideLayoutPart);

                var descriptors = drawingRenderer.Render(rendered.Page).ToList();
                if (descriptors.Count > 0)
                {
                    AppendShapes(slidePart, descriptors, pptAdapter);
                }
                else
                {
                    var imagePart = slidePart.AddImagePart(ImagePartType.Png);
                    using (var imageStream = new MemoryStream(rendered.Image))
                    {
                        imagePart.FeedData(imageStream);
                    }

                    var relId = slidePart.GetIdOfPart(imagePart);
                    AppendPicture(slidePart, relId, rendered);
                }

                slideIdList.Append(new SlideId
                {
                    Id = slideId++,
                    RelationshipId = presentationPart.GetIdOfPart(slidePart)
                });
            }

            presentationPart.Presentation.Save();

            var payloadJson = OpenXmlPayloadSerializer.Create(jsonSerializer, item);
            OpenXmlPackagePartHelper.WritePayload(presentationPart, payloadJson);
        }
        finally
        {
            if (ownsPackageStream && packageStream is MemoryStream ms)
            {
                ms.Position = 0;
                if (stream.CanSeek)
                {
                    stream.SetLength(0);
                    stream.Position = 0;
                }
                ms.CopyTo(stream);
                ms.Dispose();
            }
        }
    }

    private static void GenerateSlideMasterPart(SlideMasterPart slideMasterPart)
    {
        slideMasterPart.SlideMaster = new SlideMaster(
            new CommonSlideData(CreateShapeTree()),
            new ColorMap
            {
                Background1 = A.ColorSchemeIndexValues.Light1,
                Text1 = A.ColorSchemeIndexValues.Dark1,
                Background2 = A.ColorSchemeIndexValues.Light2,
                Text2 = A.ColorSchemeIndexValues.Dark2,
                Accent1 = A.ColorSchemeIndexValues.Accent1,
                Accent2 = A.ColorSchemeIndexValues.Accent2,
                Accent3 = A.ColorSchemeIndexValues.Accent3,
                Accent4 = A.ColorSchemeIndexValues.Accent4,
                Accent5 = A.ColorSchemeIndexValues.Accent5,
                Accent6 = A.ColorSchemeIndexValues.Accent6,
                Hyperlink = A.ColorSchemeIndexValues.Hyperlink,
                FollowedHyperlink = A.ColorSchemeIndexValues.FollowedHyperlink
            },
            new SlideLayoutIdList(),
            new TextStyles());
    }

    private static void GenerateSlideLayoutPart(SlideLayoutPart slideLayoutPart)
    {
        slideLayoutPart.SlideLayout = new SlideLayout(
            new CommonSlideData(CreateShapeTree()),
            new ColorMapOverride(new A.MasterColorMapping()));
        slideLayoutPart.SlideLayout.Save();
    }

    private static void GenerateSlidePart(SlidePart slidePart, SlideLayoutPart layoutPart)
    {
        slidePart.Slide = new Slide(
            new CommonSlideData(CreateShapeTree()),
            new ColorMapOverride(new A.MasterColorMapping()));
        slidePart.AddPart(layoutPart);
        slidePart.Slide.Save();
    }

    private static ShapeTree CreateShapeTree()
    {
        return new ShapeTree(
            new NonVisualGroupShapeProperties(
                new NonVisualDrawingProperties { Id = 1U, Name = "Shape Tree" },
                new NonVisualGroupShapeDrawingProperties(),
                new ApplicationNonVisualDrawingProperties()),
            new GroupShapeProperties(new A.TransformGroup()));
    }

    private static void AppendPicture(SlidePart slidePart, string relationshipId, OpenXmlRenderedPage rendered)
    {
        var tree = slidePart.Slide.CommonSlideData?.ShapeTree;
        if (tree is null)
        {
            return;
        }

        var pictureId = (uint)(tree.ChildElements.OfType<Shape>().Count() + tree.ChildElements.OfType<Picture>().Count() + 1000);

        var picture = new Picture(
            new NonVisualPictureProperties(
                new NonVisualDrawingProperties { Id = pictureId, Name = $"Picture {pictureId}" },
                new NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true }),
                new ApplicationNonVisualDrawingProperties()),
            new BlipFill(
                new A.Blip { Embed = relationshipId },
                new A.Stretch(new A.FillRectangle())),
            new ShapeProperties(
                new A.Transform2D(
                    new A.Offset { X = 0L, Y = 0L },
                    new A.Extents
                    {
                        Cx = OpenXmlExportUtilities.ToEmus(rendered.Width),
                        Cy = OpenXmlExportUtilities.ToEmus(rendered.Height)
                    }),
                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }));

        tree.Append(picture);
        slidePart.Slide.Save();
    }

    private static void AppendShapes(SlidePart slidePart, IEnumerable<OpenXmlShapeDescriptor> descriptors, OpenXmlPowerPointShapeAdapter adapter)
    {
        var tree = slidePart.Slide.CommonSlideData?.ShapeTree;
        if (tree is null)
        {
            return;
        }

        foreach (var descriptor in descriptors)
        {
            var shape = adapter.CreateShape(descriptor);
            tree.Append(shape);
        }

        slidePart.Slide.Save();
    }
}
