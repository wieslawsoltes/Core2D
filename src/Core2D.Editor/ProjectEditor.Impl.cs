// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor.Bounds;
using Core2D.Editor.History;
using Core2D.Editor.Input;
using Core2D.Editor.Recent;
using Core2D.Editor.Tools;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Dock.Model;
using Dock.Model.Controls;
using Spatial;
using static System.Math;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor implementation.
    /// </summary>
    public partial class ProjectEditor
    {
        private object _scriptState;
        private IPageContainer _pageToCopy = default;
        private IDocumentContainer _documentToCopy = default;
        private IBaseShape _hoveredShape = default;

        public IBaseShape HoveredShape
        {
            get => _hoveredShape;
            set => _hoveredShape = value;
        }

        private void LogError(Exception ex)
        {
            Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            if (ex.InnerException != null)
            {
                LogError(ex.InnerException);
            }
        }

        /// <summary>
        /// Snap value by specified snap amount.
        /// </summary>
        /// <param name="value">The value to snap.</param>
        /// <param name="snap">The snap amount.</param>
        /// <returns>The snapped value.</returns>
        public static double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }

        /// <summary>
        /// Try to snap input arguments.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        /// <returns>The snapped value if enabled otherwise original position.</returns>
        public (double sx, double sy) TryToSnap(InputArgs args)
        {
            if (Project != null && Project.Options.SnapToGrid == true)
                return (Snap(args.X, Project.Options.SnapX), Snap(args.Y, Project.Options.SnapY));
            else
                return (args.X, args.Y);
        }

        /// <summary>
        /// Get object item name.
        /// </summary>
        /// <param name="item">The object item.</param>
        public string GetName(object item)
        {
            if (item != null)
            {
                if (item is IObservableObject observable)
                    return observable.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Create new project, document or page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnNew(object item)
        {
            if (item is IPageContainer page)
            {
                OnNewPage(page);
            }
            else if (item is IDocumentContainer document)
            {
                OnNewPage(document);
            }
            else if (item is IProjectContainer project)
            {
                OnNewDocument();
            }
            else if (item is ProjectEditor)
            {
                OnNewProject();
            }
            else if (item == null)
            {
                if (Project == null)
                {
                    OnNewProject();
                }
                else
                {
                    if (Project.CurrentDocument == null)
                    {
                        OnNewDocument();
                    }
                    else
                    {
                        OnNewPage(Project.CurrentDocument);
                    }
                }
            }
        }

        /// <summary>
        /// Create new page.
        /// </summary>
        /// <param name="selected">The selected page.</param>
        public void OnNewPage(IPageContainer selected)
        {
            var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(selected));
            if (document != null)
            {
                var page =
                    ContainerFactory?.GetPage(Project, Constants.DefaultPageName)
                    ?? Factory.CreatePageContainer(Constants.DefaultPageName);

                Project?.AddPage(document, page);
                Project?.SetCurrentContainer(page);
            }
        }

        /// <summary>
        /// Create new page.
        /// </summary>
        /// <param name="selected">The selected document.</param>
        public void OnNewPage(IDocumentContainer selected)
        {
            var page =
                ContainerFactory?.GetPage(Project, Constants.DefaultPageName)
                ?? Factory.CreatePageContainer(Constants.DefaultPageName);

            Project?.AddPage(selected, page);
            Project?.SetCurrentContainer(page);
        }

        /// <summary>
        /// Create new document.
        /// </summary>
        public void OnNewDocument()
        {
            var document =
                ContainerFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                ?? Factory.CreateDocumentContainer(Constants.DefaultDocumentName);

            Project?.AddDocument(document);
            Project?.SetCurrentDocument(document);
            Project?.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        /// <summary>
        /// Create new project.
        /// </summary>
        public void OnNewProject()
        {
            OnUnload();
            OnLoad(ContainerFactory?.GetProject() ?? Factory.CreateProjectContainer(), string.Empty);
            OnNavigate("EditorView");
            CanvasPlatform?.Invalidate?.Invoke();
        }

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void OnOpenProject(string path)
        {
            try
            {
                if (FileIO != null && JsonSerializer != null)
                {
                    if (!string.IsNullOrEmpty(path) && FileIO.Exists(path))
                    {
                        var project = Factory.OpenProjectContainer(path, FileIO, JsonSerializer);
                        if (project != null)
                        {
                            OnOpenProject(project, path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="project">The project to open.</param>
        /// <param name="path">The project file path.</param>
        public void OnOpenProject(IProjectContainer project, string path)
        {
            try
            {
                if (project != null)
                {
                    OnUnload();
                    OnLoad(project, path);
                    OnAddRecent(path, project.Name);
                    OnNavigate("EditorView");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Close project.
        /// </summary>
        public void OnCloseProject()
        {
            OnNavigate("DashboardView");
            Project?.History?.Reset();
            OnUnload();
        }

        /// <summary>
        /// Save project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void OnSaveProject(string path)
        {
            try
            {
                if (Project != null && FileIO != null && JsonSerializer != null)
                {
                    Factory.SaveProjectContainer(Project, path, FileIO, JsonSerializer);
                    OnAddRecent(path, Project.Name);

                    if (string.IsNullOrEmpty(ProjectPath))
                    {
                        ProjectPath = path;
                    }

                    IsProjectDirty = false;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="project">The target project.</param>
        /// <param name="path">The database file path.</param>
        public void OnImportData(IProjectContainer project, string path)
        {
            try
            {
                if (project != null)
                {
                    var db = CsvReader?.Read(path, FileIO);
                    if (db != null)
                    {
                        project.AddDatabase(db);
                        project.SetCurrentDatabase(db);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        public void OnImportData(string path)
        {
            OnImportData(Project, path);
        }

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnExportData(string path, IDatabase database)
        {
            try
            {
                CsvWriter?.Write(path, FileIO, database);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnUpdateData(string path, IDatabase database)
        {
            try
            {
                var db = CsvReader?.Read(path, FileIO);
                if (db != null)
                {
                    Project?.UpdateDatabase(database, db);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Import object.
        /// </summary>
        /// <param name="item">The object to import.</param>
        /// <param name="restore">Try to restore objects by name.</param>
        public void OnImportObject(object item, bool restore)
        {
            if (item is IShapeStyle style)
            {
                Project?.AddStyle(Project?.CurrentStyleLibrary, style);
            }
            else if (item is IList<IShapeStyle> styleList)
            {
                Project.AddItems(Project?.CurrentStyleLibrary, styleList);
            }
            else if (item is IBaseShape)
            {
                if (item is IGroupShape group)
                {
                    if (restore)
                    {
                        var shapes = Enumerable.Repeat(group, 1);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project.AddGroup(Project?.CurrentGroupLibrary, group);
                }
                else
                {
                    Project?.AddShape(Project?.CurrentContainer?.CurrentLayer, item as IBaseShape);
                }
            }
            else if (item is IList<IGroupShape> groups)
            {
                if (restore)
                {
                    TryToRestoreStyles(groups);
                    TryToRestoreRecords(groups);
                }
                Project.AddItems(Project?.CurrentGroupLibrary, groups);
            }
            else if (item is ILibrary<IShapeStyle> sl)
            {
                Project.AddStyleLibrary(sl);
            }
            else if (item is IList<ILibrary<IShapeStyle>> sll)
            {
                Project.AddStyleLibraries(sll);
            }
            else if (item is ILibrary<IGroupShape> gl)
            {
                TryToRestoreStyles(gl.Items);
                TryToRestoreRecords(gl.Items);
                Project.AddGroupLibrary(gl);
            }
            else if (item is IList<ILibrary<IGroupShape>> gll)
            {
                var shapes = gll.SelectMany(x => x.Items);
                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);
                Project.AddGroupLibraries(gll);
            }
            else if (item is IContext context)
            {
                if (Renderers?[0]?.State?.SelectedShape != null || (Renderers?[0]?.State?.SelectedShapes?.Count > 0))
                {
                    OnApplyData(context);
                }
                else
                {
                    var container = Project?.CurrentContainer;
                    if (container != null)
                    {
                        container.Data = context;
                    }
                }
            }
            else if (item is IDatabase db)
            {
                Project?.AddDatabase(db);
                Project?.SetCurrentDatabase(db);
            }
            else if (item is ILayerContainer layer)
            {
                if (restore)
                {
                    TryToRestoreStyles(layer.Shapes);
                    TryToRestoreRecords(layer.Shapes);
                }
                Project?.AddLayer(Project?.CurrentContainer, layer);
            }
            else if (item is IPageContainer page)
            {
                if (page.Template == null)
                {
                    // Import as template.
                    if (restore)
                    {
                        var shapes = page.Layers.SelectMany(x => x.Shapes);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project?.AddTemplate(page);
                }
                else
                {
                    // Import as page.
                    if (restore)
                    {
                        var shapes = Enumerable.Concat(
                            page.Layers.SelectMany(x => x.Shapes),
                            page.Template?.Layers.SelectMany(x => x.Shapes));
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project?.AddPage(Project?.CurrentDocument, page);
                }
            }
            else if (item is IList<IPageContainer> templates)
            {
                if (restore)
                {
                    var shapes = templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);
                }

                // Import as templates.
                Project.AddTemplates(templates);
            }
            else if (item is IDocumentContainer document)
            {
                if (restore)
                {
                    var shapes = Enumerable.Concat(
                        document.Pages.SelectMany(x => x.Layers).SelectMany(x => x.Shapes),
                        document.Pages.SelectMany(x => x.Template.Layers).SelectMany(x => x.Shapes));
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);
                }
                Project?.AddDocument(document);
            }
            else if (item is IOptions options)
            {
                if (Project != null)
                {
                    Project.Options = options;
                }
            }
            else if (item is IProjectContainer project)
            {
                OnUnload();
                OnLoad(project, string.Empty);
            }
            else
            {
                throw new NotSupportedException("Not supported import object.");
            }
        }

        /// <summary>
        /// Import Xaml from a file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        public void OnImportXaml(string path)
        {
            try
            {
                var xaml = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(xaml))
                {
                    OnImportXamlString(xaml);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Import Xaml string.
        /// </summary>
        /// <param name="xaml">The xaml string.</param>
        public void OnImportXamlString(string xaml)
        {
            var item = XamlSerializer?.Deserialize<object>(xaml);
            if (item != null)
            {
                OnImportObject(item, false);
            }
        }

        /// <summary>
        /// Export Xaml to a file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        /// <param name="item">The object item.</param>
        public void OnExportXaml(string path, object item)
        {
            try
            {
                var xaml = XamlSerializer?.Serialize(item);
                if (!string.IsNullOrWhiteSpace(xaml))
                {
                    FileIO?.WriteUtf8Text(path, xaml);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Import Json from a file.
        /// </summary>
        /// <param name="path">The json file path.</param>
        public void OnImportJson(string path)
        {
            try
            {
                var json = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    OnImportJsonString(json);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Import Json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        private void OnImportJsonString(string json)
        {
            var item = JsonSerializer.Deserialize<object>(json);
            if (item != null)
            {
                OnImportObject(item, true);
            }
        }

        /// <summary>
        /// Export Json to a file.
        /// </summary>
        /// <param name="path">The json file path.</param>
        /// <param name="item">The object item.</param>
        public void OnExportJson(string path, object item)
        {
            try
            {
                var json = JsonSerializer?.Serialize(item);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    FileIO?.WriteUtf8Text(path, json);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Export item.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="item">The item to export.</param>
        /// <param name="writer">The file writer.</param>
        public void OnExport(string path, object item, IFileWriter writer)
        {
            try
            {
                writer?.Save(path, item, Project);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Execute code script.
        /// </summary>
        /// <param name="csharp">The script code.</param>
        public void OnExecuteCode(string csharp)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    ScriptRunner?.Execute(csharp);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Execute code script in repl.
        /// </summary>
        /// <param name="path">The script code.</param>
        public void OnExecuteRepl(string csharp)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    _scriptState = ScriptRunner?.Execute(csharp, _scriptState);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Reset previous script repl.
        /// </summary>
        public void OnResetRepl()
        {
            _scriptState = null;
        }

        /// <summary>
        /// Execute code script from file.
        /// </summary>
        /// <param name="path">The code file path.</param>
        public void OnExecuteScript(string path)
        {
            try
            {
                var csharp = FileIO?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    OnExecuteCode(csharp);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Execute code scripts from files.
        /// </summary>
        /// <param name="paths">The code file paths.</param>
        public void OnExecuteScript(string[] paths)
        {
            foreach (var path in paths)
            {
                OnExecuteScript(path);
            }
        }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public void OnUndo()
        {
            try
            {
                if (Project?.History.CanUndo() ?? false)
                {
                    Deselect();
                    Project?.History.Undo();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Redo last action.
        /// </summary>
        public void OnRedo()
        {
            try
            {
                if (Project?.History.CanRedo() ?? false)
                {
                    Deselect();
                    Project?.History.Redo();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Cut selected document, page or shapes.
        /// </summary>
        /// <param name="item">The item to cut.</param>
        public void OnCut(object item)
        {
            if (item is IPageContainer page)
            {
                _pageToCopy = page;
                _documentToCopy = default;
                Project?.RemovePage(page);
                Project?.SetCurrentContainer(Project?.CurrentDocument?.Pages.FirstOrDefault());
            }
            else if (item is IDocumentContainer document)
            {
                _pageToCopy = default;
                _documentToCopy = document;
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditor || item == null)
            {
                if (CanCopy())
                {
                    OnCopy(item);
                    OnDeleteSelected();
                }
            }
        }

        /// <summary>
        /// Copy document, page or shapes to clipboard.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        public void OnCopy(object item)
        {
            if (item is IPageContainer page)
            {
                _pageToCopy = page;
                _documentToCopy = default;
            }
            else if (item is IDocumentContainer document)
            {
                _pageToCopy = default;
                _documentToCopy = document;
            }
            else if (item is ProjectEditor || item == null)
            {
                if (CanCopy())
                {
                    if (Renderers?[0]?.State?.SelectedShape != null)
                    {
                        OnCopyShapes(Enumerable.Repeat(Renderers[0].State.SelectedShape, 1).ToList());
                    }

                    if (Renderers?[0]?.State?.SelectedShapes != null)
                    {
                        OnCopyShapes(Renderers[0].State.SelectedShapes.ToList());
                    }
                }
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        /// <param name="item">The item to paste.</param>
        public async void OnPaste(object item)
        {
            if (Project != null && item is IPageContainer page)
            {
                if (_pageToCopy != null)
                {
                    var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                    if (document != null)
                    {
                        int index = document.Pages.IndexOf(page);
                        var clone = Clone(_pageToCopy);
                        Project.ReplacePage(document, clone, index);
                        Project.SetCurrentContainer(clone);
                    }
                }
            }
            else if (Project != null && item is IDocumentContainer document)
            {
                if (_pageToCopy != null)
                {
                    var clone = Clone(_pageToCopy);
                    Project?.AddPage(document, clone);
                    Project.SetCurrentContainer(clone);
                }
                else if (_documentToCopy != null)
                {
                    int index = Project.Documents.IndexOf(document);
                    var clone = Clone(_documentToCopy);
                    Project.ReplaceDocument(clone, index);
                    Project.SetCurrentDocument(clone);
                    Project.SetCurrentContainer(clone?.Pages.FirstOrDefault());
                }
            }
            else if (item is ProjectEditor || item == null)
            {
                if (await CanPaste())
                {
                    var text = await (TextClipboard?.GetText() ?? Task.FromResult(string.Empty));
                    if (!string.IsNullOrEmpty(text))
                    {
                        OnTryPaste(text);
                    }
                }
            }
            else if (item is string text)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    OnTryPaste(text);
                }
            }
        }

        /// <summary>
        /// Delete selected document, page, layer or shapes.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void OnDelete(object item)
        {
            if (item is ILayerContainer layer)
            {
                Project?.RemoveLayer(layer);

                var selected = Project?.CurrentContainer?.Layers.FirstOrDefault();
                if (layer.Owner is IPageContainer owner)
                {
                    owner.SetCurrentLayer(selected);
                }
            }
            if (item is IPageContainer page)
            {
                Project?.RemovePage(page);

                var selected = Project?.CurrentDocument?.Pages.FirstOrDefault();
                Project?.SetCurrentContainer(selected);
            }
            else if (item is IDocumentContainer document)
            {
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditor || item == null)
            {
                OnDeleteSelected();
            }
        }

        /// <summary>
        /// Select all shapes.
        /// </summary>
        public void OnSelectAll()
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                Select(
                    Project?.CurrentContainer?.CurrentLayer,
                    ImmutableHashSet.CreateRange<IBaseShape>(Project?.CurrentContainer?.CurrentLayer?.Shapes));
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        public void OnDeselectAll()
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        public void OnClearAll()
        {
            try
            {
                var container = Project?.CurrentContainer;
                if (container != null)
                {
                    foreach (var layer in container.Layers)
                    {
                        Project?.ClearLayer(layer);
                    }

                    container.WorkingLayer.Shapes = ImmutableArray.Create<IBaseShape>();
                    container.HelperLayer.Shapes = ImmutableArray.Create<IBaseShape>();

                    Project.CurrentContainer.Invalidate();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Cancel current action.
        /// </summary>
        public void OnCancel()
        {
            OnDeselectAll();
            OnResetTool();
        }

        /// <summary>
        /// Group selected shapes.
        /// </summary>
        public void OnGroupSelected()
        {
            var group = Group(Renderers?[0]?.State?.SelectedShapes, Constants.DefaulGroupName);
            if (group != null)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, group);
            }
        }

        /// <summary>
        /// Ungroup selected shapes.
        /// </summary>
        public void OnUngroupSelected()
        {
            var result = Ungroup(Renderers?[0]?.State?.SelectedShape, Renderers?[0]?.State?.SelectedShapes);
            if (result == true && Renderers?[0]?.State != null)
            {
                Renderers[0].State.SelectedShape = null;
                Renderers[0].State.SelectedShapes = null;
            }
        }

        /// <summary>
        /// Bring selected shapes to the top of the stack.
        /// </summary>
        public void OnBringToFrontSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                BringToFront(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringToFront(s);
                }
            }
        }

        /// <summary>
        /// Bring selected shapes one step closer to the front of the stack.
        /// </summary>
        public void OnBringForwardSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                BringForward(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources)
                {
                    BringForward(s);
                }
            }
        }

        /// <summary>
        /// Bring selected shapes one step down within the stack.
        /// </summary>
        public void OnSendBackwardSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                SendBackward(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendBackward(s);
                }
            }
        }

        /// <summary>
        /// Bring selected shapes to the bottom of the stack.
        /// </summary>
        public void OnSendToBackSelected()
        {
            var source = Renderers?[0]?.State?.SelectedShape;
            if (source != null)
            {
                SendToBack(source);
            }

            var sources = Renderers?[0]?.State?.SelectedShapes;
            if (sources != null)
            {
                foreach (var s in sources.Reverse())
                {
                    SendToBack(s);
                }
            }
        }

        /// <summary>
        /// Move selected shapes up.
        /// </summary>
        public void OnMoveUpSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                0.0,
                Project.Options.SnapToGrid ? -Project.Options.SnapY : -1.0);
        }

        /// <summary>
        /// Move selected shapes down.
        /// </summary>
        public void OnMoveDownSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                0.0,
                Project.Options.SnapToGrid ? Project.Options.SnapY : 1.0);
        }

        /// <summary>
        /// Move selected shapes left.
        /// </summary>
        public void OnMoveLeftSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                Project.Options.SnapToGrid ? -Project.Options.SnapX : -1.0,
                0.0);
        }

        /// <summary>
        /// Move selected shapes right.
        /// </summary>
        public void OnMoveRightSelected()
        {
            MoveBy(
                Renderers?[0]?.State?.SelectedShape,
                Renderers?[0]?.State?.SelectedShapes,
                Project.Options.SnapToGrid ? Project.Options.SnapX : 1.0,
                0.0);
        }

        /// <summary>
        /// Set current tool to <see cref="ToolNone"/>.
        /// </summary>
        public void OnToolNone()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "None");
        }

        /// <summary>
        /// Set current tool to <see cref="ToolSelection"/>.
        /// </summary>
        public void OnToolSelection()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Selection");
        }

        /// <summary>
        /// Set current tool to <see cref="ToolPoint"/>.
        /// </summary>
        public void OnToolPoint()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Point");
        }

        /// <summary>
        /// Set current tool to <see cref="ToolLine"/> or current path tool to <see cref="PathToolLine"/>.
        /// </summary>
        public void OnToolLine()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Line")
            {
                if (Tools.FirstOrDefault(t => t.Title == "Path") is ToolPath pathTool)
                {
                    pathTool.RemoveLastSegment();
                }
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Line");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Line");
            }
        }

        /// <summary>
        /// Set current tool to <see cref="ToolArc"/> or current path tool to <see cref="PathToolArc"/>.
        /// </summary>
        public void OnToolArc()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Arc")
            {
                if (Tools.FirstOrDefault(t => t.Title == "Path") is ToolPath pathTool)
                {
                    pathTool.RemoveLastSegment();
                }
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Arc");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Arc");
            }
        }

        /// <summary>
        /// Set current tool to <see cref="ToolCubicBezier"/> or current path tool to <see cref="PathToolCubicBezier"/>.
        /// </summary>
        public void OnToolCubicBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "CubicBezier")
            {
                if (Tools.FirstOrDefault(t => t.Title == "Path") is ToolPath pathTool)
                {
                    pathTool.RemoveLastSegment();
                }
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
        }

        /// <summary>
        /// Set current tool to <see cref="ToolQuadraticBezier"/> or current path tool to <see cref="PathToolQuadraticBezier"/>.
        /// </summary>
        public void OnToolQuadraticBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "QuadraticBezier")
            {
                if (Tools.FirstOrDefault(t => t.Title == "Path") is ToolPath pathTool)
                {
                    pathTool.RemoveLastSegment();
                }
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
            else
            {
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
        }

        /// <summary>
        /// Set current tool to <see cref="ToolPath"/>.
        /// </summary>
        public void OnToolPath()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Path");
        }

        /// <summary>
        /// Set current tool to <see cref="ToolRectangle"/>.
        /// </summary>
        public void OnToolRectangle()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Rectangle");
        }

        /// <summary>
        /// Set current tool to <see cref="ToolEllipse"/>.
        /// </summary>
        public void OnToolEllipse()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Ellipse");
        }

        /// <summary>
        /// Set current tool to <see cref="ToolText"/>.
        /// </summary>
        public void OnToolText()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Text");
        }

        /// <summary>
        /// Set current tool to <see cref="ToolImage"/>.
        /// </summary>
        public void OnToolImage()
        {
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Image");
        }

        /// <summary>
        /// Set current path tool to <see cref="PathToolMove"/>.
        /// </summary>
        public void OnToolMove()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Move")
            {
                var tool = Tools.FirstOrDefault(t => t.Title == "Path");
                if (tool != null)
                {
                    var pathTool = tool as ToolPath;
                    pathTool.RemoveLastSegment();
                }
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Move");
            }
        }

        /// <summary>
        /// Reset current tool.
        /// </summary>
        public void OnResetTool()
        {
            CurrentTool?.Reset();
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsStroked"/> option.
        /// </summary>
        public void OnToggleDefaultIsStroked()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsStroked = !Project.Options.DefaultIsStroked;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsFilled"/> option.
        /// </summary>
        public void OnToggleDefaultIsFilled()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsFilled = !Project.Options.DefaultIsFilled;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsClosed"/> option.
        /// </summary>
        public void OnToggleDefaultIsClosed()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsClosed = !Project.Options.DefaultIsClosed;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public void OnToggleDefaultIsSmoothJoin()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsSmoothJoin = !Project.Options.DefaultIsSmoothJoin;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.SnapToGrid"/> option.
        /// </summary>
        public void OnToggleSnapToGrid()
        {
            if (Project?.Options != null)
            {
                Project.Options.SnapToGrid = !Project.Options.SnapToGrid;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.TryToConnect"/> option.
        /// </summary>
        public void OnToggleTryToConnect()
        {
            if (Project?.Options != null)
            {
                Project.Options.TryToConnect = !Project.Options.TryToConnect;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.CloneStyle"/> option.
        /// </summary>
        public void OnToggleCloneStyle()
        {
            if (Project?.Options != null)
            {
                Project.Options.CloneStyle = !Project.Options.CloneStyle;
            }
        }

        /// <summary>
        /// Add database.
        /// </summary>
        public void OnAddDatabase()
        {
            var db = Factory.CreateDatabase(Constants.DefaultDatabaseName);
            Project.AddDatabase(db);
            Project.SetCurrentDatabase(db);
        }

        /// <summary>
        /// Remove database.
        /// </summary>
        /// <param name="db">The database to remove.</param>
        public void OnRemoveDatabase(IDatabase db)
        {
            Project.RemoveDatabase(db);
            Project.SetCurrentDatabase(Project.Databases.FirstOrDefault());
        }

        /// <summary>
        /// Add column to database.
        /// </summary>
        /// <param name="db">The records database.</param>
        public void OnAddColumn(IDatabase db)
        {
            Project.AddColumn(db, Factory.CreateColumn(db, Constants.DefaulColumnName));
        }

        /// <summary>
        /// Remove column from database.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        public void OnRemoveColumn(IColumn column)
        {
            Project.RemoveColumn(column);
        }

        /// <summary>
        /// Add record to database.
        /// </summary>
        /// <param name="db">The records database.</param>
        public void OnAddRecord(IDatabase db)
        {
            Project.AddRecord(db, Factory.CreateRecord(db, Constants.DefaulValue));
        }

        /// <summary>
        /// Remove record from database.
        /// </summary>
        /// <param name="record">The data record.</param>
        public void OnRemoveRecord(IRecord record)
        {
            Project.RemoveRecord(record);
        }

        /// <summary>
        /// Reset data context record.
        /// </summary>
        /// <param name="data">The data context.</param>
        public void OnResetRecord(IContext data)
        {
            Project.ResetRecord(data);
        }

        /// <summary>
        /// Set current record as selected shape(s) or current page data record.
        /// </summary>
        /// <param name="record">The data record.</param>
        public void OnApplyRecord(IRecord record)
        {
            if (record != null)
            {
                // Selected shape.
                if (Renderers?[0]?.State?.SelectedShape != null)
                {
                    Project?.ApplyRecord(Renderers[0].State.SelectedShape?.Data, record);
                }

                // Selected shapes.
                if (Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in Renderers[0].State.SelectedShapes)
                    {
                        Project.ApplyRecord(shape.Data, record);
                    }
                }

                // Current page.
                if (Renderers[0].State.SelectedShape == null && Renderers[0].State.SelectedShapes == null)
                {
                    var container = Project?.CurrentContainer;
                    if (container != null)
                    {
                        Project?.ApplyRecord(container.Data, record);
                    }
                }
            }
        }

        /// <summary>
        /// Add property to data context.
        /// </summary>
        /// <param name="data">The data context.</param>
        public void OnAddProperty(IContext data)
        {
            Project.AddProperty(data, Factory.CreateProperty(data, Constants.DefaulPropertyName, Constants.DefaulValue));
        }

        /// <summary>
        /// Remove property from data context.
        /// </summary>
        /// <param name="property">The property to remove.</param>
        public void OnRemoveProperty(IProperty property)
        {
            Project.RemoveProperty(property);
        }

        /// <summary>
        /// Add group library.
        /// </summary>
        public void OnAddGroupLibrary()
        {
            var gl = Factory.CreateLibrary<IGroupShape>(Constants.DefaulGroupLibraryName);
            Project.AddGroupLibrary(gl);
            Project.SetCurrentGroupLibrary(gl);
        }

        /// <summary>
        /// Remove group library.
        /// </summary>
        /// <param name="library">The group library to remove.</param>
        public void OnRemoveGroupLibrary(ILibrary<IGroupShape> library)
        {
            Project.RemoveGroupLibrary(library);
            Project.SetCurrentGroupLibrary(Project?.GroupLibraries.FirstOrDefault());
        }

        /// <summary>
        /// Add group.
        /// </summary>
        /// <param name="library">The group library.</param>
        public void OnAddGroup(ILibrary<IGroupShape> library)
        {
            if (Project != null && library != null)
            {
                if (Renderers?[0]?.State?.SelectedShape is IGroupShape group)
                {
                    var clone = CloneShape(group);
                    if (clone != null)
                    {
                        Project?.AddGroup(library, clone);
                    }
                }
            }
        }

        /// <summary>
        /// Remove group.
        /// </summary>
        /// <param name="group">The group item.</param>
        public void OnRemoveGroup(IGroupShape group)
        {
            if (Project != null && group != null)
            {
                var library = Project.RemoveGroup(group);
                library?.SetSelected(library?.Items.FirstOrDefault());
            }
        }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        /// <param name="group">The group instance.</param>
        public void OnInsertGroup(IGroupShape group)
        {
            if (Project?.CurrentContainer != null)
            {
                OnDropShapeAsClone(group, 0.0, 0.0);
            }
        }

        /// <summary>
        /// Add layer to container.
        /// </summary>
        /// <param name="container">The container instance.</param>
        public void OnAddLayer(IPageContainer container)
        {
            Project.AddLayer(container, Factory.CreateLayerContainer(Constants.DefaultLayerName, container));
        }

        /// <summary>
        /// Remove layer.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        public void OnRemoveLayer(ILayerContainer layer)
        {
            Project.RemoveLayer(layer);
            if (layer.Owner is IPageContainer owner)
            {
                owner.SetCurrentLayer(owner.Layers.FirstOrDefault());
            }
        }

        /// <summary>
        /// Add style library.
        /// </summary>
        public void OnAddStyleLibrary()
        {
            var sl = Factory.CreateLibrary<IShapeStyle>(Constants.DefaulStyleLibraryName);
            Project.AddStyleLibrary(sl);
            Project.SetCurrentStyleLibrary(sl);
        }

        /// <summary>
        /// Remove style library.
        /// </summary>
        /// <param name="library">The style library to remove.</param>
        public void OnRemoveStyleLibrary(ILibrary<IShapeStyle> library)
        {
            Project.RemoveStyleLibrary(library);
            Project.SetCurrentStyleLibrary(Project?.StyleLibraries.FirstOrDefault());
        }

        /// <summary>
        /// Add style.
        /// </summary>
        /// <param name="library">The style library.</param>
        public void OnAddStyle(ILibrary<IShapeStyle> library)
        {
            Project.AddStyle(library, Factory.CreateShapeStyle(Constants.DefaulStyleName));
        }

        /// <summary>
        /// Remove style.
        /// </summary>
        /// <param name="style">The style to remove.</param>
        public void OnRemoveStyle(IShapeStyle style)
        {
            var library = Project.RemoveStyle(style);
            library?.SetSelected(library?.Items.FirstOrDefault());
        }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        /// <param name="style">The shape style item.</param>
        public void OnApplyStyle(IShapeStyle style)
        {
            if (style != null)
            {
                // Selected shape.
                if (Renderers[0]?.State?.SelectedShape != null)
                {
                    Project?.ApplyStyle(Renderers[0].State.SelectedShape, style);
                }

                // Selected shapes.
                if (Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in Renderers[0].State.SelectedShapes)
                    {
                        Project?.ApplyStyle(shape, style);
                    }
                }
            }
        }

        /// <summary>
        /// Set current data as selected shape data.
        /// </summary>
        /// <param name="data">The data item.</param>
        public void OnApplyData(IContext data)
        {
            if (data != null)
            {
                // Selected shape.
                if (Renderers?[0]?.State?.SelectedShape != null)
                {
                    Project?.ApplyData(Renderers[0].State.SelectedShape, data);
                }

                // Selected shapes.
                if (Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var shape in Renderers[0].State.SelectedShapes)
                    {
                        Project?.ApplyData(shape, data);
                    }
                }
            }
        }

        /// <summary>
        /// Add shape.
        /// </summary>
        /// <param name="shape">The shape instance.</param>
        public void OnAddShape(IBaseShape shape)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null && shape != null)
            {
                Project.AddShape(layer, shape);
            }
        }

        /// <summary>
        /// Remove shape.
        /// </summary>
        /// <param name="shape">The shape instance.</param>
        public void OnRemoveShape(IBaseShape shape)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null && shape != null)
            {
                Project.RemoveShape(layer, shape);
                Project.CurrentContainer.CurrentShape = layer.Shapes.FirstOrDefault();
            }
        }

        /// <summary>
        /// Add template.
        /// </summary>
        public void OnAddTemplate()
        {
            if (Project != null)
            {
                var template = ContainerFactory.GetTemplate(Project, "Empty");
                if (template == null)
                {
                    template = Factory.CreateTemplateContainer(Constants.DefaultTemplateName);
                }

                Project.AddTemplate(template);
            }
        }

        /// <summary>
        /// Remove template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnRemoveTemplate(IPageContainer template)
        {
            if (template != null)
            {
                Project?.RemoveTemplate(template);
                Project?.SetCurrentTemplate(Project?.Templates.FirstOrDefault());
            }
        }

        /// <summary>
        /// Edit template.
        /// </summary>
        public void OnEditTemplate(IPageContainer template)
        {
            if (Project != null && template != null)
            {
                Project.SetCurrentContainer(template);
                Project.CurrentContainer?.Invalidate();
            }
        }

        /// <summary>
        /// Set page template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnApplyTemplate(IPageContainer template)
        {
            var page = Project?.CurrentContainer;
            if (page != null && template != null && page != template)
            {
                Project.ApplyTemplate(page, template);
                Project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// Add image key.
        /// </summary>
        /// <param name="path">The image path.</param>
        public async Task<string> OnAddImageKey(string path)
        {
            if (Project != null)
            {
                if (path == null || string.IsNullOrEmpty(path))
                {
                    var key = await (ImageImporter.GetImageKeyAsync() ?? Task.FromResult(string.Empty));
                    if (key == null || string.IsNullOrEmpty(key))
                        return null;

                    return key;
                }
                else
                {
                    byte[] bytes;
                    using (var stream = FileIO?.Open(path))
                    {
                        bytes = FileIO?.ReadBinary(stream);
                    }
                    if (Project is IImageCache imageCache)
                    {
                        var key = imageCache.AddImageFromFile(path, bytes);
                        return key;
                    }
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Remove image key.
        /// </summary>
        /// <param name="key">The image key.</param>
        public void OnRemoveImageKey(string key)
        {
            if (key != null)
            {
                if (Project is IImageCache imageCache)
                {
                    imageCache.RemoveImage(key);
                }
            }
        }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public void OnSelectedItemChanged(IObservableObject item)
        {
            if (Project != null)
            {
                Project.Selected = item;
            }
        }

        /// <summary>
        /// Add page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddPage(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                var page =
                    ContainerFactory?.GetPage(Project, Constants.DefaultPageName)
                    ?? Factory.CreatePageContainer(Constants.DefaultPageName);

                Project.AddPage(Project.CurrentDocument, page);
                Project.SetCurrentContainer(page);
            }
        }

        /// <summary>
        /// Insert page before current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertPageBefore(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                if (item is IPageContainer selected)
                {
                    int index = Project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        ContainerFactory?.GetPage(Project, Constants.DefaultPageName)
                        ?? Factory.CreatePageContainer(Constants.DefaultPageName);

                    Project.AddPageAt(Project.CurrentDocument, page, index);
                    Project.SetCurrentContainer(page);
                }
            }
        }

        /// <summary>
        /// Insert page after current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertPageAfter(object item)
        {
            if (Project?.CurrentDocument != null)
            {
                if (item is IPageContainer selected)
                {
                    int index = Project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        ContainerFactory?.GetPage(Project, Constants.DefaultPageName)
                        ?? Factory.CreatePageContainer(Constants.DefaultPageName);

                    Project.AddPageAt(Project.CurrentDocument, page, index + 1);
                    Project.SetCurrentContainer(page);
                }
            }
        }

        /// <summary>
        /// Add document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddDocument(object item)
        {
            if (Project != null)
            {
                var document =
                    ContainerFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                    ?? Factory.CreateDocumentContainer(Constants.DefaultDocumentName);

                Project.AddDocument(document);
                Project.SetCurrentDocument(document);
                Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
            }
        }

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentBefore(object item)
        {
            if (Project != null)
            {
                if (item is IDocumentContainer selected)
                {
                    int index = Project.Documents.IndexOf(selected);
                    var document =
                        ContainerFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                        ?? Factory.CreateDocumentContainer(Constants.DefaultDocumentName);

                    Project.AddDocumentAt(document, index);
                    Project.SetCurrentDocument(document);
                    Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
                }
            }
        }

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentAfter(object item)
        {
            if (Project != null)
            {
                if (item is IDocumentContainer selected)
                {
                    int index = Project.Documents.IndexOf(selected);
                    var document =
                        ContainerFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                        ?? Factory.CreateDocumentContainer(Constants.DefaultDocumentName);

                    Project.AddDocumentAt(document, index + 1);
                    Project.SetCurrentDocument(document);
                    Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
                }
            }
        }

        /// <summary>
        /// Set renderer's image cache.
        /// </summary>
        /// <param name="cache">The image cache instance.</param>
        private void SetRenderersImageCache(IImageCache cache)
        {
            if (Renderers != null)
            {
                foreach (var renderer in Renderers)
                {
                    renderer.ClearCache(isZooming: false);
                    renderer.State.ImageCache = cache;
                }
            }
        }

        /// <summary>
        /// Load project.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="path">The project path.</param>
        public void OnLoad(IProjectContainer project, string path = null)
        {
            if (project != null)
            {
                Deselect();
                if (project is IImageCache imageCache)
                {
                    SetRenderersImageCache(imageCache);
                }
                Project = project;
                Project.History = new StackHistory();
                ProjectPath = path;
                IsProjectDirty = false;
                Observer = new ProjectObserver(this);
            }
        }

        /// <summary>
        /// Unload project.
        /// </summary>
        public void OnUnload()
        {
            if (Observer != null)
            {
                Observer?.Dispose();
                Observer = null;
            }

            if (Project?.History != null)
            {
                Project.History.Reset();
                Project.History = null;
            }

            if (Project != null)
            {
                if (Project is IImageCache imageCache)
                {
                    imageCache.PurgeUnusedImages(Enumerable.Empty<string>().ToImmutableHashSet());
                }
                Deselect();
                SetRenderersImageCache(null);
                Project = null;
                ProjectPath = string.Empty;
                IsProjectDirty = false;
                GC.Collect();
            }
        }

        /// <summary>
        /// Invalidate renderer's cache.
        /// </summary>
        /// <param name="isZooming">The flag indicating whether is zooming.</param>
        public void OnInvalidateCache(bool isZooming)
        {
            try
            {
                if (Renderers != null)
                {
                    foreach (var renderer in Renderers)
                    {
                        renderer.ClearCache(isZooming);
                    }
                }

                Project?.CurrentContainer?.Invalidate();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Add recent project file.
        /// </summary>
        /// <param name="path">The project path.</param>
        /// <param name="name">The project name.</param>
        public void OnAddRecent(string path, string name)
        {
            if (_recentProjects != null)
            {
                var q = _recentProjects.Where(x => x.Path.ToLower() == path.ToLower()).ToList();
                var builder = _recentProjects.ToBuilder();

                if (q.Count() > 0)
                {
                    foreach (var r in q)
                    {
                        builder.Remove(r);
                    }
                }

                builder.Insert(0, RecentFile.Create(name, path));

                RecentProjects = builder.ToImmutable();
                CurrentRecentProject = _recentProjects.FirstOrDefault();
            }
        }

        /// <summary>
        /// Load recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void OnLoadRecent(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var json = FileIO.ReadUtf8Text(path);
                    var recent = JsonSerializer.Deserialize<Recents>(json);
                    if (recent != null)
                    {
                        var remove = recent.Files.Where(x => FileIO?.Exists(x.Path) == false).ToList();
                        var builder = recent.Files.ToBuilder();

                        foreach (var file in remove)
                        {
                            builder.Remove(file);
                        }

                        RecentProjects = builder.ToImmutable();

                        if (recent.Current != null
                            && (FileIO?.Exists(recent.Current.Path) ?? false))
                        {
                            CurrentRecentProject = recent.Current;
                        }
                        else
                        {
                            CurrentRecentProject = _recentProjects.FirstOrDefault();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
            }
        }

        /// <summary>
        /// Save recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void OnSaveRecent(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var recent = Recents.Create(_recentProjects, _currentRecentProject);
                    var json = JsonSerializer.Serialize(recent);
                    FileIO.WriteUtf8Text(path, json);
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
            }
        }

        /// <summary>
        /// Load layout configuration.
        /// </summary>
        /// <param name="path">The layout configuration path.</param>
        public void OnLoadLayout(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var json = FileIO.ReadUtf8Text(path);
                    var layout = JsonSerializer.Deserialize<RootDock>(json);
                    if (layout != null)
                    {
                        Layout = layout;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
            }
        }

        /// <summary>
        /// Save layout configuration.
        /// </summary>
        /// <param name="path">The layout configuration path.</param>
        public void OnSaveLayout(string path)
        {
            if (JsonSerializer != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(_layout);
                    FileIO.WriteUtf8Text(path, json);
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
            }
        }

        /// <summary>
        /// Navigate to view.
        /// </summary>
        /// <param name="view">The view to navigate to.</param>
        public void OnNavigate(object view)
        {
            if (Layout is IDock dock)
            {
                dock.Navigate(view);
            }
        }

        /// <summary>
        /// Check if undo action is available.
        /// </summary>
        /// <returns>Returns true if undo action is available.</returns>
        public bool CanUndo()
        {
            return Project?.History?.CanUndo() ?? false;
        }

        /// <summary>
        /// Check if redo action is available.
        /// </summary>
        /// <returns>Returns true if redo action is available.</returns>
        public bool CanRedo()
        {
            return Project?.History?.CanRedo() ?? false;
        }

        /// <summary>
        /// Checks if can copy.
        /// </summary>
        /// <returns>Returns true if can copy.</returns>
        public bool CanCopy()
        {
            return IsSelectionAvailable();
        }

        /// <summary>
        /// Checks if can paste.
        /// </summary>
        /// <returns>Returns true if can paste.</returns>
        public async Task<bool> CanPaste()
        {
            try
            {
                return await (TextClipboard?.ContainsText() ?? Task.FromResult(false));
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Copy selected shapes to clipboard.
        /// </summary>
        /// <param name="shapes"></param>
        public void OnCopyShapes(IList<IBaseShape> shapes)
        {
            try
            {
                var json = JsonSerializer?.Serialize(shapes);
                if (!string.IsNullOrEmpty(json))
                {
                    TextClipboard?.SetText(json);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Paste text from clipboard as shapes.
        /// </summary>
        /// <param name="text">The text string.</param>
        public void OnTryPaste(string text)
        {
            try
            {
                var exception = default(Exception);

                // Try to deserialize Xaml.
                try
                {
                    if (XamlSerializer != null)
                    {
                        OnImportXamlString(text);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                // Try to deserialize Json.
                try
                {
                    var shapes = JsonSerializer?.Deserialize<IList<IBaseShape>>(text);
                    if (shapes?.Count() > 0)
                    {
                        OnPasteShapes(shapes);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                throw exception;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void ResetPointShapeToDefault(IEnumerable<IBaseShape> shapes)
        {
            foreach (var point in shapes?.SelectMany(s => s?.GetPoints()))
            {
                point.Shape = Project?.Options?.PointShape;
            }
        }

        private IDictionary<string, IShapeStyle> GenerateStyleDictionaryByName()
        {
            return Project?.StyleLibraries
                .Where(sl => sl?.Items != null && sl?.Items.Length > 0)
                .SelectMany(sl => sl.Items)
                .Distinct(new ShapeStyleByNameComparer())
                .ToDictionary(s => s.Name);
        }

        /// <summary>
        /// Try to restore shape styles.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreStyles(IEnumerable<IBaseShape> shapes)
        {
            try
            {
                if (Project?.StyleLibraries == null)
                    return;

                var styles = GenerateStyleDictionaryByName();

                // Reset point shape to defaults.
                ResetPointShapeToDefault(shapes);

                // Try to restore shape styles.
                foreach (var shape in ProjectContainer.GetAllShapes(shapes))
                {
                    if (shape?.Style == null)
                        continue;

                    if (!string.IsNullOrWhiteSpace(shape.Style.Name))
                    {
                        if (styles.TryGetValue(shape.Style.Name, out var style))
                        {
                            // Use existing style.
                            shape.Style = style;
                        }
                        else
                        {
                            // Create Imported style library.
                            if (Project?.CurrentStyleLibrary == null)
                            {
                                var sl = Factory.CreateLibrary<IShapeStyle>(Constants.ImportedStyleLibraryName);
                                Project.AddStyleLibrary(sl);
                                Project.SetCurrentStyleLibrary(sl);
                            }

                            // Add missing style.
                            Project?.AddStyle(Project?.CurrentStyleLibrary, shape.Style);

                            // Recreate styles dictionary.
                            styles = GenerateStyleDictionaryByName();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private IDictionary<string, IRecord> GenerateRecordDictionaryById()
        {
            return Project?.Databases
                .Where(d => d?.Records != null && d?.Records.Length > 0)
                .SelectMany(d => d.Records)
                .ToDictionary(s => s.Id);
        }

        /// <summary>
        /// Try to restore shape records.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreRecords(IEnumerable<IBaseShape> shapes)
        {
            try
            {
                if (Project?.Databases == null)
                    return;

                var records = GenerateRecordDictionaryById();

                // Try to restore shape record.
                foreach (var shape in ProjectContainer.GetAllShapes(shapes))
                {
                    if (shape?.Data?.Record == null)
                        continue;

                    if (records.TryGetValue((string)shape.Data.Record.Id, out var record))
                    {
                        // Use existing record.
                        shape.Data.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (Project?.CurrentDatabase == null && shape.Data.Record.Owner is IDatabase owner)
                        {
                            var db = Factory.CreateDatabase(
                                Constants.ImportedDatabaseName,
                                (ImmutableArray<IColumn>)owner.Columns);
                            Project.AddDatabase(db);
                            Project.SetCurrentDatabase(db);
                        }

                        // Add missing data record.
                        shape.Data.Record.Owner = Project.CurrentDatabase;
                        Project?.AddRecord(Project?.CurrentDatabase, (IRecord)shape.Data.Record);

                        // Recreate records dictionary.
                        records = GenerateRecordDictionaryById();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Restore shape project references afer closing.
        /// </summary>
        /// <param name="shape">The shape to restore.</param>
        private void RestoreShape(IBaseShape shape)
        {
            var shapes = Enumerable.Repeat(shape, 1).ToList();
            TryToRestoreStyles(shapes);
            TryToRestoreRecords(shapes);
        }

        /// <summary>
        /// Paste shapes to current container.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        public void OnPasteShapes(IEnumerable<IBaseShape> shapes)
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);

                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);

                Project.AddShapes(Project?.CurrentContainer?.CurrentLayer, shapes);

                OnSelect(shapes);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        public void OnSelect(IEnumerable<IBaseShape> shapes)
        {
            if (shapes?.Count() == 1)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, shapes.FirstOrDefault());
            }
            else
            {
                Select(Project?.CurrentContainer?.CurrentLayer, ImmutableHashSet.CreateRange<IBaseShape>(shapes));
            }
        }

        /// <summary>
        /// Clone the <see cref="BaseShape"/> object.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="IBaseShape"/> object.</param>
        /// <returns>The cloned <see cref="IBaseShape"/> object.</returns>
        public T CloneShape<T>(T shape) where T : IBaseShape
        {
            try
            {
                if (JsonSerializer is IJsonSerializer serializer)
                {
                    var json = serializer.Serialize(shape);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var clone = serializer.Deserialize<T>(json);
                        if (clone != null)
                        {
                            RestoreShape(clone);
                            return clone;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return default;
        }

        /// <summary>
        /// Clone the <see cref="LayerContainer"/> object.
        /// </summary>
        /// <param name="container">The <see cref="LayerContainer"/> object.</param>
        /// <returns>The cloned <see cref="LayerContainer"/> object.</returns>
        public ILayerContainer Clone(ILayerContainer container)
        {
            try
            {
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<ILayerContainer>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Shapes;
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return default;
        }

        /// <summary>
        /// Clone the <see cref="PageContainer"/> object.
        /// </summary>
        /// <param name="container">The <see cref="PageContainer"/> object.</param>
        /// <returns>The cloned <see cref="PageContainer"/> object.</returns>
        public IPageContainer Clone(IPageContainer container)
        {
            try
            {
                var template = container?.Template;
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<IPageContainer>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Layers.SelectMany(l => l.Shapes);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        clone.Template = template;
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return default;
        }

        /// <summary>
        /// Clone the <see cref="DocumentContainer"/> object.
        /// </summary>
        /// <param name="document">The <see cref="DocumentContainer"/> object.</param>
        /// <returns>The cloned <see cref="DocumentContainer"/> object.</returns>
        public IDocumentContainer Clone(IDocumentContainer document)
        {
            try
            {
                var templates = document?.Pages.Select(c => c?.Template)?.ToArray();
                var json = JsonSerializer?.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<IDocumentContainer>(json);
                    if (clone != null)
                    {
                        for (int i = 0; i < clone.Pages.Length; i++)
                        {
                            var container = clone.Pages[i];
                            var shapes = container.Layers.SelectMany(l => l.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            container.Template = templates[i];
                        }
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return default;
        }

        /// <summary>
        /// Process dropped files.
        /// </summary>
        /// <param name="files">The files array.</param>
        /// <returns>Returns true if success.</returns>
        public bool OnDropFiles(string[] files)
        {
            try
            {
                if (files?.Length >= 1)
                {
                    bool result = false;
                    foreach (var path in files)
                    {
                        if (string.IsNullOrEmpty(path))
                            continue;

                        string ext = System.IO.Path.GetExtension(path);

                        if (string.Compare(ext, Constants.ProjectExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnOpenProject(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.CsvExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportData(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.JsonExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportJson(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.XamlExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnImportXaml(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.ScriptExtension, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            OnExecuteScript(path);
                            result = true;
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }

        /// <summary>
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <param name="shape">The <see cref="IBaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        public bool OnDropShape(IBaseShape shape, double x, double y, bool bExecute = true)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null)
                {
                    var target = Renderers[0].State.SelectedShape;
                    if (target is IPointShape point)
                    {
                        if (bExecute == true)
                        {
                            point.Shape = shape;
                        }
                        return true;
                    }
                }
                else if (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var target in Renderers[0].State.SelectedShapes)
                    {
                        if (target is IPointShape point)
                        {
                            if (bExecute == true)
                            {
                                point.Shape = shape;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    var layer = Project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var point = HitTest.TryToGetPoint(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                        if (point != null)
                        {
                            if (bExecute == true)
                            {
                                point.Shape = shape;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (bExecute == true)
                            {
                                OnDropShapeAsClone(shape, x, y);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void OnDropShapeAsClone<T>(T shape, double x, double y) where T : IBaseShape
        {
            double sx = Project.Options.SnapToGrid ? Snap(x, Project.Options.SnapX) : x;
            double sy = Project.Options.SnapToGrid ? Snap(y, Project.Options.SnapY) : y;

            try
            {
                var clone = CloneShape(shape);
                if (clone != null)
                {
                    Deselect(Project?.CurrentContainer?.CurrentLayer);
                    clone.Move(null, sx, sy);

                    Project.AddShape(Project?.CurrentContainer?.CurrentLayer, clone);

                    Select(Project?.CurrentContainer?.CurrentLayer, clone);

                    if (Project.Options.TryToConnect)
                    {
                        if (clone is IGroupShape group)
                        {
                            TryToConnectLines(
                                ProjectContainer.GetAllShapes<ILineShape>(Project?.CurrentContainer?.CurrentLayer?.Shapes),
                                group.Connectors,
                                Project.Options.HitThreshold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        /// <summary>
        /// Drop <see cref="IRecord"/> object in current container at specified location.
        /// </summary>
        /// <param name="record">The <see cref="IRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        public bool OnDropRecord(IRecord record, double x, double y, bool bExecute = true)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null
                    || (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    if (bExecute)
                    {
                        OnApplyRecord(record);
                    }
                    return true;
                }
                else
                {
                    var layer = Project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = HitTest.TryToGetShape(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                        if (result != null)
                        {
                            if (bExecute)
                            {
                                Project?.ApplyRecord(result.Data, record);
                            }
                            return true;
                        }
                        else
                        {
                            if (bExecute)
                            {
                                OnDropRecordAsGroup(record, x, y);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Drop <see cref="IRecord"/> object in current container at specified location as group bound to this record.
        /// </summary>
        /// <param name="record">The <see cref="IRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void OnDropRecordAsGroup(IRecord record, double x, double y)
        {
            var selected = Project.CurrentStyleLibrary.Selected;
            var style = Project.Options.CloneStyle ? selected.Clone() : selected;
            var point = Project?.Options?.PointShape;
            var layer = Project?.CurrentContainer?.CurrentLayer;
            double sx = Project.Options.SnapToGrid ? Snap(x, Project.Options.SnapX) : x;
            double sy = Project.Options.SnapToGrid ? Snap(y, Project.Options.SnapY) : y;

            var g = Factory.CreateGroupShape(Constants.DefaulGroupName);

            g.Data.Record = record;

            var length = record.Values.Length;
            double px = sx;
            double py = sy;
            double width = 150;
            double height = 15;

            var db = record.Owner as IDatabase;

            for (int i = 0; i < length; i++)
            {
                var column = db.Columns[i];
                if (column.IsVisible)
                {
                    var binding = "{" + db.Columns[i].Name + "}";
                    var text = Factory.CreateTextShape(px, py, px + width, py + height, style, point, binding);
                    g.AddShape(text);
                    py += height;
                }
            }

            var rectangle = Factory.CreateRectangleShape(sx, sy, sx + width, sy + (length * height), style, point);
            g.AddShape(rectangle);

            var pt = Factory.CreatePointShape(sx + (width / 2), sy, point);
            var pb = Factory.CreatePointShape(sx + (width / 2), sy + (length * height), point);
            var pl = Factory.CreatePointShape(sx, sy + ((length * height) / 2), point);
            var pr = Factory.CreatePointShape(sx + width, sy + ((length * height) / 2), point);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            Project.AddShape(layer, g);
        }

        /// <summary>
        /// Drop <see cref="IShapeStyle"/> object in current container at specified location.
        /// </summary>
        /// <param name="style">The <see cref="IShapeStyle"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        public bool OnDropStyle(IShapeStyle style, double x, double y, bool bExecute = true)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null
                    || (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    if (bExecute == true)
                    {
                        OnApplyStyle(style);
                    }
                    return true;
                }
                else
                {
                    var layer = Project.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = HitTest.TryToGetShape(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                        if (result != null)
                        {
                            if (bExecute == true)
                            {
                                Project.ApplyStyle(result, style);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Drop <see cref="PageContainer"/> object in current container at specified location.
        /// </summary>
        /// <param name="template">The template object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        public bool OnDropTemplate(IPageContainer template, double x, double y, bool bExecute = true)
        {
            try
            {
                var page = Project?.CurrentContainer;
                if (page != null && template != null && page != template)
                {
                    if (bExecute)
                    {
                        OnApplyTemplate(template);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Remove selected shapes.
        /// </summary>
        public void OnDeleteSelected()
        {
            if (Project?.CurrentContainer?.CurrentLayer == null || Renderers?[0]?.State == null)
                return;

            if (Renderers[0].State.SelectedShape != null)
            {
                var layer = Project.CurrentContainer.CurrentLayer;

                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(Renderers[0].State.SelectedShape);
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Renderers[0].State.SelectedShape = default;
                layer.Invalidate();
            }

            if (Renderers[0].State.SelectedShapes != null && Renderers[0].State.SelectedShapes.Count > 0)
            {
                var layer = Project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in Renderers[0].State.SelectedShapes)
                {
                    builder.Remove(shape);
                }

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                Renderers[0].State.SelectedShapes = default;
                layer.Invalidate();
            }
        }

        /// <summary>
        /// Select shape.
        /// </summary>
        /// <param name="shape">The shape to select.</param>
        public void Select(IBaseShape shape)
        {
            if (Renderers?[0]?.State != null)
            {
                Renderers[0].State.SelectedShape = shape;

                if (Renderers[0].State.SelectedShapes != null)
                {
                    Renderers[0].State.SelectedShapes = default;
                }
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(ISet<IBaseShape> shapes)
        {
            if (Renderers?[0]?.State != null)
            {
                if (Renderers[0].State.SelectedShape != null)
                {
                    Renderers[0].State.SelectedShape = default;
                }

                Renderers[0].State.SelectedShapes = shapes;
            }
        }

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        public void Deselect()
        {
            if (Renderers?[0].State?.SelectedShape != null)
            {
                Renderers[0].State.SelectedShape = default;
            }

            if (Renderers?[0].State?.SelectedShapes != null)
            {
                Renderers[0].State.SelectedShapes = default;
            }
        }

        /// <summary>
        /// Select shape.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shape">The shape to select.</param>
        public void Select(ILayerContainer layer, IBaseShape shape)
        {
            Select(shape);

            if (layer.Owner is IPageContainer owner)
            {
                owner.CurrentShape = shape;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                CanvasPlatform?.Invalidate?.Invoke();
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(ILayerContainer layer, ISet<IBaseShape> shapes)
        {
            Select(shapes);

            if (layer.Owner is IPageContainer owner && owner.CurrentShape != null)
            {
                owner.CurrentShape = default;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                CanvasPlatform?.Invalidate?.Invoke();
            }
        }

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        /// <param name="layer">The layer object.</param>
        public void Deselect(ILayerContainer layer)
        {
            Deselect();

            if (layer.Owner is IPageContainer owner && owner.CurrentShape != null)
            {
                owner.CurrentShape = default;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                if (CanvasPlatform?.Invalidate != null)
                {
                    CanvasPlatform?.Invalidate();
                }
            }
        }

        /// <summary>
        /// Try to select shape at specified coordinates.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="x">The X coordinate in layer.</param>
        /// <param name="y">The Y coordinate in layer.</param>
        /// <returns>True if selecting shape was successful.</returns>
        public bool TryToSelectShape(ILayerContainer layer, double x, double y)
        {
            if (layer != null)
            {
                var point = HitTest.TryToGetPoint(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                if (point != null)
                {
                    Select(layer, point);
                    return true;
                }

                var shape = HitTest.TryToGetShape(layer.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                if (shape != null)
                {
                    Select(layer, shape);
                    return true;
                }

                Deselect(layer);
            }

            return false;
        }

        /// <summary>
        /// Try to select shapes inside rectangle.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="rectangle">The selection rectangle.</param>
        /// <returns>True if selecting shapes was successful.</returns>
        public bool TryToSelectShapes(ILayerContainer layer, IRectangleShape rectangle)
        {
            if (layer != null)
            {
                var rect = Rect2.FromPoints(
                    rectangle.TopLeft.X,
                    rectangle.TopLeft.Y,
                    rectangle.BottomRight.X,
                    rectangle.BottomRight.Y);
                var result = HitTest.TryToGetShapes(layer.Shapes, rect, Project.Options.HitThreshold);
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        if (result.Count == 1)
                        {
                            Select(layer, result.FirstOrDefault());
                        }
                        else
                        {
                            Select(layer, result.ToImmutableHashSet());
                        }
                        return true;
                    }
                }

                Deselect(layer);
            }

            return false;
        }

        /// <summary>
        /// Hover shape.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="shape">The shape to hover.</param>
        public void Hover(ILayerContainer layer, IBaseShape shape)
        {
            if (layer != null)
            {
                Select(layer, shape);
                HoveredShape = shape;
            }
        }

        /// <summary>
        /// De-hover shape.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        public void Dehover(ILayerContainer layer)
        {
            if (layer != null && HoveredShape != null)
            {
                HoveredShape = default;
                Deselect(layer);
            }
        }

        /// <summary>
        /// Try to hover shape at specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <returns>True if hovering shape was successful.</returns>
        public bool TryToHoverShape(double x, double y)
        {
            if (Project?.CurrentContainer?.CurrentLayer == null)
                return false;

            if (Renderers?[0]?.State?.SelectedShapes == null
                && !(Renderers?[0]?.State?.SelectedShape != null && HoveredShape != Renderers?[0]?.State?.SelectedShape))
            {
                var point = HitTest.TryToGetPoint(Project.CurrentContainer?.CurrentLayer?.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                if (point != null)
                {
                    Hover(Project.CurrentContainer?.CurrentLayer, point);
                    return true;
                }
                else
                {
                    var shape = HitTest.TryToGetShape(Project.CurrentContainer?.CurrentLayer?.Shapes, new Point2(x, y), Project.Options.HitThreshold);
                    if (shape != null)
                    {
                        Hover(Project.CurrentContainer?.CurrentLayer, shape);
                        return true;
                    }
                    else
                    {
                        if (Renderers[0].State.SelectedShape != null && Renderers[0].State.SelectedShape == HoveredShape)
                        {
                            Dehover(Project.CurrentContainer?.CurrentLayer);
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Try to get connection point at specified location.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>The connected point if success.</returns>
        public IPointShape TryToGetConnectionPoint(double x, double y)
        {
            if (Project.Options.TryToConnect)
            {
                return HitTest.TryToGetPoint(
                    Project.CurrentContainer.CurrentLayer.Shapes,
                    new Point2(x, y),
                    Project.Options.HitThreshold);
            }
            return null;
        }

        private void SwapLineStart(ILineShape line, IPointShape point)
        {
            if (line?.Start != null && point != null)
            {
                var previous = line.Start;
                var next = point;
                Project?.History?.Snapshot(previous, next, (p) => line.Start = p);
                line.Start = next;
            }
        }

        private void SwapLineEnd(ILineShape line, IPointShape point)
        {
            if (line?.End != null && point != null)
            {
                var previous = line.End;
                var next = point;
                Project?.History?.Snapshot(previous, next, (p) => line.End = p);
                line.End = next;
            }
        }

        /// <summary>
        /// Try to split line at specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="point">The point used for split line start or end.</param>
        /// <param name="select">The flag indicating whether to select split line.</param>
        /// <returns>True if line split was successful.</returns>
        public bool TryToSplitLine(double x, double y, IPointShape point, bool select = false)
        {
            if (Project?.CurrentContainer == null || Project?.Options == null)
                return false;

            var result = HitTest.TryToGetShape(
                Project.CurrentContainer.CurrentLayer.Shapes,
                new Point2(x, y),
                Project.Options.HitThreshold);

            if (result is ILineShape line)
            {
                if (!Project.Options.SnapToGrid)
                {
                    var a = new Point2(line.Start.X, line.Start.Y);
                    var b = new Point2(line.End.X, line.End.Y);
                    var target = new Point2(x, y);
                    var nearest = target.NearestOnLine(a, b);
                    point.X = nearest.X;
                    point.Y = nearest.Y;
                }

                var split = Factory.CreateLineShape(
                    x, y,
                    line.Style,
                    Project.Options.PointShape,
                    line.IsStroked);

                double ds = point.DistanceTo(line.Start);
                double de = point.DistanceTo(line.End);

                if (ds < de)
                {
                    split.Start = line.Start;
                    split.End = point;
                    SwapLineStart(line, point);
                }
                else
                {
                    split.Start = point;
                    split.End = line.End;
                    SwapLineEnd(line, point);
                }

                Project.AddShape(Project.CurrentContainer.CurrentLayer, split);

                if (select)
                {
                    Select(Project.CurrentContainer.CurrentLayer, point);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to split lines using group connectors.
        /// </summary>
        /// <param name="line">The line to split.</param>
        /// <param name="p0">The first connector point.</param>
        /// <param name="p1">The second connector point.</param>
        /// <returns>True if line split was successful.</returns>
        public bool TryToSplitLine(ILineShape line, IPointShape p0, IPointShape p1)
        {
            if (Project?.Options == null)
                return false;

            // Points must be aligned horizontally or vertically.
            if (p0.X != p1.X && p0.Y != p1.Y)
                return false;

            // Line must be horizontal or vertical.
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
                return false;

            ILineShape split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = Factory.CreateLineShape(
                    p0,
                    line.End,
                    line.Style,
                    Project.Options.PointShape,
                    line.IsStroked);

                SwapLineEnd(line, p1);
            }
            else
            {
                split = Factory.CreateLineShape(
                    p1,
                    line.End,
                    line.Style,
                    Project.Options.PointShape,
                    line.IsStroked);

                SwapLineEnd(line, p0);
            }

            Project.AddShape(Project.CurrentContainer.CurrentLayer, split);

            return true;
        }

        /// <summary>
        /// Try to connect lines to connectors.
        /// </summary>
        /// <param name="lines">The lines to connect.</param>
        /// <param name="connectors">The connectors array.</param>
        /// <param name="threshold">The connection threshold.</param>
        /// <returns>True if connection was successful.</returns>
        public bool TryToConnectLines(IEnumerable<ILineShape> lines, ImmutableArray<IPointShape> connectors, double threshold)
        {
            if (connectors.Length > 0)
            {
                var lineToPoints = new Dictionary<ILineShape, IList<IPointShape>>();

                // Find possible connector to line connections.
                foreach (var connector in connectors)
                {
                    ILineShape result = null;
                    foreach (var line in lines)
                    {
                        if (HitTest.Contains(line, new Point2(connector.X, connector.Y), threshold))
                        {
                            result = line;
                            break;
                        }
                    }

                    if (result != null)
                    {
                        if (lineToPoints.ContainsKey(result))
                        {
                            lineToPoints[result].Add(connector);
                        }
                        else
                        {
                            lineToPoints.Add(result, new List<IPointShape>());
                            lineToPoints[result].Add(connector);
                        }
                    }
                }

                // Try to split lines using connectors.
                bool success = false;
                foreach (var kv in lineToPoints)
                {
                    var line = kv.Key;
                    var points = kv.Value;
                    if (points.Count == 2)
                    {
                        var p0 = points[0];
                        var p1 = points[1];
                        bool horizontal = Abs(p0.Y - p1.Y) < threshold;
                        bool vertical = Abs(p0.X - p1.X) < threshold;

                        // Points are aligned horizontally.
                        if (horizontal && !vertical)
                        {
                            if (p0.X <= p1.X)
                            {
                                success = TryToSplitLine(line, p0, p1);
                            }
                            else
                            {
                                success = TryToSplitLine(line, p1, p0);
                            }
                        }

                        // Points are aligned vertically.
                        if (!horizontal && vertical)
                        {
                            if (p0.Y >= p1.Y)
                            {
                                success = TryToSplitLine(line, p1, p0);
                            }
                            else
                            {
                                success = TryToSplitLine(line, p0, p1);
                            }
                        }
                    }
                }

                return success;
            }

            return false;
        }

        private IGroupShape Group(ILayerContainer layer, ISet<IBaseShape> shapes, string name)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();
                var group = Factory.CreateGroupShape(name);
                group.Group(shapes, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                return group;
            }

            return null;
        }

        private void Ungroup(ILayerContainer layer, ISet<IBaseShape> shapes)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();

                foreach (var shape in shapes)
                {
                    if (shape is IGroupShape group)
                    {
                        group.Ungroup(source);
                    }
                }

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        private void Ungroup(ILayerContainer layer, IGroupShape group)
        {
            if (layer != null && group != null)
            {
                var source = layer.Shapes.ToBuilder();

                group.Ungroup(source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Group shapes.
        /// </summary>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="name">The group name.</param>
        public IGroupShape Group(ISet<IBaseShape> shapes, string name)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                return Group(layer, shapes, name);
            }

            return null;
        }

        /// <summary>
        /// Ungroup shapes.
        /// </summary>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        public bool Ungroup(IBaseShape shape, ISet<IBaseShape> shapes)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                if (shape != null && shape is IGroupShape group)
                {
                    Ungroup(layer, group);
                    return true;
                }

                if (shapes != null)
                {
                    Ungroup(layer, shapes);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Swap shape from source index to target index position in an array. 
        /// </summary>
        /// <param name="shape">The source shape.</param>
        /// <param name="sourceIndex">The source shape index.</param>
        /// <param name="targetIndex">The target shape index.</param>
        private void Swap(IBaseShape shape, int sourceIndex, int targetIndex)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer?.Shapes != null)
            {
                if (sourceIndex < targetIndex)
                {
                    Project.SwapShape(layer, shape, targetIndex + 1, sourceIndex);
                }
                else
                {
                    if (layer.Shapes.Length + 1 > sourceIndex + 1)
                    {
                        Project.SwapShape(layer, shape, targetIndex, sourceIndex + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Bring a shape to the top of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void BringToFront(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = 0;
                if (sourceIndex != targetIndex)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        /// <summary>
        /// Move a shape one step closer to the front of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void BringForward(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = sourceIndex - 1;
                if (targetIndex >= 0)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        /// <summary>
        /// Move a shape one step down within the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendBackward(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = sourceIndex + 1;
                if (targetIndex < items.Length)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        /// <summary>
        /// Move a shape to the bottom of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendToBack(IBaseShape source)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                var items = layer.Shapes;
                int sourceIndex = items.IndexOf(source);
                int targetIndex = items.Length - 1;
                if (targetIndex >= 0 && sourceIndex != targetIndex)
                {
                    Swap(source, sourceIndex, targetIndex);
                }
            }
        }

        /// <summary>
        /// Move shapes by specified offset.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        public static void MoveShapesBy(IEnumerable<IBaseShape> shapes, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(null, dx, dy);
                }
            }
        }

        private void MoveShapesByWithHistory(IEnumerable<IBaseShape> shapes, double dx, double dy)
        {
            MoveShapesBy(shapes, dx, dy);

            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = shapes };
            var next = new { DeltaX = dx, DeltaY = dy, Shapes = shapes };
            Project?.History?.Snapshot(previous, next, (s) => MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
        }

        /// <summary>
        /// Move shape(s) by specified offset.
        /// </summary>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        public void MoveBy(IBaseShape shape, ISet<IBaseShape> shapes, double dx, double dy)
        {
            if (shape != null)
            {
                switch (Project?.Options?.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var distinct = Enumerable.Repeat(shape, 1).SelectMany(s => s.GetPoints()).Distinct().ToList();
                                MoveShapesByWithHistory(distinct, dx, dy);
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked) && !shape.State.Flags.HasFlag(ShapeStateFlags.Connector))
                            {
                                var items = Enumerable.Repeat(shape, 1).ToList();
                                MoveShapesByWithHistory(items, dx, dy);
                            }
                        }
                        break;
                }
            }

            if (shapes != null)
            {
                switch (Project?.Options?.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var distinct = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked)).SelectMany(s => s.GetPoints()).Distinct().ToList();
                            MoveShapesByWithHistory(distinct, dx, dy);
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                            MoveShapesByWithHistory(items, dx, dy);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Move items in the library.
        /// </summary>
        /// <typeparam name="T">The type of the library.</typeparam>
        /// <param name="library">The items library.</param>
        /// <param name="sourceIndex">The source item index.</param>
        /// <param name="targetIndex">The target item index.</param>
        public void MoveItem<T>(ILibrary<T> library, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var item = library.Items[sourceIndex];
                var builder = library.Items.ToBuilder();
                builder.Insert(targetIndex + 1, item);
                builder.RemoveAt(sourceIndex);

                var previous = library.Items;
                var next = builder.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                library.Items = next;
            }
            else
            {
                int removeIndex = sourceIndex + 1;
                if (library.Items.Length + 1 > removeIndex)
                {
                    var item = library.Items[sourceIndex];
                    var builder = library.Items.ToBuilder();
                    builder.Insert(targetIndex, item);
                    builder.RemoveAt(removeIndex);

                    var previous = library.Items;
                    var next = builder.ToImmutable();
                    Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                    library.Items = next;
                }
            }
        }

        /// <summary>
        /// Swap items in the library.
        /// </summary>
        /// <typeparam name="T">The type of the library.</typeparam>
        /// <param name="library">The items library.</param>
        /// <param name="sourceIndex">The source item index.</param>
        /// <param name="targetIndex">The target item index.</param>
        public void SwapItem<T>(ILibrary<T> library, int sourceIndex, int targetIndex)
        {
            var item1 = library.Items[sourceIndex];
            var item2 = library.Items[targetIndex];
            var builder = library.Items.ToBuilder();
            builder[targetIndex] = item1;
            builder[sourceIndex] = item2;

            var previous = library.Items;
            var next = builder.ToImmutable();
            Project?.History?.Snapshot(previous, next, (p) => library.Items = p);
            library.Items = next;
        }
    }
}
