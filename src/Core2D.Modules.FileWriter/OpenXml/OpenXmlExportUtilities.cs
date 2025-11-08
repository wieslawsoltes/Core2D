// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core2D.Modules.Renderer.OpenXml;
using Core2D.ViewModels.Containers;

namespace Core2D.Modules.FileWriter.OpenXml;

internal static class OpenXmlExportUtilities
{
    private const double EmusPerPixel = 914400.0 / 96.0;

    public static IReadOnlyList<PageContainerViewModel> ExtractPages(object item)
    {
        return item switch
        {
            PageContainerViewModel page => new[] { page },
            DocumentContainerViewModel document => document.Pages.ToArray(),
            ProjectContainerViewModel project => project.Documents
                .SelectMany(d => d.Pages)
                .ToArray(),
            _ => Array.Empty<PageContainerViewModel>()
        };
    }

    public static string CreatePageName(PageContainerViewModel page, int index)
    {
        var name = string.IsNullOrWhiteSpace(page.Name)
            ? $"Page {index + 1}"
            : page.Name;

        foreach (var invalid in new[] { '\\', '/', '?', '*', '[', ']' })
        {
            name = name.Replace(invalid, '-');
        }

        if (name.Length > 31)
        {
            name = name.Substring(0, 31);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            name = $"Page {index + 1}";
        }

        return name;
    }

    public static long ToEmus(int value)
        => (long)Math.Round(value * EmusPerPixel, MidpointRounding.AwayFromZero);

    public static IEnumerable<OpenXmlRenderedPage> RenderPages(OpenXmlRenderer renderer, ProjectContainerViewModel project, IEnumerable<PageContainerViewModel> pages)
    {
        foreach (var page in pages)
        {
            var rendered = renderer.Render(page, project);
            if (rendered is { })
            {
                yield return rendered;
            }
        }
    }
}
