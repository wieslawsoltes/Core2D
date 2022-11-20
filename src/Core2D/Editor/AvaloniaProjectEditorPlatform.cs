#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.Services;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.Editor;

public class AvaloniaProjectEditorPlatform : ViewModelBase, IProjectEditorPlatform
{
    private static List<FilePickerFileType> GetProjectFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.Project,
            StorageService.All
        };
    }

    private static List<FilePickerFileType> GetJsonFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.Json,
            StorageService.All
        };
    }

    private static List<FilePickerFileType> GetSvgFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.ImageSvg,
            StorageService.ImageSvgz,
            StorageService.All
        };
    }

    private static List<FilePickerFileType> GetScriptFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.CSharpScript,
            StorageService.All
        };
    }

    private static List<FilePickerFileType> GetPickerItemFileTypes(IEnumerable<IPickerItem> items, bool includeAllFilter)
    {
        var result = new List<FilePickerFileType>();
        
        foreach (var item in items)
        {
            switch (item.Extension.ToLower())
            {
                case "json":
                    result.Add(StorageService.Json);
                    break;
                case "cs":
                    result.Add(StorageService.CSharp);
                    break;
                case "csx":
                    result.Add(StorageService.CSharpScript);
                    break;
                case "png":
                    result.Add(StorageService.ImagePng);
                    break;
                case "jpg":
                case "jpeg":
                    result.Add(StorageService.ImageJpg);
                    break;
                case "skp":
                    result.Add(StorageService.ImageSkp);
                    break;
                case "bmp":
                    result.Add(StorageService.ImageBmp);
                    break;
                case "svg":
                    result.Add(StorageService.ImageSvg);
                    break;
                case "svgz":
                    result.Add(StorageService.ImageSvgz);
                    break;
                case "xml":
                    result.Add(StorageService.Xml);
                    break;
                case "xaml":
                    result.Add(StorageService.Xaml);
                    break;
                case "axaml":
                    result.Add(StorageService.Axaml);
                    break;
                case "pdf":
                    result.Add(StorageService.Pdf);
                    break;
                case "xps":
                    result.Add(StorageService.Xps);
                    break;
                case "xlsx":
                    result.Add(StorageService.Xlsx);
                    break;
                case "csv":
                    result.Add(StorageService.Csv);
                    break;
                case "project":
                    result.Add(StorageService.Project);
                    break;
                default:
                {
                    var filePickerFileType = new FilePickerFileType(item.Name)
                    {
                        Patterns = new[] {$"*.{item.Extension}"},
                        // TODO:
                        AppleUniformTypeIdentifiers = new[] {$"public.{item.Extension}"},
                        // TODO:
                        MimeTypes = new[] {$"application/{item.Extension}"}
                    };
                    result.Add(filePickerFileType);
                    break;
                }
            }
        }

        if (includeAllFilter)
        {
            result.Add(StorageService.All);
        }

        return result;
    }

    private IStorageFile? _openProjectFile;

    public AvaloniaProjectEditorPlatform(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public async void OnOpen()
    {
        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor is null)
            {
                return;
            }

            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open project",
                FileTypeFilter = GetProjectFileTypes(),
                AllowMultiple = false
            });

            var file = result.FirstOrDefault();
            if (file is not null && file.CanOpenRead)
            {
                _openProjectFile = file;
                await using var stream = await _openProjectFile.OpenReadAsync();
                editor.OnOpenProject(stream, _openProjectFile.Name);
                editor.CanvasPlatform?.InvalidateControl?.Invoke();
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnSave()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(editor.ProjectName) && _openProjectFile is { })
        {
            try
            {
                await using var stream = await _openProjectFile.OpenWriteAsync();
                editor.OnSaveProject(stream, editor.ProjectName);
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }
        else
        {
            OnSaveAs();
        }
    }

    public async void OnSaveAs()
    {
        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor is null)
            {
                return;
            }

            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save project",
                FileTypeChoices = GetProjectFileTypes(),
                SuggestedFileName = Path.GetFileNameWithoutExtension("Project"),
                DefaultExtension = "project",
                ShowOverwritePrompt = true
            });

            if (file is not null && file.CanOpenWrite)
            {
                _openProjectFile = file;
                await using var stream = await _openProjectFile.OpenWriteAsync();
                editor.OnSaveProject(stream, _openProjectFile.Name);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnClose()
    {
        _openProjectFile?.Dispose();
        _openProjectFile = null;
    }
    
    public async void OnImportJson(object? param)
    {
        try
        {
            if (param is null)
            {
                var storageProvider = StorageService.GetStorageProvider();
                if (storageProvider is null)
                {
                    return;
                }

                var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Import json",
                    FileTypeFilter = GetJsonFileTypes(),
                    AllowMultiple = true
                });

                foreach (var file in result)
                {
                    if (file.CanOpenRead)
                    {
                        await using var stream = await file.OpenReadAsync();
                        ServiceProvider.GetService<ProjectEditorViewModel>()?.OnImportJson(stream);
                    }
                }
            }
            else
            {
                if (param is not string path)
                {
                    return;
                }

                var fileSystem = ServiceProvider.GetService<IFileSystem>();
                if (fileSystem is { } && fileSystem.Exists(path))
                {
                    await using var stream = fileSystem.Open(path);
                    if (stream is { })
                    {
                        ServiceProvider.GetService<ProjectEditorViewModel>()?.OnImportJson(stream);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnImportSvg(object? param)
    {
        try
        {
            if (param is null)
            {
                var storageProvider = StorageService.GetStorageProvider();
                if (storageProvider is null)
                {
                    return;
                }

                var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Import svg",
                    FileTypeFilter = GetSvgFileTypes(),
                    AllowMultiple = true
                });

                foreach (var file in result)
                {
                    if (file.CanOpenRead)
                    {
                        await using var stream = await file.OpenReadAsync();
                        ServiceProvider.GetService<ProjectEditorViewModel>()?.OnImportSvg(stream);
                    }
                }
            }
            else
            {
                if (param is not string path)
                {
                    return;
                }

                var fileSystem = ServiceProvider.GetService<IFileSystem>();
                if (fileSystem is { } && fileSystem.Exists(path))
                {
                    await using var stream = fileSystem.Open(path);
                    if (stream is { })
                    {
                        ServiceProvider.GetService<ProjectEditorViewModel>()?.OnImportSvg(stream);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnExportJson(object? param)
    {
        if (param is null)
        {
            return;
        }

        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor is null)
            {
                return;
            }

            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export json",
                FileTypeChoices = GetJsonFileTypes(),
                SuggestedFileName = editor.GetName(param),
                DefaultExtension = "json",
                ShowOverwritePrompt = true
            });

            if (file is not null && file.CanOpenWrite)
            {
                await using var stream = await file.OpenWriteAsync();
                editor.OnExportJson(stream, param);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnExport(object? param)
    {
        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor?.Project is null)
            {
                return;
            }

            var name = string.Empty;

            if (param is null || param is ProjectEditorViewModel)
            {
                name = editor.Project.Name;
                param = editor.Project;
            }
            else if (param is ProjectContainerViewModel project)
            {
                name = project.Name;
            }
            else if (param is DocumentContainerViewModel document)
            {
                name = document.Name;
            }
            else if (param is FrameContainerViewModel container)
            {
                name = container.Name;
            }

            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export",
                FileTypeChoices = GetPickerItemFileTypes(editor.FileWriters, true),
                SuggestedFileName = name,
                DefaultExtension = editor.FileWriters.FirstOrDefault()?.Extension,
                ShowOverwritePrompt = true
            });

            if (file is not null && file.CanOpenWrite)
            {
                var ext = Path.GetExtension(file.Name).ToLower().TrimStart('.');
                var writer = editor.FileWriters.FirstOrDefault(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0);
                if (writer is { })
                {
                    await using var stream = await file.OpenWriteAsync();
                    editor.OnExport(stream, param, writer);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnExecuteScriptFile(object? param)
    {
        try
        {
            if (param is null)
            {
                var storageProvider = StorageService.GetStorageProvider();
                if (storageProvider is null)
                {
                    return;
                }

                var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open script", 
                    FileTypeFilter = GetScriptFileTypes(), 
                    AllowMultiple = true
                });

                foreach (var file in result)
                {
                    if (file.CanOpenRead)
                    {
                        await using var stream = await file.OpenReadAsync();
                        var editorViewModel = ServiceProvider.GetService<ProjectEditorViewModel>();
                        if (editorViewModel is { })
                        {
                            await editorViewModel.OnExecuteScript(stream);
                        }
                    }
                }
            }
            else
            {
                if (param is not Stream stream)
                {
                    return;
                }

                var editorViewModel = ServiceProvider.GetService<ProjectEditorViewModel>();
                if (editorViewModel is { })
                {
                    await editorViewModel.OnExecuteScript(stream);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnExit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.Shutdown();
        }
    }

    public void OnCopyAsSvg(object? param)
    {
        try
        {
            if (param is null)
            {
                var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
                var textClipboard = ServiceProvider.GetService<ITextClipboard>();
                var exporter = ServiceProvider.GetService<ISvgExporter>();

                if (editor?.Project is null || textClipboard is null || exporter is null)
                {
                    return;
                }

                var container = editor.Project.CurrentContainer;
                if (container is null)
                {
                    return;
                }
                
                var width = 0.0;
                var height = 0.0;
                switch (container)
                {
                    case TemplateContainerViewModel template:
                        width = template.Width;
                        height = template.Height;
                        break;
                    case PageContainerViewModel page:
                    {
                        if (page.Template is { })
                        {
                            width = page.Template.Width;
                            height = page.Template.Height;
                        }
                        break;
                    }
                }

                var sources = editor.Project.SelectedShapes;
                if (sources is { })
                {
                    var xaml = exporter.Create(sources, width, height);
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        textClipboard.SetText(xaml);
                    }
                    return;
                }

                {
                    var shapes = container.Layers.SelectMany(x => x.Shapes);
                    var xaml = exporter.Create(shapes, width, height);
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        textClipboard.SetText(xaml);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnPasteSvg()
    {
        try
        {
            var textClipboard = ServiceProvider.GetService<ITextClipboard>();
            var clipboard = ServiceProvider.GetService<IClipboardService>();
            var converter = ServiceProvider.GetService<ISvgConverter>();

            if (textClipboard is null || clipboard is null || converter is null)
            {
                return;
            }

            var svgText = await textClipboard.GetText();
            if (!string.IsNullOrEmpty(svgText))
            {
                var shapes = converter.FromString(svgText, out _, out _);
                if (shapes is { })
                {
                    clipboard.OnPasteShapes(shapes);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnCopyAsXaml(object? param)
    {
        try
        {
            if (param is not null)
            {
                return;
            }

            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var textClipboard = ServiceProvider.GetService<ITextClipboard>();
            var exporter = ServiceProvider.GetService<IXamlExporter>();

            if (editor?.Project is null || textClipboard is null || exporter is null)
            {
                return;
            }

            var container = editor.Project.CurrentContainer;
            if (container is null)
            {
                return;
            }

            var sources = editor.Project.SelectedShapes;
            if (sources is { })
            {
                var xaml = exporter.Create(sources, null);
                if (!string.IsNullOrEmpty(xaml))
                {
                    textClipboard.SetText(xaml);
                }
                return;
            }

            var shapes = new List<BaseShapeViewModel>();

            if (container is PageContainerViewModel page)
            {
                if (page.Template is { } template)
                {
                    shapes.AddRange(template.Layers.SelectMany(x => x.Shapes));
                }
                shapes.AddRange(page.Layers.SelectMany(x => x.Shapes));
            }
            else
            {
                shapes.AddRange(container.Layers.SelectMany(x => x.Shapes));
            }

            {
                var key = container.Name;
                var xaml = exporter.Create(shapes, key);
                if (!string.IsNullOrEmpty(xaml))
                {
                    textClipboard.SetText(xaml);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    private static class Win32
    {
        public const uint CF_ENHMETAFILE = 14;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CopyEnhMetaFile(IntPtr hemetafileSrc, IntPtr hNULL);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteEnhMetaFile(IntPtr hemetafile);
    }

    private void SetClipboardMetafile(MemoryStream ms)
    {
        using var metafile = new System.Drawing.Imaging.Metafile(ms);

        var emfHandle = metafile.GetHenhmetafile();
        if (emfHandle.Equals(IntPtr.Zero))
        {
            return;
        }

        var emfCloneHandle = Win32.CopyEnhMetaFile(emfHandle, IntPtr.Zero);
        if (emfCloneHandle.Equals(IntPtr.Zero))
        {
            return;
        }

        try
        {
            if (Win32.OpenClipboard(IntPtr.Zero) && Win32.EmptyClipboard())
            {
                Win32.SetClipboardData(Win32.CF_ENHMETAFILE, emfCloneHandle);
                Win32.CloseClipboard();
            }
        }
        finally
        {
            Win32.DeleteEnhMetaFile(emfHandle);
        }
    }

    public void OnCopyAsEmf(object? param)
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            return;
        }

        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var exporter = ServiceProvider.GetService<IMetafileExporter>();
            if (editor?.Project is null || editor.DataFlow is null || exporter is null)
            {
                return;
            }

            var imageCache = editor.Project as IImageCache;
            var container = editor.Project.CurrentContainer;
            if (container is null)
            {
                return;
            }
            
            var shapes = editor.Project.SelectedShapes;
            var db = (object?)container.Properties;
            var record = (object?)container.Record;

            var width = 0.0;
            var height = 0.0;

            if (container is PageContainerViewModel page)
            {
                editor.DataFlow.Bind(page.Template, db, record);
                if (page.Template is { })
                {
                    width = page.Template.Width;
                    height = page.Template.Height;
                }
            }
            else if (container is TemplateContainerViewModel template)
            {
                width = template.Width;
                height = template.Height;
            }

            editor.DataFlow.Bind(container, db, record);

            using var bitmap = new System.Drawing.Bitmap((int)width, (int)height);

            if (shapes is { } && shapes.Count > 0)
            {
                using var ms = exporter.MakeMetafileStream(bitmap, shapes, imageCache);
                if (ms is { })
                {
                    ms.Position = 0;
                    SetClipboardMetafile(ms);
                }
            }
            else
            {
                using var ms = exporter.MakeMetafileStream(bitmap, container, imageCache);
                if (ms is { })
                {
                    ms.Position = 0;
                    SetClipboardMetafile(ms);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnCopyAsPathData(object? param)
    {
        if (param is null)
        {
            return;
        }

        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var textClipboard = ServiceProvider.GetService<ITextClipboard>();
            var converter = ServiceProvider.GetService<IPathConverter>();
            if (editor?.Project is null || textClipboard is null || converter is null)
            {
                return;
            }

            var container = editor.Project.CurrentContainer;
            var shapes = editor.Project.SelectedShapes ?? container?.Layers.SelectMany(x => x.Shapes);
            if (shapes is null)
            {
                return;
            }

            var sb = new StringBuilder();

            foreach (var shape in shapes)
            {
                var svgPath = converter.ToSvgPathData(shape);
                if (!string.IsNullOrEmpty(svgPath))
                {
                    sb.Append(svgPath);
                }
            }

            var result = sb.ToString();
            if (!string.IsNullOrEmpty(result))
            {
                await textClipboard.SetText(result);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnPastePathDataStroked()
    {
        try
        {
            var textClipboard = ServiceProvider.GetService<ITextClipboard>();
            var clipboard = ServiceProvider.GetService<IClipboardService>();
            var converter = ServiceProvider.GetService<IPathConverter>();
            if (textClipboard is null || clipboard is null || converter is null)
            {
                return;
            }

            var svgPath = await textClipboard.GetText();
            if (!string.IsNullOrEmpty(svgPath))
            {
                var pathShape = converter.FromSvgPathData(svgPath, isStroked: true, isFilled: false);
                if (pathShape is { })
                {
                    clipboard.OnPasteShapes(Enumerable.Repeat<BaseShapeViewModel>(pathShape, 1));
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnPastePathDataFilled()
    {
        try
        {
            var textClipboard = ServiceProvider.GetService<ITextClipboard>();
            var clipboard = ServiceProvider.GetService<IClipboardService>();
            var converter = ServiceProvider.GetService<IPathConverter>();
            if (textClipboard is null || clipboard is null || converter is null)
            {
                return;
            }

            var svgPath = await textClipboard.GetText();
            if (!string.IsNullOrEmpty(svgPath))
            {
                var pathShape = converter.FromSvgPathData(svgPath, isStroked: false, isFilled: true);
                if (pathShape is { })
                {
                    clipboard.OnPasteShapes(Enumerable.Repeat<BaseShapeViewModel>(pathShape, 1));
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnImportData(object? param)
    {
        if (param is not ProjectContainerViewModel project)
        {
            return;
        }

        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor is null)
            {
                return;
            }
            
            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Import data",
                FileTypeFilter = GetPickerItemFileTypes(editor.TextFieldReaders, true),
                AllowMultiple = false
            });

            var file = result.FirstOrDefault();
            if (file is not null && file.CanOpenRead)
            {
                string ext = Path.GetExtension(file.Name).ToLower().TrimStart('.');
                var reader = editor.TextFieldReaders.FirstOrDefault(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0);
                if (reader is { })
                {
                    await using var stream = await file.OpenReadAsync();
                    editor.OnImportData(project, stream, reader);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnExportData(object? param)
    {
        if (param is not DatabaseViewModel db)
        {
            return;
        }

        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor is null)
            {
                return;
            }

            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export data",
                FileTypeChoices = GetPickerItemFileTypes(editor.TextFieldWriters, true),
                SuggestedFileName = db.Name,
                DefaultExtension = editor.TextFieldWriters.FirstOrDefault()?.Extension,
                ShowOverwritePrompt = true
            });

            if (file is not null && file.CanOpenWrite)
            {
                await using var stream = await file.OpenWriteAsync();
                var ext = Path.GetExtension(file.Name).ToLower().TrimStart('.');
                var writer = editor.TextFieldWriters.FirstOrDefault(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0);
                if (writer is { })
                {
                    editor.OnExportData(stream, db, writer);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async void OnUpdateData(object? param)
    {
        if (param is not DatabaseViewModel db)
        {
            return;
        }

        try
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor is null)
            {
                return;
            }
            
            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Update data",
                FileTypeFilter = GetPickerItemFileTypes(editor.TextFieldReaders, true),
                AllowMultiple = false
            });

            var file = result.FirstOrDefault();
            if (file is not null && file.CanOpenRead)
            {
                await using var stream = await file.OpenWriteAsync();
                var ext = Path.GetExtension(file.Name).ToLower().TrimStart('.');
                var reader = editor.TextFieldReaders.FirstOrDefault(w => string.Compare(w.Extension, ext, StringComparison.OrdinalIgnoreCase) == 0);
                if (reader is { })
                {
                    editor.OnUpdateData(stream, db, reader);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnAboutDialog()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.AboutInfo is null)
        {
            return;
        }

        var dialog = new DialogViewModel(ServiceProvider, editor)
        {
            Title = $"About {editor.AboutInfo.Title}",
            IsOverlayVisible = true,
            IsTitleBarVisible = true,
            IsCloseButtonVisible = true,
            ViewModel = editor.AboutInfo
        };

        editor.ShowDialog(dialog);
    }

    public void OnZoomReset()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CanvasPlatform?.ResetZoom?.Invoke();
    }

    public void OnZoomFill()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CanvasPlatform?.FillZoom?.Invoke();
    }

    public void OnZoomUniform()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CanvasPlatform?.UniformZoom?.Invoke();
    }

    public void OnZoomUniformToFill()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CanvasPlatform?.UniformToFillZoom?.Invoke();
    }

    public void OnZoomAutoFit()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CanvasPlatform?.AutoFitZoom?.Invoke();
    }

    public void OnZoomIn()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CanvasPlatform?.InZoom?.Invoke();
    }

    public void OnZoomOut()
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.CanvasPlatform?.OutZoom?.Invoke();
    }
}
