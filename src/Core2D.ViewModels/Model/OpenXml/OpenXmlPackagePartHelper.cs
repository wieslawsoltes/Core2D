// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace Core2D.Model.OpenXml;

public static class OpenXmlPackagePartHelper
{
    private const string RelationshipType = "http://schemas.core2d.app/relationships/project";
    private const string Namespace = "http://schemas.core2d.app/project";
    private const string RootElementName = "core2dPayload";

    public static void WritePayload(OpenXmlPartContainer container, string payload)
    {
        ArgumentNullException.ThrowIfNull(container);

        if (string.IsNullOrWhiteSpace(payload))
        {
            return;
        }

        var existing = FindPayloadPart(container);
        if (existing is null)
        {
            existing = container.AddNewPart<CustomXmlPart>(RelationshipType);
        }

        var document = new XDocument(new XElement(XName.Get(RootElementName, Namespace), payload));
        using var stream = existing.GetStream(FileMode.Create, FileAccess.Write);
        document.Save(stream);
    }

    public static string? ReadPayload(OpenXmlPartContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);

        var part = FindPayloadPart(container);
        if (part is null)
        {
            return null;
        }

        using var stream = part.GetStream(FileMode.Open, FileAccess.Read);
        var document = XDocument.Load(stream);
        return document.Root?.Value;
    }

    private static CustomXmlPart? FindPayloadPart(OpenXmlPartContainer container)
    {
        return container.Parts
            .Select(p => p.OpenXmlPart)
            .OfType<CustomXmlPart>()
            .FirstOrDefault(IsPayloadPart);
    }

    private static bool IsPayloadPart(CustomXmlPart part)
    {
        try
        {
            using var stream = part.GetStream(FileMode.Open, FileAccess.Read);
            if (!stream.CanRead || stream.Length == 0)
            {
                return false;
            }

            var document = XDocument.Load(stream);
            return document.Root?.Name == XName.Get(RootElementName, Namespace);
        }
        catch
        {
            return false;
        }
    }
}
