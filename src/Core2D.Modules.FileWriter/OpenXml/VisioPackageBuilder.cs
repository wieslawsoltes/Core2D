// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Core2D.Modules.FileWriter.OpenXml;

internal sealed class VisioPackageBuilder
{
    private const string PayloadRelationshipType = "http://schemas.core2d.app/relationships/project";
    private static readonly XNamespace ContentTypesNs = "http://schemas.openxmlformats.org/package/2006/content-types";
    private static readonly XNamespace PackageRelationshipsNs = "http://schemas.openxmlformats.org/package/2006/relationships";
    private static readonly XNamespace OfficeRelationshipsNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private static readonly XNamespace VisioNs = "http://schemas.microsoft.com/office/visio/2012/main";
    private static readonly XNamespace CoreNs = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
    private static readonly XNamespace DcNs = "http://purl.org/dc/elements/1.1/";
    private static readonly XNamespace DcTermsNs = "http://purl.org/dc/terms/";
    private static readonly XNamespace DcTypeNs = "http://purl.org/dc/dcmitype/";
    private static readonly XNamespace XsiNs = "http://www.w3.org/2001/XMLSchema-instance";
    private static readonly XNamespace AppNs = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
    private static readonly XNamespace VtNs = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";

    private readonly IReadOnlyList<VisioPageDefinition> _pages;
    private readonly IReadOnlyList<VisioMasterDefinition> _masters;
    private readonly string _payload;
    private readonly string _title;

    public VisioPackageBuilder(IReadOnlyList<VisioPageDefinition> pages, IReadOnlyList<VisioMasterDefinition> masters, string payload, string title)
    {
        _pages = pages;
        _masters = masters;
        _payload = payload;
        _title = string.IsNullOrWhiteSpace(title) ? "Core2D Export" : title;
    }

    public void Save(Stream stream)
    {
        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);
        WriteXml(archive, "[Content_Types].xml", CreateContentTypes());
        WriteXml(archive, "_rels/.rels", CreateRootRelationships());
        WriteXml(archive, "visio/document.xml", CreateDocument());
        WriteXml(archive, "visio/_rels/document.xml.rels", CreateDocumentRelationships());
        WriteMasters(archive);
        WriteXml(archive, "visio/pages/pages.xml", CreatePagesPart());
        WriteXml(archive, "visio/pages/_rels/pages.xml.rels", CreatePagesRelationships());
        WriteXml(archive, "visio/windows.xml", CreateWindowsPart());
        WriteXml(archive, "docProps/core.xml", CreateCoreProperties());
        WriteXml(archive, "docProps/app.xml", CreateAppProperties());
        WritePayload(archive);
        WritePages(archive);
    }

    private static void WriteXml(ZipArchive archive, string entryName, XDocument document)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        using var stream = entry.Open();
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = false,
            OmitXmlDeclaration = false
        };

        using var writer = XmlWriter.Create(stream, settings);
        document.Save(writer);
    }

    private void WritePages(ZipArchive archive)
    {
        foreach (var (page, index) in _pages.Select((p, i) => (p, i + 1)))
        {
            var entryName = $"visio/pages/page{index}.xml";
            WriteXml(archive, entryName, CreatePageContent(page));

            var relName = $"visio/pages/_rels/page{index}.xml.rels";
            WriteXml(archive, relName, CreateEmptyRelationships());
        }
    }

    private void WriteMasters(ZipArchive archive)
    {
        if (_masters.Count == 0)
        {
            return;
        }

        WriteXml(archive, "visio/masters/masters.xml", CreateMastersPart());
        WriteXml(archive, "visio/masters/_rels/masters.xml.rels", CreateMastersRelationships());

        foreach (var master in _masters)
        {
            var entryName = $"visio/masters/master{master.Id}.xml";
            WriteXml(archive, entryName, CreateMasterContent(master));
        }
    }

    private void WritePayload(ZipArchive archive)
    {
        var entry = archive.CreateEntry("customXml/item1.xml", CompressionLevel.Optimal);
        using var stream = entry.Open();
        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        var document = new XDocument(new XElement(XName.Get("core2dPayload", "http://schemas.core2d.app/project"), _payload));
        document.Save(writer);
    }

    private XDocument CreateContentTypes()
    {
        var types = new XElement(ContentTypesNs + "Types",
            new XElement(ContentTypesNs + "Default",
                new XAttribute("Extension", "rels"),
                new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")),
            new XElement(ContentTypesNs + "Default",
                new XAttribute("Extension", "xml"),
                new XAttribute("ContentType", "application/xml")),
            new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", "/visio/document.xml"),
                new XAttribute("ContentType", "application/vnd.ms-visio.drawing.main+xml")),
            new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", "/visio/pages/pages.xml"),
                new XAttribute("ContentType", "application/vnd.ms-visio.pages+xml")),
            new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", "/visio/windows.xml"),
                new XAttribute("ContentType", "application/vnd.ms-visio.windows+xml")),
            new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", "/docProps/core.xml"),
                new XAttribute("ContentType", "application/vnd.openxmlformats-package.core-properties+xml")),
            new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", "/docProps/app.xml"),
                new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.extended-properties+xml")),
            new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", "/customXml/item1.xml"),
                new XAttribute("ContentType", "application/xml")));

        if (_masters.Count > 0)
        {
            types.Add(new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", "/visio/masters/masters.xml"),
                new XAttribute("ContentType", "application/vnd.ms-visio.masters+xml")));

            foreach (var master in _masters)
            {
                types.Add(new XElement(ContentTypesNs + "Override",
                    new XAttribute("PartName", $"/visio/masters/master{master.Id}.xml"),
                    new XAttribute("ContentType", "application/vnd.ms-visio.master+xml")));
            }
        }

        foreach (var (page, index) in _pages.Select((p, i) => (p, i + 1)))
        {
            types.Add(new XElement(ContentTypesNs + "Override",
                new XAttribute("PartName", $"/visio/pages/page{index}.xml"),
                new XAttribute("ContentType", "application/vnd.ms-visio.page+xml")));
        }

        return new XDocument(types);
    }

    private XDocument CreateRootRelationships()
    {
        var relationships = new XElement(PackageRelationshipsNs + "Relationships",
            new XElement(PackageRelationshipsNs + "Relationship",
                new XAttribute("Id", "rId1"),
                new XAttribute("Type", "http://schemas.microsoft.com/visio/2010/relationships/document"),
                new XAttribute("Target", "visio/document.xml")),
            new XElement(PackageRelationshipsNs + "Relationship",
                new XAttribute("Id", "rIdPayload"),
                new XAttribute("Type", PayloadRelationshipType),
                new XAttribute("Target", "customXml/item1.xml")));
        return new XDocument(relationships);
    }

    private XDocument CreateDocument()
    {
        var document = new XElement(VisioNs + "VisioDocument",
            new XAttribute(XNamespace.Xmlns + "r", OfficeRelationshipsNs));

        var settings = new XElement(VisioNs + "DocumentSettings",
            new XAttribute("TopPage", "0"),
            new XAttribute("DefaultTextStyle", "0"),
            new XAttribute("DefaultLineStyle", "0"),
            new XAttribute("DefaultFillStyle", "0"),
            new XElement(VisioNs + "GlueSettings", "9"),
            new XElement(VisioNs + "SnapSettings", "65847"),
            new XElement(VisioNs + "SnapExtensions", "34"),
            new XElement(VisioNs + "DynamicGridEnabled", "1"));
        document.Add(settings);

        var colors = new XElement(VisioNs + "Colors",
            new XElement(VisioNs + "ColorEntry", new XAttribute("IX", "0"), new XAttribute("RGB", "#000000")),
            new XElement(VisioNs + "ColorEntry", new XAttribute("IX", "1"), new XAttribute("RGB", "#FFFFFF")));
        document.Add(colors);

        var faces = new XElement(VisioNs + "FaceNames",
            new XElement(VisioNs + "FaceName",
                new XAttribute("NameU", "Calibri"),
                new XAttribute("UnicodeRanges", "-469750017 -1073732485 9 0"),
                new XAttribute("CharSets", "536871423 0"),
                new XAttribute("Panose", "2 15 5 2 2 2 4 3 2 4"),
                new XAttribute("Flags", "325")));
        document.Add(faces);

        var styleSheet = new XElement(VisioNs + "StyleSheet",
            new XAttribute("ID", "0"),
            new XAttribute("NameU", "Default"),
            new XAttribute("Name", "Default"),
            new XElement(VisioNs + "Cell", new XAttribute("N", "LineWeight"), new XAttribute("V", "0.01041666666666667")),
            new XElement(VisioNs + "Cell", new XAttribute("N", "LineColor"), new XAttribute("V", "#000000")),
            new XElement(VisioNs + "Cell", new XAttribute("N", "LinePattern"), new XAttribute("V", "1")),
            new XElement(VisioNs + "Cell", new XAttribute("N", "FillForegnd"), new XAttribute("V", "#FFFFFF")),
            new XElement(VisioNs + "Cell", new XAttribute("N", "FillPattern"), new XAttribute("V", "1")),
            new XElement(VisioNs + "Cell", new XAttribute("N", "TextBkgnd"), new XAttribute("V", "#FFFFFF")));
        var styles = new XElement(VisioNs + "StyleSheets", styleSheet);
        document.Add(styles);

        return new XDocument(document);
    }

    private XDocument CreateDocumentRelationships()
    {
        var relationships = new XElement(PackageRelationshipsNs + "Relationships");

        relationships.Add(new XElement(PackageRelationshipsNs + "Relationship",
            new XAttribute("Id", "rIdPages"),
            new XAttribute("Type", "http://schemas.microsoft.com/visio/2010/relationships/pages"),
            new XAttribute("Target", "pages/pages.xml")));

        relationships.Add(new XElement(PackageRelationshipsNs + "Relationship",
            new XAttribute("Id", "rIdWindows"),
            new XAttribute("Type", "http://schemas.microsoft.com/visio/2010/relationships/windows"),
            new XAttribute("Target", "windows.xml")));

        if (_masters.Count > 0)
        {
            relationships.Add(new XElement(PackageRelationshipsNs + "Relationship",
                new XAttribute("Id", "rIdMasters"),
                new XAttribute("Type", "http://schemas.microsoft.com/visio/2010/relationships/masters"),
                new XAttribute("Target", "masters/masters.xml")));
        }

        return new XDocument(relationships);
    }

    private XDocument CreatePagesPart()
    {
        var pages = new XElement(VisioNs + "Pages",
            new XAttribute(XNamespace.Xmlns + "r", OfficeRelationshipsNs));

        foreach (var (page, index) in _pages.Select((p, i) => (p, i)))
        {
            var viewCenterX = page.WidthInches * 0.5;
            var viewCenterY = page.HeightInches * 0.5;

            var pageElement = new XElement(VisioNs + "Page",
                new XAttribute("ID", index.ToString()),
                new XAttribute("NameU", page.Name),
                new XAttribute("Name", page.Name),
                new XAttribute("ViewScale", "1"),
                new XAttribute("ViewCenterX", viewCenterX.ToString("G17")),
                new XAttribute("ViewCenterY", viewCenterY.ToString("G17")));

            var pageSheet = new XElement(VisioNs + "PageSheet",
                new XAttribute("LineStyle", "0"),
                new XAttribute("FillStyle", "0"),
                new XAttribute("TextStyle", "0"),
                new XElement(VisioNs + "Cell", new XAttribute("N", "PageWidth"), new XAttribute("V", page.WidthInches.ToString("G17"))),
                new XElement(VisioNs + "Cell", new XAttribute("N", "PageHeight"), new XAttribute("V", page.HeightInches.ToString("G17"))),
                new XElement(VisioNs + "Cell", new XAttribute("N", "PageScale"), new XAttribute("V", "1")),
                new XElement(VisioNs + "Cell", new XAttribute("N", "DrawingScale"), new XAttribute("V", "1")));
            pageElement.Add(pageSheet);
            pageElement.Add(new XElement(VisioNs + "Rel", new XAttribute(OfficeRelationshipsNs + "id", $"rId{index + 1}")));
            pages.Add(pageElement);
        }

        return new XDocument(pages);
    }

    private XDocument CreateMastersPart()
    {
        var masters = new XElement(VisioNs + "Masters",
            new XAttribute(XNamespace.Xmlns + "r", OfficeRelationshipsNs),
            new XAttribute(XNamespace.Xml + "space", "preserve"));

        foreach (var (master, index) in _masters.Select((m, i) => (m, i + 1)))
        {
            var masterElement = new XElement(VisioNs + "Master",
                new XAttribute("ID", master.Id),
                new XAttribute("NameU", master.Name),
                new XAttribute("Name", master.Name),
                new XAttribute("UniqueID", master.UniqueId),
                new XAttribute("MasterType", "0"));

            var pageSheet = new XElement(VisioNs + "PageSheet",
                new XAttribute("LineStyle", "0"),
                new XAttribute("FillStyle", "0"),
                new XAttribute("TextStyle", "0"),
                new XElement(VisioNs + "Cell", new XAttribute("N", "PageWidth"), new XAttribute("V", master.WidthInches.ToString("G17"))),
                new XElement(VisioNs + "Cell", new XAttribute("N", "PageHeight"), new XAttribute("V", master.HeightInches.ToString("G17"))),
                new XElement(VisioNs + "Cell", new XAttribute("N", "PageScale"), new XAttribute("V", "1")),
                new XElement(VisioNs + "Cell", new XAttribute("N", "DrawingScale"), new XAttribute("V", "1")));

            masterElement.Add(pageSheet);
            masterElement.Add(new XElement(VisioNs + "Rel", new XAttribute(OfficeRelationshipsNs + "id", $"rId{index}")));
            masters.Add(masterElement);
        }

        return new XDocument(masters);
    }

    private XDocument CreateMastersRelationships()
    {
        var relationships = new XElement(PackageRelationshipsNs + "Relationships");
        foreach (var (master, index) in _masters.Select((m, i) => (m, i + 1)))
        {
            relationships.Add(new XElement(PackageRelationshipsNs + "Relationship",
                new XAttribute("Id", $"rId{index}"),
                new XAttribute("Type", "http://schemas.microsoft.com/visio/2010/relationships/master"),
                new XAttribute("Target", $"master{master.Id}.xml")));
        }

        return new XDocument(relationships);
    }

    private XDocument CreateMasterContent(VisioMasterDefinition master)
    {
        var shapes = new XElement(VisioNs + "Shapes");
        foreach (var shape in master.Shapes)
        {
            shapes.Add(new XElement(shape));
        }

        var contents = new XElement(VisioNs + "MasterContents",
            new XAttribute(XNamespace.Xmlns + "r", OfficeRelationshipsNs),
            new XAttribute(XNamespace.Xml + "space", "preserve"),
            shapes);

        return new XDocument(contents);
    }

    private XDocument CreatePagesRelationships()
    {
        var relationships = new XElement(PackageRelationshipsNs + "Relationships");
        foreach (var (page, index) in _pages.Select((p, i) => (p, i + 1)))
        {
            relationships.Add(new XElement(PackageRelationshipsNs + "Relationship",
                new XAttribute("Id", $"rId{index}"),
                new XAttribute("Type", "http://schemas.microsoft.com/visio/2010/relationships/page"),
                new XAttribute("Target", $"page{index}.xml")));
        }

        return new XDocument(relationships);
    }

    private XDocument CreateWindowsPart()
    {
        var first = _pages.FirstOrDefault();
        var width = first?.WidthInches ?? 8.5;
        var height = first?.HeightInches ?? 11.0;
        var window = new XElement(VisioNs + "Window",
            new XAttribute("ID", "0"),
            new XAttribute("WindowType", "Drawing"),
            new XAttribute("WindowState", "1073741824"),
            new XAttribute("WindowLeft", "0"),
            new XAttribute("WindowTop", "0"),
            new XAttribute("WindowWidth", "1200"),
            new XAttribute("WindowHeight", "800"),
            new XAttribute("ContainerType", "Page"),
            new XAttribute("Page", "0"),
            new XAttribute("ViewScale", "1"),
            new XAttribute("ViewCenterX", (width * 0.5).ToString("G17")),
            new XAttribute("ViewCenterY", (height * 0.5).ToString("G17")),
            new XElement(VisioNs + "ShowRulers", "1"),
            new XElement(VisioNs + "ShowGrid", "1"),
            new XElement(VisioNs + "ShowGuides", "1"),
            new XElement(VisioNs + "ShowConnectionPoints", "1"),
            new XElement(VisioNs + "GlueSettings", "9"),
            new XElement(VisioNs + "SnapSettings", "65847"),
            new XElement(VisioNs + "SnapExtensions", "34"),
            new XElement(VisioNs + "DynamicGridEnabled", "1"));

        var windows = new XElement(VisioNs + "Windows",
            new XAttribute("ClientWidth", "1920"),
            new XAttribute("ClientHeight", "1080"),
            window);
        return new XDocument(windows);
    }

    private XDocument CreatePageContent(VisioPageDefinition page)
    {
        var contents = new XElement(VisioNs + "PageContents");
        var shapes = new XElement(VisioNs + "Shapes");
        foreach (var shape in page.Shapes)
        {
            shapes.Add(shape);
        }
        contents.Add(shapes);
        return new XDocument(contents);
    }

    private static XDocument CreateEmptyRelationships()
    {
        var relationships = new XElement(PackageRelationshipsNs + "Relationships");
        return new XDocument(relationships);
    }

    private XDocument CreateCoreProperties()
    {
        var now = DateTime.UtcNow;
        var created = XmlConvert.ToString(now, XmlDateTimeSerializationMode.Utc);
        var core = new XElement(CoreNs + "coreProperties",
            new XAttribute(XNamespace.Xmlns + "cp", CoreNs),
            new XAttribute(XNamespace.Xmlns + "dc", DcNs),
            new XAttribute(XNamespace.Xmlns + "dcterms", DcTermsNs),
            new XAttribute(XNamespace.Xmlns + "dcmitype", DcTypeNs),
            new XAttribute(XNamespace.Xmlns + "xsi", XsiNs),
            new XElement(DcNs + "title", _title),
            new XElement(DcNs + "creator", "Core2D"),
            new XElement(CoreNs + "lastModifiedBy", "Core2D"),
            new XElement(DcTermsNs + "created",
                new XAttribute(XsiNs + "type", "dcterms:W3CDTF"),
                created),
            new XElement(DcTermsNs + "modified",
                new XAttribute(XsiNs + "type", "dcterms:W3CDTF"),
                created));
        return new XDocument(core);
    }

    private XDocument CreateAppProperties()
    {
        var properties = new XElement(AppNs + "Properties",
            new XAttribute(XNamespace.Xmlns + "vt", VtNs),
            new XElement(AppNs + "Application", "Core2D"),
            new XElement(AppNs + "DocSecurity", "0"),
            new XElement(AppNs + "Pages", _pages.Count.ToString()),
            new XElement(AppNs + "Company", string.Empty),
            new XElement(AppNs + "LinksUpToDate", "false"),
            new XElement(AppNs + "SharedDoc", "false"),
            new XElement(AppNs + "HyperlinksChanged", "false"),
            new XElement(AppNs + "AppVersion", "16.0"));
        return new XDocument(properties);
    }
}
