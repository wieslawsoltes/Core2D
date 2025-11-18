// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Xml.Linq;

namespace Core2D.Model.OpenXml;

public static class VisioPackagePayloadHelper
{
    private const string PayloadRelationshipType = "http://schemas.core2d.app/relationships/project";

    public static string? ReadPayload(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return ReadPayload(buffer.ToArray());
    }

    public static string? ReadPayload(byte[] buffer)
    {
        if (buffer is null || buffer.Length == 0)
        {
            return null;
        }

        using var memory = new MemoryStream(buffer, writable: false);
        using var package = Package.Open(memory, FileMode.Open, FileAccess.Read);
        return ReadPayload(package);
    }

    private static string? ReadPayload(Package package)
    {
        var relationship = package.GetRelationshipsByType(PayloadRelationshipType).FirstOrDefault();
        if (relationship is null)
        {
            return null;
        }

        var partUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), relationship.TargetUri);
        if (!package.PartExists(partUri))
        {
            return null;
        }

        var part = package.GetPart(partUri);
        using var partStream = part.GetStream(FileMode.Open, FileAccess.Read);
        var document = XDocument.Load(partStream);
        return document.Root?.Value;
    }
}
