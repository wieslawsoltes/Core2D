// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

namespace Core2D.Services;

internal static class StorageService
{
    public static FilePickerFileType All { get; } = new("All")
    {
        Patterns = new[] { "*.*" },
        MimeTypes = new[] { "*/*" }
    };

    public static FilePickerFileType Json { get; } = new("Json")
    {
        Patterns = new[] { "*.json" },
        AppleUniformTypeIdentifiers = new[] { "public.json" },
        MimeTypes = new[] { "application/json" }
    };

    public static FilePickerFileType CSharp { get; } = new("C#")
    {
        Patterns = new[] { "*.cs" },
        AppleUniformTypeIdentifiers = new[] { "public.csharp-source" },
        MimeTypes = new[] { "text/plain" }
    };

    public static FilePickerFileType CSharpScript { get; } = new("C#")
    {
        Patterns = new[] { "*.cs", "*.csx" },
        AppleUniformTypeIdentifiers = new[] { "public.csharp-source" },
        MimeTypes = new[] { "text/plain" }
    };

    public static FilePickerFileType ImagePng { get; } = new("PNG image")
    {
        Patterns = new[] { "*.png" },
        AppleUniformTypeIdentifiers = new[] { "public.png" },
        MimeTypes = new[] { "image/png" }
    };

    public static FilePickerFileType ImageJpg { get; } = new("JPEG image")
    {
        Patterns = new[] { "*.jpg", "*.jpeg" },
        AppleUniformTypeIdentifiers = new[] { "public.jpeg" },
        MimeTypes = new[] { "image/jpeg" }
    };

    public static FilePickerFileType ImageSkp { get; } = new("SKP image")
    {
        Patterns = new[] { "*.skp" },
        AppleUniformTypeIdentifiers = new[] { "com.google.skp" },
        MimeTypes = new[] { "image/skp" }
    };

    public static FilePickerFileType ImageBmp { get; } = new("BMP image")
    {
        Patterns = new[] { "*.bmp" },
        AppleUniformTypeIdentifiers = new[] { "public.bmp" },
        MimeTypes = new[] { "image/bmp" }
    };

    public static FilePickerFileType ImageAll { get; } = new("All Images")
    {
        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" },
        AppleUniformTypeIdentifiers = new[] { "public.image" },
        MimeTypes = new[] { "image/*" }
    };

    public static FilePickerFileType ImageSvg { get; } = new("Svg")
    {
        Patterns = new[] { "*.svg" },
        AppleUniformTypeIdentifiers = new[] { "public.svg-image" },
        MimeTypes = new[] { "image/svg+xml" }
    };

    public static FilePickerFileType ImageSvgz { get; } = new("Svgz")
    {
        Patterns = new[] { "*.svgz" },
        // TODO:
        AppleUniformTypeIdentifiers = new[] { "public.svg-image" },
        // TODO:
        MimeTypes = new[] { "image/svg+xml" }
    };

    public static FilePickerFileType CadDwg { get; } = new("DWG")
    {
        Patterns = new[] { "*.dwg" },
        AppleUniformTypeIdentifiers = new[] { "com.autodesk.dwg" },
        MimeTypes = new[] { "image/vnd.dwg", "application/acad" }
    };

    public static FilePickerFileType CadDxf { get; } = new("DXF")
    {
        Patterns = new[] { "*.dxf" },
        AppleUniformTypeIdentifiers = new[] { "com.autodesk.dxf" },
        MimeTypes = new[] { "image/vnd.dxf", "application/dxf" }
    };

    public static FilePickerFileType Xml { get; } = new("Xml")
    {
        Patterns = new[] { "*.xml" },
        AppleUniformTypeIdentifiers = new[] { "public.xml" },
        MimeTypes = new[] { "application/xaml" }
    };

    public static FilePickerFileType Xaml { get; } = new("Xaml")
    {
        Patterns = new[] { "*.xaml" },
        // TODO:
        AppleUniformTypeIdentifiers = new[] { "public.xaml" },
        // TODO:
        MimeTypes = new[] { "application/xaml" }
    };

    public static FilePickerFileType Axaml { get; } = new("Axaml")
    {
        Patterns = new[] { "*.axaml" },
        // TODO:
        AppleUniformTypeIdentifiers = new[] { "public.axaml" },
        // TODO:
        MimeTypes = new[] { "application/axaml" }
    };

    public static FilePickerFileType Pdf { get; } = new("PDF document")
    {
        Patterns = new[] { "*.pdf" },
        AppleUniformTypeIdentifiers = new[] { "com.adobe.pdf" },
        MimeTypes = new[] { "application/pdf" }
    };

    public static FilePickerFileType Docx { get; } = new("Word document")
    {
        Patterns = new[] { "*.docx" },
        AppleUniformTypeIdentifiers = new[] { "org.openxmlformats.wordprocessingml.document" },
        MimeTypes = new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };

    public static FilePickerFileType Pptx { get; } = new("PowerPoint presentation")
    {
        Patterns = new[] { "*.pptx" },
        AppleUniformTypeIdentifiers = new[] { "org.openxmlformats.presentationml.presentation" },
        MimeTypes = new[] { "application/vnd.openxmlformats-officedocument.presentationml.presentation" }
    };

    public static FilePickerFileType Vsdx { get; } = new("Visio drawing")
    {
        Patterns = new[] { "*.vsdx" },
        AppleUniformTypeIdentifiers = new[] { "com.microsoft.visio.drawing" },
        MimeTypes = new[] { "application/vnd.ms-visio.drawing" }
    };

    public static FilePickerFileType Xps { get; } = new("XPS document")
    {
        Patterns = new[] { "*.xps" },
        AppleUniformTypeIdentifiers = new[] { "com.microsoft.xps" },
        MimeTypes = new[] { "application/oxps", "application/vnd.ms-xpsdocument" }
    };

    // TODO: xlsx
    
    public static FilePickerFileType Xlsx { get; } = new("XLSX document")
    {
        Patterns = new[] { "*.xlsx" },
        // TODO:
        AppleUniformTypeIdentifiers = new[] { "public.xlsx" },
        // TODO:
        MimeTypes = new[] { "application/xlsx" }
    };
    
    public static FilePickerFileType Csv { get; } = new("CSV document")
    {
        Patterns = new[] { "*.csv" },
        // TODO:
        AppleUniformTypeIdentifiers = new[] { "public.csv" },
        // TODO:
        MimeTypes = new[] { "application/csv" }
    };

    public static FilePickerFileType Project { get; } = new("Project")
    {
        Patterns = new[] { "*.project" },
        // TODO:
        AppleUniformTypeIdentifiers = new[] { "public.project" },
        // TODO:
        MimeTypes = new[] { "application/project" }
    };

    public static IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            return window.StorageProvider;
        }

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } mainView })
        {
            var visualRoot = mainView.GetVisualRoot();
            if (visualRoot is TopLevel topLevel)
            {
                return topLevel.StorageProvider;
            }
        }

        return null;
    }
}
