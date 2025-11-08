// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.ViewModels.Wizard.Export.Scopes;

public static class ExportScopeFormatter
{
    public static string Describe(ExportScopeSelection scope)
    {
        return scope.Kind switch
        {
            ExportScopeKind.Page when scope.Page is { } page => $"Page: {page.Name}",
            ExportScopeKind.Document when scope.Document is { } document => $"Document: {document.Name}",
            _ => $"Project: {scope.Project.Name}"
        };
    }
}
