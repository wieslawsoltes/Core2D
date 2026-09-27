// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Avalonia;
using Core2D.ViewModels.Editor;

namespace Core2D;

/// <summary>Read-only browser health observations. Editing still uses native Avalonia input.</summary>
[SupportedOSPlatform("browser")]
internal static partial class BrowserDiagnostics
{
    private static ProjectEditorViewModel? Editor => Application.Current?.DataContext as ProjectEditorViewModel;

    [JSExport]
    public static bool IsReady() => Editor?.RootDock is not null;

    [JSExport]
    public static bool HasProject() => Editor?.Project is not null;

    [JSExport]
    public static int GetShapeCount() => Editor?.Project?.CurrentContainer?.CurrentLayer?.Shapes.Length ?? 0;

    [JSExport]
    public static string GetCurrentTool() => Editor?.CurrentTool?.Title ?? string.Empty;
}
