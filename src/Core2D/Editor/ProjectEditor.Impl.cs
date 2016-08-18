// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Core2D.Collections;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor.Bounds;
using Core2D.Editor.Interfaces;
using Core2D.Editor.Recent;
using Core2D.Editor.Tools;
using Core2D.Editor.Tools.Path;
using Core2D.History;
using Core2D.Interfaces;
using Core2D.Math;
using Core2D.Path.Parser;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using static System.Math;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor implementation.
    /// </summary>
    public partial class ProjectEditor
    {
        private XContainer _pageToCopy = default(XContainer);
        private XDocument _documentToCopy = default(XDocument);
        private BaseShape _hover = default(BaseShape);

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
        /// Get object item name.
        /// </summary>
        /// <param name="item">The object item.</param>
        public string GetName(object item)
        {
            if (item != null)
            {
                if (item is BaseStyle)
                    return (item as BaseStyle).Name;
                else if (item is BaseShape)
                    return (item as BaseShape).Name;
                else if (item is XLibrary<ShapeStyle>)
                    return (item as XLibrary<ShapeStyle>).Name;
                else if (item is XLibrary<XGroup>)
                    return (item as XLibrary<XGroup>).Name;
                if (item is XContainer)
                    return (item as XContainer).Name;
                if (item is XLayer)
                    return (item as XLayer).Name;
                if (item is XDocument)
                    return (item as XDocument).Name;
                if (item is XProject)
                    return (item as XProject).Name;
                if (item is XDatabase)
                    return (item as XDatabase).Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Create new project, document or page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnNew(object item)
        {
            if (item is XContainer)
            {
                OnNewPage(item as XContainer);
            }
            else if (item is XDocument)
            {
                OnNewPage(item as XDocument);
            }
            else if (item is XProject)
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
        public void OnNewPage(XContainer selected)
        {
            var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(selected));
            if (document != null)
            {
                var page =
                    ProjectFactory?.GetPage(Project, Constants.DefaultPageName)
                    ?? XContainer.CreatePage(Constants.DefaultPageName);

                Project?.AddPage(document, page);
                Project?.SetCurrentContainer(page);
            }
        }

        /// <summary>
        /// Create new page.
        /// </summary>
        /// <param name="selected">The selected document.</param>
        public void OnNewPage(XDocument selected)
        {
            var page =
                ProjectFactory?.GetPage(Project, Constants.DefaultPageName)
                ?? XContainer.CreatePage(Constants.DefaultPageName);

            Project?.AddPage(selected, page);
            Project?.SetCurrentContainer(page);
        }

        /// <summary>
        /// Create new document.
        /// </summary>
        public void OnNewDocument()
        {
            var document =
                ProjectFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                ?? XDocument.Create(Constants.DefaultDocumentName);

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
            OnLoad(ProjectFactory?.GetProject() ?? XProject.Create(), string.Empty);
            OnChangeCurrentView(Views.FirstOrDefault(view => view.Name == "Editor"));
            Invalidate?.Invoke();
        }

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void OnOpen(string path)
        {
            try
            {
                if (FileIO != null && JsonSerializer != null)
                {
                    if (!string.IsNullOrEmpty(path) && FileIO.Exists(path))
                    {
                        var project = XProject.Open(path, FileIO, JsonSerializer);
                        if (project != null)
                        {
                            OnOpen(project, path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="project">The project to open.</param>
        /// <param name="path">The project file path.</param>
        public void OnOpen(XProject project, string path)
        {
            try
            {
                if (project != null)
                {
                    OnUnload();
                    OnLoad(project, path);
                    OnAddRecent(path, project.Name);
                    OnChangeCurrentView(Views.FirstOrDefault(view => view.Name == "Editor"));
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Close project.
        /// </summary>
        public void OnClose()
        {
            OnChangeCurrentView(Views.FirstOrDefault(view => view.Name == "Dashboard"));
            Project?.History?.Reset();
            OnUnload();
        }

        /// <summary>
        /// Save project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void OnSave(string path)
        {
            try
            {
                if (Project != null && FileIO != null && JsonSerializer != null)
                {
                    XProject.Save(Project, path, FileIO, JsonSerializer);
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        public void OnImportData(string path)
        {
            try
            {
                if (Project != null)
                {
                    var db = CsvReader?.Read(path, FileIO);
                    if (db != null)
                    {
                        Project.AddDatabase(db);
                        Project.SetCurrentDatabase(db);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnExportData(string path, XDatabase database)
        {
            try
            {
                CsvWriter?.Write(path, FileIO, database);
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void OnUpdateData(string path, XDatabase database)
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Import object.
        /// </summary>
        /// <param name="item">The object to import.</param>
        /// <param name="restore">Try to restore objects by name.</param>
        public void OnImportObject(object item, bool restore)
        {
            if (item is ShapeStyle)
            {
                Project?.AddStyle(Project?.CurrentStyleLibrary, item as ShapeStyle);
            }
            else if (item is IList<ShapeStyle>)
            {
                Project.AddItems(Project?.CurrentStyleLibrary, item as IList<ShapeStyle>);
            }
            else if (item is BaseShape)
            {
                if (item is XGroup)
                {
                    var group = item as XGroup;
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
                    Project?.AddShape(Project?.CurrentContainer?.CurrentLayer, item as BaseShape);
                }
            }
            else if (item is IList<XGroup>)
            {
                var groups = item as IList<XGroup>;
                if (restore)
                {
                    TryToRestoreStyles(groups);
                    TryToRestoreRecords(groups);
                }
                Project.AddItems(Project?.CurrentGroupLibrary, groups);
            }
            else if (item is XLibrary<ShapeStyle>)
            {
                Project.AddStyleLibrary(item as XLibrary<ShapeStyle>);
            }
            else if (item is IList<XLibrary<ShapeStyle>>)
            {
                Project.AddStyleLibraries(item as IList<XLibrary<ShapeStyle>>);
            }
            else if (item is XLibrary<XGroup>)
            {
                var gl = item as XLibrary<XGroup>;
                TryToRestoreStyles(gl.Items);
                TryToRestoreRecords(gl.Items);
                Project.AddGroupLibrary(gl);
            }
            else if (item is IList<XLibrary<XGroup>>)
            {
                var gll = item as IList<XLibrary<XGroup>>;
                var shapes = gll.SelectMany(x => x.Items);
                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);
                Project.AddGroupLibraries(gll);
            }
            else if (item is XStyles)
            {
                var styles = item as XStyles;
                var library = XLibrary<ShapeStyle>.Create(styles.Name, styles.Children);
                Project?.AddStyleLibrary(library);
            }
            else if (item is XShapes)
            {
                var shapes = (item as XShapes).Children;
                if (shapes.Length > 0)
                {
                    Project?.AddShapes(Project?.CurrentContainer?.CurrentLayer, shapes);
                }
            }
            else if (item is XGroups)
            {
                var groups = item as XGroups;
                var library = XLibrary<XGroup>.Create(groups.Name, groups.Children);
                Project?.AddGroupLibrary(library);
            }
            else if (item is XDatabases)
            {
                var databases = (item as XDatabases).Children;
                if (databases.Length > 0)
                {
                    foreach (var database in databases)
                    {
                        Project?.AddDatabase(database);
                    }
                }
            }
            else if (item is XTemplates)
            {
                var templates = (item as XTemplates).Children;
                if (templates.Length > 0)
                {
                    foreach (var template in templates)
                    {
                        Project?.AddTemplate(template);
                    }
                }
            }
            else if (item is XContext)
            {
                if (Renderers?[0]?.State?.SelectedShape != null || (Renderers?[0]?.State?.SelectedShapes?.Count > 0))
                {
                    OnApplyData(item as XContext);
                }
                else
                {
                    var container = Project?.CurrentContainer;
                    if (container != null)
                    {
                        container.Data = item as XContext;
                    }
                }
            }
            else if (item is XDatabase)
            {
                var db = item as XDatabase;
                Project?.AddDatabase(db);
                Project?.SetCurrentDatabase(db);
            }
            else if (item is XLayer)
            {
                var layer = item as XLayer;
                if (restore)
                {
                    TryToRestoreStyles(layer.Shapes);
                    TryToRestoreRecords(layer.Shapes);
                }
                Project?.AddLayer(Project?.CurrentContainer, layer);
            }
            else if (item is XContainer)
            {
                var container = item as XContainer;
                if (container.Template == null)
                {
                    // Import as template.
                    if (restore)
                    {
                        var shapes = container.Layers.SelectMany(x => x.Shapes);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project?.AddTemplate(container);
                }
                else
                {
                    // Import as page.
                    if (restore)
                    {
                        var shapes = Enumerable.Concat(
                            container.Layers.SelectMany(x => x.Shapes),
                            container.Template?.Layers.SelectMany(x => x.Shapes));
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                    }
                    Project?.AddPage(Project?.CurrentDocument, container);
                }
            }
            else if (item is IList<XContainer>)
            {
                var templates = item as IList<XContainer>;
                if (restore)
                {
                    var shapes = templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                    TryToRestoreStyles(shapes);
                    TryToRestoreRecords(shapes);
                }

                // Import as templates.
                Project.AddTemplates(templates);
            }
            else if (item is XDocument)
            {
                var document = item as XDocument;
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
            else if (item is XOptions)
            {
                if (Project != null)
                {
                    Project.Options = item as XOptions;
                }
            }
            else if (item is XProject)
            {
                OnUnload();
                OnLoad(item as XProject, string.Empty);
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Cut selected document, page or shapes to clipboard.
        /// </summary>
        public void OnCut()
        {
            try
            {
                if (CanCopy())
                {
                    OnCopy();
                    OnDeleteSelected();
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Copy document, page or shapes to clipboard.
        /// </summary>
        public void OnCopy()
        {
            try
            {
                if (CanCopy())
                {
                    if (Renderers?[0]?.State?.SelectedShape != null)
                    {
                        OnCopy(Enumerable.Repeat(Renderers[0].State.SelectedShape, 1).ToList());
                    }

                    if (Renderers?[0]?.State?.SelectedShapes != null)
                    {
                        OnCopy(Renderers[0].State.SelectedShapes.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        public async void OnPaste()
        {
            try
            {
                if (await CanPaste())
                {
                    var text = await (TextClipboard?.GetText() ?? Task.FromResult(string.Empty));
                    if (!string.IsNullOrEmpty(text))
                    {
                        OnPaste(text);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Cut selected document, page or shapes.
        /// </summary>
        /// <param name="item">The item to cut.</param>
        public void OnCut(object item)
        {
            if (item is XContainer)
            {
                var page = item as XContainer;
                _pageToCopy = page;
                _documentToCopy = default(XDocument);
                Project?.RemovePage(page);
                Project?.SetCurrentContainer(Project?.CurrentDocument?.Pages.FirstOrDefault());
            }
            else if (item is XDocument)
            {
                var document = item as XDocument;
                _pageToCopy = default(XContainer);
                _documentToCopy = document;
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditor || item == null)
            {
                OnCut();
            }
        }

        /// <summary>
        /// Copy document, page or shapes to clipboard.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        public void OnCopy(object item)
        {
            if (item is XContainer)
            {
                var page = item as XContainer;
                _pageToCopy = page;
                _documentToCopy = default(XDocument);
            }
            else if (item is XDocument)
            {
                var document = item as XDocument;
                _pageToCopy = default(XContainer);
                _documentToCopy = document;
            }
            else if (item is ProjectEditor || item == null)
            {
                OnCopy();
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        /// <param name="item">The item to paste.</param>
        public void OnPaste(object item)
        {
            if (Project != null && item is XContainer)
            {
                if (_pageToCopy != null)
                {
                    var page = item as XContainer;
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
            else if (Project != null && item is XDocument)
            {
                if (_pageToCopy != null)
                {
                    var document = item as XDocument;
                    var clone = Clone(_pageToCopy);
                    Project?.AddPage(document, clone);
                    Project.SetCurrentContainer(clone);
                }
                else if (_documentToCopy != null)
                {
                    var document = item as XDocument;
                    int index = Project.Documents.IndexOf(document);
                    var clone = Clone(_documentToCopy);
                    Project.ReplaceDocument(clone, index);
                    Project.SetCurrentDocument(clone);
                    Project.SetCurrentContainer(clone?.Pages.FirstOrDefault());
                }
            }
            else if (item is ProjectEditor || item == null)
            {
                OnPaste();
            }
        }

        /// <summary>
        /// Delete selected document, page, layer or shapes.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void OnDelete(object item)
        {
            if (item is XLayer)
            {
                var layer = item as XLayer;
                Project?.RemoveLayer(item as XLayer);

                var selected = Project?.CurrentContainer?.Layers.FirstOrDefault();
                layer?.Owner?.SetCurrentLayer(selected);
            }
            if (item is XContainer)
            {
                Project?.RemovePage(item as XContainer);

                var selected = Project?.CurrentDocument?.Pages.FirstOrDefault();
                Project?.SetCurrentContainer(selected);
            }
            else if (item is XDocument)
            {
                Project?.RemoveDocument(item as XDocument);

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
                    ImmutableHashSet.CreateRange<BaseShape>(Project?.CurrentContainer?.CurrentLayer?.Shapes));
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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

                    container.WorkingLayer.Shapes = ImmutableArray.Create<BaseShape>();
                    container.HelperLayer.Shapes = ImmutableArray.Create<BaseShape>();

                    Project.CurrentContainer.Invalidate();
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
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
        /// Move selected shapes one step closer to the front of the stack.
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
        /// Move selected shapes one step down within the stack.
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
        /// Move selected shapes to the bottom of the stack.
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
        /// Set current tool to <see cref="Tool.None"/>.
        /// </summary>
        public void OnToolNone()
        {
            CurrentTool = typeof(ToolNone);
        }
        
        /// <summary>
        /// Set current tool to <see cref="Tool.Selection"/>.
        /// </summary>
        public void OnToolSelection()
        {
            CurrentTool = typeof(ToolSelection);
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Point"/>.
        /// </summary>
        public void OnToolPoint()
        {
            CurrentTool = typeof(ToolPoint);
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Line"/> or current path tool to <see cref="PathTool.Line"/>.
        /// </summary>
        public void OnToolLine()
        {
            if (CurrentTool == typeof(ToolPath) && CurrentPathTool != typeof(PathToolLine))
            {
                CurrentPathTool = typeof(PathToolLine);
            }
            else
            {
                CurrentTool = typeof(ToolLine);
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Arc"/> or current path tool to <see cref="PathTool.Arc"/>.
        /// </summary>
        public void OnToolArc()
        {
            if (CurrentTool == typeof(ToolPath) && CurrentPathTool != typeof(PathToolArc))
            {
                CurrentPathTool = typeof(PathToolArc);
            }
            else
            {
                CurrentTool = typeof(ToolArc);
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.CubicBezier"/> or current path tool to <see cref="PathTool.CubicBezier"/>.
        /// </summary>
        public void OnToolCubicBezier()
        {
            if (CurrentTool == typeof(ToolPath) && CurrentPathTool != typeof(PathToolCubicBezier))
            {
                CurrentPathTool = typeof(PathToolCubicBezier);
            }
            else
            {
                CurrentTool = typeof(ToolCubicBezier);
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.QuadraticBezier"/> or current path tool to <see cref="PathTool.QuadraticBezier"/>.
        /// </summary>
        public void OnToolQuadraticBezier()
        {
            if (CurrentTool == typeof(ToolPath) && CurrentPathTool != typeof(PathToolQuadraticBezier))
            {
                CurrentPathTool = typeof(PathToolQuadraticBezier);
            }
            else
            {
                CurrentTool = typeof(ToolQuadraticBezier);
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Path"/>.
        /// </summary>
        public void OnToolPath()
        {
            CurrentTool = typeof(ToolPath);
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Rectangle"/>.
        /// </summary>
        public void OnToolRectangle()
        {
            CurrentTool = typeof(ToolRectangle);
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Ellipse"/>.
        /// </summary>
        public void OnToolEllipse()
        {
            CurrentTool = typeof(ToolEllipse);
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Text"/>.
        /// </summary>
        public void OnToolText()
        {
            CurrentTool = typeof(ToolText);
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Image"/>.
        /// </summary>
        public void OnToolImage()
        {
            CurrentTool = typeof(ToolImage);
        }

        /// <summary>
        /// Set current path tool to <see cref="PathTool.Move"/>.
        /// </summary>
        public void OnToolMove()
        {
            if (CurrentTool == typeof(ToolPath) && CurrentPathTool != typeof(PathToolMove))
            {
                CurrentPathTool = typeof(PathToolMove);
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsStroked"/> option.
        /// </summary>
        public void OnToggleDefaultIsStroked()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsStroked = !Project.Options.DefaultIsStroked;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsFilled"/> option.
        /// </summary>
        public void OnToggleDefaultIsFilled()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsFilled = !Project.Options.DefaultIsFilled;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsClosed"/> option.
        /// </summary>
        public void OnToggleDefaultIsClosed()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsClosed = !Project.Options.DefaultIsClosed;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public void OnToggleDefaultIsSmoothJoin()
        {
            if (Project?.Options != null)
            {
                Project.Options.DefaultIsSmoothJoin = !Project.Options.DefaultIsSmoothJoin;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.SnapToGrid"/> option.
        /// </summary>
        public void OnToggleSnapToGrid()
        {
            if (Project?.Options != null)
            {
                Project.Options.SnapToGrid = !Project.Options.SnapToGrid;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.TryToConnect"/> option.
        /// </summary>
        public void OnToggleTryToConnect()
        {
            if (Project?.Options != null)
            {
                Project.Options.TryToConnect = !Project.Options.TryToConnect;
            }
        }

        /// <summary>
        /// Toggle <see cref="XOptions.CloneStyle"/> option.
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
            var db = XDatabase.Create(Constants.DefaultDatabaseName);
            Project.AddDatabase(db);
            Project.SetCurrentDatabase(db);
        }

        /// <summary>
        /// Remove database.
        /// </summary>
        /// <param name="db">The database to remove.</param>
        public void OnRemoveDatabase(XDatabase db)
        {
            Project.RemoveDatabase(db);
            Project.SetCurrentDatabase(Project.Databases.FirstOrDefault());
        }

        /// <summary>
        /// Add column to database.
        /// </summary>
        /// <param name="db">The records database.</param>
        public void OnAddColumn(XDatabase db)
        {
            Project.AddColumn(db, XColumn.Create(db, Constants.DefaulColumnName));
        }

        /// <summary>
        /// Remove column from database.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        public void OnRemoveColumn(XColumn column)
        {
            Project.RemoveColumn(column);
        }

        /// <summary>
        /// Add record to database.
        /// </summary>
        /// <param name="db">The records database.</param>
        public void OnAddRecord(XDatabase db)
        {
            Project.AddRecord(db, XRecord.Create(db, Constants.DefaulValue));
        }

        /// <summary>
        /// Remove record from database.
        /// </summary>
        /// <param name="record">The data record.</param>
        public void OnRemoveRecord(XRecord record)
        {
            Project.RemoveRecord(record);
        }

        /// <summary>
        /// Reset data context record.
        /// </summary>
        /// <param name="data">The data context.</param>
        public void OnResetRecord(XContext data)
        {
            Project.ResetRecord(data);
        }

        /// <summary>
        /// Set current record as selected shape(s) or current page data record.
        /// </summary>
        /// <param name="record">The data record.</param>
        public void OnApplyRecord(XRecord record)
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
        public void OnAddProperty(XContext data)
        {
            Project.AddProperty(data, XProperty.Create(data, Constants.DefaulPropertyName, Constants.DefaulValue));
        }

        /// <summary>
        /// Remove property from data context.
        /// </summary>
        /// <param name="property">The property to remove.</param>
        public void OnRemoveProperty(XProperty property)
        {
            Project.RemoveProperty(property);
        }

        /// <summary>
        /// Add group library.
        /// </summary>
        public void OnAddGroupLibrary()
        {
            var gl = XLibrary<XGroup>.Create(Constants.DefaulGroupLibraryName);
            Project.AddGroupLibrary(gl);
            Project.SetCurrentGroupLibrary(gl);
        }

        /// <summary>
        /// Remove group library.
        /// </summary>
        /// <param name="library">The group library to remove.</param>
        public void OnRemoveGroupLibrary(XLibrary<XGroup> library)
        {
            Project.RemoveGroupLibrary(library);
            Project.SetCurrentGroupLibrary(Project?.GroupLibraries.FirstOrDefault());
        }

        /// <summary>
        /// Add group.
        /// </summary>
        /// <param name="library">The group library.</param>
        public void OnAddGroup(XLibrary<XGroup> library)
        {
            if (Project != null && library != null)
            {
                var group = Renderers?[0]?.State?.SelectedShape as XGroup;
                if (group != null)
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
        public void OnRemoveGroup(XGroup group)
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
        public void OnInsertGroup(XGroup group)
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
        public void OnAddLayer(XContainer container)
        {
            Project.AddLayer(container, XLayer.Create(Constants.DefaultLayerName, container));
        }

        /// <summary>
        /// Remove layer.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        public void OnRemoveLayer(XLayer layer)
        {
            Project.RemoveLayer(layer);
            layer.Owner.SetCurrentLayer(layer.Owner.Layers.FirstOrDefault());
        }

        /// <summary>
        /// Add style library.
        /// </summary>
        public void OnAddStyleLibrary()
        {
            var sl = XLibrary<ShapeStyle>.Create(Constants.DefaulStyleLibraryName);
            Project.AddStyleLibrary(sl);
            Project.SetCurrentStyleLibrary(sl);
        }

        /// <summary>
        /// Remove style library.
        /// </summary>
        /// <param name="library">The style library to remove.</param>
        public void OnRemoveStyleLibrary(XLibrary<ShapeStyle> library)
        {
            Project.RemoveStyleLibrary(library);
            Project.SetCurrentStyleLibrary(Project?.StyleLibraries.FirstOrDefault());
        }

        /// <summary>
        /// Add style.
        /// </summary>
        /// <param name="library">The style library.</param>
        public void OnAddStyle(XLibrary<ShapeStyle> library)
        {
            Project.AddStyle(library, ShapeStyle.Create(Constants.DefaulStyleName));
        }

        /// <summary>
        /// Remove style.
        /// </summary>
        /// <param name="style">The style to remove.</param>
        public void OnRemoveStyle(ShapeStyle style)
        {
            var library = Project.RemoveStyle(style);
            library?.SetSelected(library?.Items.FirstOrDefault());
        }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        /// <param name="style">The shape style item.</param>
        public void OnApplyStyle(ShapeStyle style)
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
        public void OnApplyData(XContext data)
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
        public void OnAddShape(BaseShape shape)
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
        public void OnRemoveShape(BaseShape shape)
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
                var template = ProjectFactory.GetTemplate(Project, "Empty");
                if (template == null)
                {
                    template = XContainer.CreateTemplate(Constants.DefaultTemplateName);
                }

                Project.AddTemplate(template);
            }
        }

        /// <summary>
        /// Remove template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnRemoveTemplate(XContainer template)
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
        public void OnEditTemplate(XContainer template)
        {
            if (Project != null && template != null)
            {
                Project.SetCurrentDocument(null);
                Project.SetCurrentContainer(template);
                Project.CurrentContainer?.Invalidate();
            }
        }

        /// <summary>
        /// Set page template.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void OnApplyTemplate(XContainer template)
        {
            var page = Project?.CurrentContainer;
            if (page != null && template != null)
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
                    var key = await (GetImageKey() ?? Task.FromResult(string.Empty));
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
                    var key = Project.AddImageFromFile(path, bytes);
                    return key;
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
                Project?.RemoveImage(key);
            }
        }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public void OnSelectedItemChanged(XSelectable item)
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
                    ProjectFactory?.GetPage(Project, Constants.DefaultPageName)
                    ?? XContainer.CreatePage(Constants.DefaultPageName);

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
                if (item is XContainer)
                {
                    var selected = item as XContainer;
                    int index = Project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        ProjectFactory?.GetPage(Project, Constants.DefaultPageName)
                        ?? XContainer.CreatePage(Constants.DefaultPageName);

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
                if (item is XContainer)
                {
                    var selected = item as XContainer;
                    int index = Project.CurrentDocument.Pages.IndexOf(selected);
                    var page =
                        ProjectFactory?.GetPage(Project, Constants.DefaultPageName)
                        ?? XContainer.CreatePage(Constants.DefaultPageName);

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
                    ProjectFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                    ?? XDocument.Create(Constants.DefaultDocumentName);

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
                if (item is XDocument)
                {
                    var selected = item as XDocument;
                    int index = Project.Documents.IndexOf(selected);
                    var document =
                        ProjectFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                        ?? XDocument.Create(Constants.DefaultDocumentName);

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
                if (item is XDocument)
                {
                    var selected = item as XDocument;
                    int index = Project.Documents.IndexOf(selected);
                    var document =
                        ProjectFactory?.GetDocument(Project, Constants.DefaultDocumentName)
                        ?? XDocument.Create(Constants.DefaultDocumentName);

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
        public void OnLoad(XProject project, string path = null)
        {
            if (project != null)
            {
                Deselect();
                SetRenderersImageCache(project);
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
            Observer?.Dispose();
            Observer = null;

            if (Project?.History != null)
            {
                Project.History.Reset();
                Project.History = null;
            }

            Project?.PurgeUnusedImages(Enumerable.Empty<string>().ToImmutableHashSet());

            Deselect();
            SetRenderersImageCache(null);
            Project = null;
            ProjectPath = string.Empty;
            IsProjectDirty = false;

            GC.Collect();
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                    Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                    Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            return false;
        }

        /// <summary>
        /// Copy selected shapes to clipboard.
        /// </summary>
        /// <param name="shapes"></param>
        public void OnCopy(IList<BaseShape> shapes)
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Paste text from clipboard as shapes.
        /// </summary>
        /// <param name="text">The text string.</param>
        public void OnPaste(string text)
        {
            try
            {
                var exception = default(Exception);

                // Try to parse SVG path geometry. 
                try
                {
                    var geometry = XPathGeometryParser.Parse(text);
                    var style = Project?.CurrentStyleLibrary?.Selected;
                    if (style != null)
                    {
                        var path = XPath.Create(
                            "Path",
                            Project.Options.CloneStyle ? style.Clone() : style,
                            geometry,
                            Project.Options.DefaultIsStroked,
                            Project.Options.DefaultIsFilled);

                        OnPaste(Enumerable.Repeat(path, 1));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

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
                    var shapes = JsonSerializer?.Deserialize<IList<BaseShape>>(text);
                    if (shapes?.Count() > 0)
                    {
                        OnPaste(shapes);
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        private void ResetPointShapeToDefault(IEnumerable<BaseShape> shapes)
        {
            foreach (var point in shapes?.SelectMany(s => s?.GetPoints()))
            {
                point.Shape = Project?.Options?.PointShape;
            }
        }

        private IDictionary<string, ShapeStyle> GenerateStyleDictionaryByName()
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
        private void TryToRestoreStyles(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (Project?.StyleLibraries == null)
                    return;

                var styles = GenerateStyleDictionaryByName();

                // Reset point shape to defaults.
                ResetPointShapeToDefault(shapes);

                // Try to restore shape styles.
                foreach (var shape in XProject.GetAllShapes(shapes))
                {
                    if (shape?.Style == null)
                        continue;

                    if (!string.IsNullOrWhiteSpace(shape.Style.Name))
                    {
                        ShapeStyle style;
                        if (styles.TryGetValue(shape.Style.Name, out style))
                        {
                            // Use existing style.
                            shape.Style = style;
                        }
                        else
                        {
                            // Create Imported style library.
                            if (Project?.CurrentStyleLibrary == null)
                            {
                                var sl = XLibrary<ShapeStyle>.Create(Constants.ImportedStyleLibraryName);
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        private IDictionary<Guid, XRecord> GenerateRecordDictionaryById()
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
        private void TryToRestoreRecords(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (Project?.Databases == null)
                    return;

                var records = GenerateRecordDictionaryById();

                // Try to restore shape record.
                foreach (var shape in XProject.GetAllShapes(shapes))
                {
                    if (shape?.Data?.Record == null)
                        continue;

                    XRecord record;
                    if (records.TryGetValue(shape.Data.Record.Id, out record))
                    {
                        // Use existing record.
                        shape.Data.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (Project?.CurrentDatabase == null)
                        {
                            var db = XDatabase.Create(Constants.ImportedDatabaseName, shape.Data.Record.Columns);
                            Project.AddDatabase(db);
                            Project.SetCurrentDatabase(db);
                        }

                        // Add missing data record.
                        shape.Data.Record.Owner = Project.CurrentDatabase;
                        Project?.AddRecord(Project?.CurrentDatabase, shape.Data.Record);

                        // Recreate records dictionary.
                        records = GenerateRecordDictionaryById();
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Paste shapes to current container.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        public void OnPaste(IEnumerable<BaseShape> shapes)
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        public void OnSelect(IEnumerable<BaseShape> shapes)
        {
            if (shapes?.Count() == 1)
            {
                Select(Project?.CurrentContainer?.CurrentLayer, shapes.FirstOrDefault());
            }
            else
            {
                Select(Project?.CurrentContainer?.CurrentLayer, ImmutableHashSet.CreateRange<BaseShape>(shapes));
            }
        }

        /// <summary>
        /// Clone the <see cref="BaseShape"/> object.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <returns>The cloned <see cref="BaseShape"/> object.</returns>
        public T CloneShape<T>(T shape) where T : BaseShape
        {
            try
            {
                var json = JsonSerializer?.Serialize(shape);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<T>(json);
                    if (clone != null)
                    {
                        var shapes = Enumerable.Repeat(clone, 1).ToList();
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return default(T);
        }

        /// <summary>
        /// Clone the <see cref="XContainer"/> object.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> object.</param>
        /// <returns>The cloned <see cref="XContainer"/> object.</returns>
        public XContainer Clone(XContainer container)
        {
            try
            {
                var template = container?.Template;
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<XContainer>(json);
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return default(XContainer);
        }

        /// <summary>
        /// Clone the <see cref="XDocument"/> object.
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/> object.</param>
        /// <returns>The cloned <see cref="XDocument"/> object.</returns>
        public XDocument Clone(XDocument document)
        {
            try
            {
                var templates = document?.Pages.Select(c => c?.Template)?.ToArray();
                var json = JsonSerializer?.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<XDocument>(json);
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
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return default(XDocument);
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
                            OnOpen(path);
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
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            return false;
        }

        /// <summary>
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void OnDropShape(BaseShape shape, double x, double y)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null)
                {
                    var target = Renderers[0].State.SelectedShape;
                    if (target is XPoint)
                    {
                        var point = target as XPoint;
                        if (point != null)
                        {
                            point.Shape = shape;
                        }
                    }
                }
                else if (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes?.Count > 0)
                {
                    foreach (var target in Renderers[0].State.SelectedShapes)
                    {
                        if (target is XPoint)
                        {
                            var point = target as XPoint;
                            if (point != null)
                            {
                                point.Shape = shape;
                            }
                        }
                    }
                }
                else
                {
                    var layer = Project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var target = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), Project.Options.HitThreshold);
                        if (target != null)
                        {
                            if (target is XPoint)
                            {
                                var point = target as XPoint;
                                if (point != null)
                                {
                                    point.Shape = shape;
                                }
                            }
                        }
                        else
                        {
                            OnDropShapeAsClone(shape, x, y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void OnDropShapeAsClone<T>(T shape, double x, double y) where T : BaseShape
        {
            double sx = Project.Options.SnapToGrid ? Snap(x, Project.Options.SnapX) : x;
            double sy = Project.Options.SnapToGrid ? Snap(y, Project.Options.SnapY) : y;

            try
            {
                var clone = CloneShape(shape);
                if (clone != null)
                {
                    Deselect(Project?.CurrentContainer?.CurrentLayer);
                    clone.Move(sx, sy);

                    Project.AddShape(Project?.CurrentContainer?.CurrentLayer, clone);

                    Select(Project?.CurrentContainer?.CurrentLayer, clone);

                    if (Project.Options.TryToConnect)
                    {
                        if (clone is XGroup)
                        {
                            TryToConnectLines(
                                XProject.GetAllShapes<XLine>(Project?.CurrentContainer?.CurrentLayer?.Shapes),
                                (clone as XGroup).Connectors,
                                Project.Options.HitThreshold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Drop <see cref="XRecord"/> object in current container at specified location.
        /// </summary>
        /// <param name="record">The <see cref="XRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void OnDropRecord(XRecord record, double x, double y)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null
                    || (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    OnApplyRecord(record);
                }
                else
                {
                    var layer = Project?.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), Project.Options.HitThreshold);
                        if (result != null)
                        {
                            Project?.ApplyRecord(result.Data, record);
                        }
                        else
                        {
                            OnDropRecordAsGroup(record, x, y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Drop <see cref="XRecord"/> object in current container at specified location as group bound to this record.
        /// </summary>
        /// <param name="record">The <see cref="XRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void OnDropRecordAsGroup(XRecord record, double x, double y)
        {
            var selected = Project.CurrentStyleLibrary.Selected;
            var style = Project.Options.CloneStyle ? selected.Clone() : selected;
            var point = Project?.Options?.PointShape;
            var layer = Project?.CurrentContainer?.CurrentLayer;
            double sx = Project.Options.SnapToGrid ? Snap(x, Project.Options.SnapX) : x;
            double sy = Project.Options.SnapToGrid ? Snap(y, Project.Options.SnapY) : y;

            var g = XGroup.Create(Constants.DefaulGroupName);

            g.Data.Record = record;

            var length = record.Values.Length;
            double px = sx;
            double py = sy;
            double width = 150;
            double height = 15;

            for (int i = 0; i < length; i++)
            {
                var column = record.Columns[i];
                if (column.IsVisible)
                {
                    var binding = "{" + record.Columns[i].Name + "}";
                    var text = XText.Create(px, py, px + width, py + height, style, point, binding);
                    g.AddShape(text);
                    py += height;
                }
            }

            var rectangle = XRectangle.Create(sx, sy, sx + width, sy + length * height, style, point);
            g.AddShape(rectangle);

            var pt = XPoint.Create(sx + width / 2, sy, point);
            var pb = XPoint.Create(sx + width / 2, sy + length * height, point);
            var pl = XPoint.Create(sx, sy + (length * height) / 2, point);
            var pr = XPoint.Create(sx + width, sy + (length * height) / 2, point);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            Project.AddShape(layer, g);
        }

        /// <summary>
        /// Drop <see cref="ShapeStyle"/> object in current container at specified location.
        /// </summary>
        /// <param name="style">The <see cref="ShapeStyle"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void OnDropStyle(ShapeStyle style, double x, double y)
        {
            try
            {
                if (Renderers?[0]?.State?.SelectedShape != null
                    || (Renderers?[0]?.State?.SelectedShapes != null && Renderers?[0]?.State?.SelectedShapes.Count > 0))
                {
                    OnApplyStyle(style);
                }
                else
                {
                    var layer = Project.CurrentContainer?.CurrentLayer;
                    if (layer != null)
                    {
                        var result = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), Project.Options.HitThreshold);
                        if (result != null)
                        {
                            Project.ApplyStyle(result, style);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
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

                Renderers[0].State.SelectedShape = default(BaseShape);
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

                Renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
                layer.Invalidate();
            }
        }

        /// <summary>
        /// Select shape.
        /// </summary>
        /// <param name="shape">The shape to select.</param>
        public void Select(BaseShape shape)
        {
            if (Renderers?[0]?.State != null)
            {
                Renderers[0].State.SelectedShape = shape;

                if (Renderers[0].State.SelectedShapes != null)
                {
                    Renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
                }
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(ImmutableHashSet<BaseShape> shapes)
        {
            if (Renderers?[0]?.State != null)
            {
                if (Renderers[0].State.SelectedShape != null)
                {
                    Renderers[0].State.SelectedShape = default(BaseShape);
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
                Renderers[0].State.SelectedShape = default(BaseShape);
            }

            if (Renderers?[0].State?.SelectedShapes != null)
            {
                Renderers[0].State.SelectedShapes = default(ImmutableHashSet<BaseShape>);
            }
        }

        /// <summary>
        /// Select shape.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shape">The shape to select.</param>
        public void Select(XLayer layer, BaseShape shape)
        {
            Select(shape);

            if (layer?.Owner != null)
            {
                layer.Owner.CurrentShape = shape;
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                Invalidate?.Invoke();
            }
        }

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shapes">The shapes to select.</param>
        public void Select(XLayer layer, ImmutableHashSet<BaseShape> shapes)
        {
            Select(shapes);

            if (layer?.Owner?.CurrentShape != null)
            {
                layer.Owner.CurrentShape = default(BaseShape);
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                Invalidate?.Invoke();
            }
        }

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        /// <param name="layer">The layer object.</param>
        public void Deselect(XLayer layer)
        {
            Deselect();

            if (layer?.Owner?.CurrentShape != null)
            {
                layer.Owner.CurrentShape = default(BaseShape);
            }

            if (layer != null)
            {
                layer.Invalidate();
            }
            else
            {
                if (Invalidate != null)
                {
                    Invalidate();
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
        public bool TryToSelectShape(XLayer layer, double x, double y)
        {
            if (layer != null)
            {
                var result = ShapeHitTestPoint.HitTest(layer.Shapes, new Vector2(x, y), Project.Options.HitThreshold);
                if (result != null)
                {
                    Select(layer, result);
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
        public bool TryToSelectShapes(XLayer layer, XRectangle rectangle)
        {
            if (layer != null)
            {
                var rect = Rect2.Create(rectangle.TopLeft, rectangle.BottomRight);
                var result = ShapeHitTestSelection.HitTest(layer.Shapes, rect, Project.Options.HitThreshold);
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
                            Select(layer, result);
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
        public void Hover(XLayer layer, BaseShape shape)
        {
            if (layer != null)
            {
                Select(layer, shape);
                _hover = shape;
            }
        }

        /// <summary>
        /// De-hover shape.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        public void Dehover(XLayer layer)
        {
            if (layer != null && _hover != null)
            {
                _hover = default(BaseShape);
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
                && !(Renderers?[0]?.State?.SelectedShape != null && _hover != Renderers?[0]?.State?.SelectedShape))
            {
                var result = ShapeHitTestPoint.HitTest(Project.CurrentContainer?.CurrentLayer?.Shapes, new Vector2(x, y), Project.Options.HitThreshold);
                if (result != null)
                {
                    Hover(Project.CurrentContainer?.CurrentLayer, result);
                    return true;
                }
                else
                {
                    if (Renderers[0].State.SelectedShape != null && Renderers[0].State.SelectedShape == _hover)
                    {
                        Dehover(Project.CurrentContainer?.CurrentLayer);
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
        public XPoint TryToGetConnectionPoint(double x, double y)
        {
            if (Project.Options.TryToConnect)
            {
                var result = ShapeHitTestPoint.HitTest(
                    Project.CurrentContainer.CurrentLayer.Shapes,
                    new Vector2(x, y),
                    Project.Options.HitThreshold);
                if (result != null && result is XPoint)
                {
                    return result as XPoint;
                }
            }
            return null;
        }

        private void SwapLineStart(XLine line, XPoint point)
        {
            if (line?.Start != null && point != null)
            {
                var previous = line.Start;
                var next = point;
                Project?.History?.Snapshot(previous, next, (p) => line.Start = p);
                line.Start = next;
            }
        }

        private void SwapLineEnd(XLine line, XPoint point)
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
        public bool TryToSplitLine(double x, double y, XPoint point, bool select = false)
        {
            if (Project?.CurrentContainer == null || Project?.Options == null)
                return false;

            var result = ShapeHitTestPoint.HitTest(
                Project.CurrentContainer.CurrentLayer.Shapes,
                new Vector2(x, y),
                Project.Options.HitThreshold);

            if (result is XLine)
            {
                var line = result as XLine;

                if (!Project.Options.SnapToGrid)
                {
                    var a = new Vector2(line.Start.X, line.Start.Y);
                    var b = new Vector2(line.End.X, line.End.Y);
                    var nearest = MathHelpers.NearestPointOnLine(a, b, new Vector2(x, y));
                    point.X = nearest.X;
                    point.Y = nearest.Y;
                }

                var split = XLine.Create(
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
        public bool TryToSplitLine(XLine line, XPoint p0, XPoint p1)
        {
            if (Project?.Options == null)
                return false;

            // Points must be aligned horizontally or vertically.
            if (p0.X != p1.X && p0.Y != p1.Y)
                return false;

            // Line must be horizontal or vertical.
            if (line.Start.X != line.End.X && line.Start.Y != line.End.Y)
                return false;

            XLine split;
            if (line.Start.X > line.End.X || line.Start.Y > line.End.Y)
            {
                split = XLine.Create(
                    p0,
                    line.End,
                    line.Style,
                    Project.Options.PointShape,
                    line.IsStroked);

                SwapLineEnd(line, p1);
            }
            else
            {
                split = XLine.Create(
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
        public bool TryToConnectLines(IEnumerable<XLine> lines, ImmutableArray<XPoint> connectors, double threshold)
        {
            if (connectors.Length > 0)
            {
                var lineToPoints = new Dictionary<XLine, IList<XPoint>>();

                // Find possible connector to line connections.
                foreach (var connector in connectors)
                {
                    XLine result = null;
                    foreach (var line in lines)
                    {
                        if (LineBounds.Contains(line, new Vector2(connector.X, connector.Y), threshold, 0, 0))
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
                            lineToPoints.Add(result, new List<XPoint>());
                            lineToPoints[result].Add(connector);
                        }
                    }
                }

                // Try to split lines using connectors.
                bool success = false;
                foreach (var kv in lineToPoints)
                {
                    XLine line = kv.Key;
                    IList<XPoint> points = kv.Value;
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

        private XGroup Group(XLayer layer, ImmutableHashSet<BaseShape> shapes, string name)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();
                var group = XGroup.Group(name, shapes, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                return group;
            }

            return null;
        }

        private void Ungroup(XLayer layer, ImmutableHashSet<BaseShape> shapes)
        {
            if (layer != null && shapes != null)
            {
                var source = layer.Shapes.ToBuilder();

                XGroup.Ungroup(shapes, source);

                var previous = layer.Shapes;
                var next = source.ToImmutable();
                Project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        private void Ungroup(XLayer layer, BaseShape shape)
        {
            if (layer != null && shape != null)
            {
                var source = layer.Shapes.ToBuilder();

                XGroup.Ungroup(shape as XGroup, source);

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
        public XGroup Group(ImmutableHashSet<BaseShape> shapes, string name)
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
        public bool Ungroup(BaseShape shape, ImmutableHashSet<BaseShape> shapes)
        {
            var layer = Project?.CurrentContainer?.CurrentLayer;
            if (layer != null)
            {
                if (shape != null && shape is XGroup)
                {
                    Ungroup(layer, shape);
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
        private void Swap(BaseShape shape, int sourceIndex, int targetIndex)
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
        public void BringToFront(BaseShape source)
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
        /// Move a shape one step closer to the front of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void BringForward(BaseShape source)
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
        /// Move a shape one step down within the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendBackward(BaseShape source)
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
        /// Move a shape to the bottom of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        public void SendToBack(BaseShape source)
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
        /// Move shapes by specified offset.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        public static void MoveShapesBy(IEnumerable<BaseShape> shapes, double dx, double dy)
        {
            foreach (var shape in shapes)
            {
                if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                {
                    shape.Move(dx, dy);
                }
            }
        }

        private void MoveShapesByWithHistory(IEnumerable<BaseShape> shapes, double dx, double dy)
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
        public void MoveBy(BaseShape shape, ImmutableHashSet<BaseShape> shapes, double dx, double dy)
        {
            if (shape != null)
            {
                switch (Project?.Options?.MoveMode)
                {
                    case XMoveMode.Point:
                        {
                            if (!shape.State.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var distinct = Enumerable.Repeat(shape, 1).SelectMany(s => s.GetPoints()).Distinct().ToList();
                                MoveShapesByWithHistory(distinct, dx, dy);
                            }
                        }
                        break;
                    case XMoveMode.Shape:
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
                    case XMoveMode.Point:
                        {
                            var distinct = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked)).SelectMany(s => s.GetPoints()).Distinct().ToList();
                            MoveShapesByWithHistory(distinct, dx, dy);
                        }
                        break;
                    case XMoveMode.Shape:
                        {
                            var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));
                            MoveShapesByWithHistory(items, dx, dy);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if edit mode is active.
        /// </summary>
        /// <returns>Return true if edit mode is active.</returns>
        public bool IsEditMode()
        {
            return true;
        }

        /// <summary>
        /// Change current view.
        /// </summary>
        /// <param name="view">The view instance.</param>
        public void OnChangeCurrentView(IView view)
        {
            if (view != null && _currentView != view)
            {
                CurrentView = view;
            }
        }
    }
}
