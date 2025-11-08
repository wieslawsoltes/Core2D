// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core2D.ViewModels.Wizard.Export.Exporters;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.ViewModels.Wizard.Export.Destination;

public static class ExportPathBuilder
{
    public static string BuildPath(ExportWizardContext context, ExportScopeSelection scope, ExporterSelectionViewModel exporter)
    {
        var tokens = CreateTokens(scope, exporter);
        var folder = ReplaceTokens(context.SubfolderTemplate ?? string.Empty, tokens);
        var fileName = ReplaceTokens(context.FileNameTemplate ?? string.Empty, tokens);

        var relative = string.IsNullOrWhiteSpace(folder)
            ? fileName
            : System.IO.Path.Combine(folder, fileName);

        if (string.IsNullOrWhiteSpace(context.DestinationFolder))
        {
            return relative;
        }

        return System.IO.Path.Combine(context.DestinationFolder, relative);
    }

    private static Dictionary<string, string> CreateTokens(ExportScopeSelection scope, ExporterSelectionViewModel exporter)
    {
        var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["{ProjectName}"] = Sanitize(scope.Project?.Name, "Project"),
            ["{DocumentName}"] = Sanitize(scope.Document?.Name, scope.Project?.Name ?? "Document"),
            ["{PageName}"] = Sanitize(scope.Page?.Name, scope.Document?.Name ?? scope.Project?.Name ?? "Page"),
            ["{PageNumber}"] = ResolvePageNumber(scope),
            ["{Exporter}"] = Sanitize(exporter.DisplayName, "Exporter"),
            ["{ExporterExtension}"] = Sanitize(exporter.Extension, "ext").TrimStart('.')
        };
        return tokens;
    }

    private static string ReplaceTokens(string template, IDictionary<string, string> tokens)
    {
        var result = template;
        foreach (var (key, value) in tokens)
        {
            result = result.Replace(key, value, StringComparison.OrdinalIgnoreCase);
        }
        return result;
    }

    private static string Sanitize(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            value = fallback;
        }

        foreach (var invalid in System.IO.Path.GetInvalidFileNameChars().Concat(System.IO.Path.GetInvalidPathChars()))
        {
            value = value.Replace(invalid, '_');
        }

        return value;
    }

    private static string ResolvePageNumber(ExportScopeSelection scope)
    {
        if (scope.Page is { } page && scope.Document is { } document)
        {
            var pages = document.Pages;
            if (!pages.IsDefaultOrEmpty)
            {
                var index = pages.IndexOf(page);
                if (index >= 0)
                {
                    return (index + 1).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        return "1";
    }
}
