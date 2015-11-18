// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// Project editor context.
    /// </summary>
    public class EditorContext : ObservableObject, IDisposable
    {
        private Commands _commands = new Commands();
        private Editor _editor;
        private IView _view;
        private IRenderer[] _renderers;
        private IProjectFactory _projectFactory;
        private ITextClipboard _textClipboard;
        private ISerializer _serializer;
        private IFileWriter _pdfWriter;
        private IFileWriter _dxfWriter;
        private ITextFieldReader<Database> _csvReader;
        private ITextFieldWriter<Database> _csvWriter;
        private ImmutableArray<RecentProject> _recentProjects = ImmutableArray.Create<RecentProject>();
        private RecentProject _currentRecentProject = default(RecentProject);
        private Container _containerToCopy = default(Container);
        private Document _documentToCopy = default(Document);

        /// <summary>
        /// Gets or sets editor commands.
        /// </summary>
        public Commands Commands
        {
            get { return _commands; }
            set { Update(ref _commands, value); }
        }

        /// <summary>
        /// Gets or sets current editor.
        /// </summary>
        public Editor Editor
        {
            get { return _editor; }
            set { Update(ref _editor, value); }
        }

        /// <summary>
        /// Gets or sets editor view.
        /// </summary>
        public IView View
        {
            get { return _view; }
            set { Update(ref _view, value); }
        }

        /// <summary>
        /// Gets or sets editor renderer's.
        /// </summary>
        public IRenderer[] Renderers
        {
            get { return _renderers; }
            set { Update(ref _renderers, value); }
        }

        /// <summary>
        ///Gets or sets project factory.
        /// </summary>
        public IProjectFactory ProjectFactory
        {
            get { return _projectFactory; }
            set { Update(ref _projectFactory, value); }
        }

        /// <summary>
        /// Gets or sets text clipboard.
        /// </summary>
        public ITextClipboard TextClipboard
        {
            get { return _textClipboard; }
            set { Update(ref _textClipboard, value); }
        }

        /// <summary>
        /// Gets or sets object serializer.
        /// </summary>
        public ISerializer Serializer
        {
            get { return _serializer; }
            set { Update(ref _serializer, value); }
        }

        /// <summary>
        /// Gets or sets Pdf file writer.
        /// </summary>
        public IFileWriter PdfWriter
        {
            get { return _pdfWriter; }
            set { Update(ref _pdfWriter, value); }
        }

        /// <summary>
        /// Gets or sets Dxf file writer.
        /// </summary>
        public IFileWriter DxfWriter
        {
            get { return _dxfWriter; }
            set { Update(ref _dxfWriter, value); }
        }

        /// <summary>
        /// Gets or sets Csv file reader.
        /// </summary>
        public ITextFieldReader<Database> CsvReader
        {
            get { return _csvReader; }
            set { Update(ref _csvReader, value); }
        }

        /// <summary>
        /// Gets or sets Csv file writer.
        /// </summary>
        public ITextFieldWriter<Database> CsvWriter
        {
            get { return _csvWriter; }
            set { Update(ref _csvWriter, value); }
        }

        /// <summary>
        /// Gets or sets recent projects collection.
        /// </summary>
        public ImmutableArray<RecentProject> RecentProjects
        {
            get { return _recentProjects; }
            set { Update(ref _recentProjects, value); }
        }

        /// <summary>
        /// Gets or sets current recent project.
        /// </summary>
        public RecentProject CurrentRecentProject
        {
            get { return _currentRecentProject; }
            set { Update(ref _currentRecentProject, value); }
        }

        /// <summary>
        /// Initializes new <see cref="Core2D.Editor"/> instance.
        /// </summary>
        /// <param name="log">The logger object.</param>
        /// <param name="logFileName">The log file name.</param>
        /// <param name="createProject">The flag indicating whether to create project.</param>
        public void InitializeEditor(ILog log = null, string logFileName = null, bool createProject = true)
        {
            try
            {
                if (createProject)
                {
                    if (_projectFactory != null)
                    {
                        _editor = Editor.Create(_projectFactory.GetProject(), _renderers);
                    }
                    else
                    {
                        _editor = Editor.Create(Project.Create(), _renderers);
                    }
                }
                else
                {
                    _editor = Editor.Create(null, _renderers);
                }

                if (log != null && logFileName != null)
                {
                    _editor.Log = log;
                    _editor.Log.Initialize(logFileName);
                }
            }
            catch (Exception ex)
            {
                if (_editor != null && _editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
                else
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
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
        /// Performs freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        ~EditorContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.Close();
                }
            }
        }

        /// <summary>
        /// Create new project, document or container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnNew(object item)
        {
            if (item is Container)
            {
                var selected = item as Container;
                var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(selected));
                if (document != null)
                {
                    var container = default(Container);
                    if (_projectFactory != null)
                    {
                        container = _projectFactory.GetContainer(_editor.Project, Constants.DefaultContainerName);
                    }
                    else
                    {
                        container = Container.Create(Constants.DefaultContainerName);
                    }

                    if (_editor.EnableHistory)
                    {
                        var previous = document.Containers;
                        var next = document.Containers.Add(container);
                        _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                        document.Containers = next;
                    }
                    else
                    {
                        document.Containers = document.Containers.Add(container);
                    }

                    _editor.Project.CurrentContainer = container;
                }
            }
            else if (item is Document)
            {
                var selected = item as Document;

                var container = default(Container);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetContainer(_editor.Project, Constants.DefaultContainerName);
                }
                else
                {
                    container = Container.Create(Constants.DefaultContainerName);
                }

                if (_editor.EnableHistory)
                {
                    var previous = selected.Containers;
                    var next = selected.Containers.Add(container);
                    _editor.History.Snapshot(previous, next, (p) => selected.Containers = p);
                    selected.Containers = next;
                }
                else
                {
                    selected.Containers = selected.Containers.Add(container);
                }

                _editor.Project.CurrentContainer = container;
            }
            else if (item is Project)
            {
                var document = default(Document);
                if (_projectFactory != null)
                {
                    document = _projectFactory.GetDocument(_editor.Project, Constants.DefaultDocumentName);
                }
                else
                {
                    document = Document.Create(Constants.DefaultDocumentName);
                }

                if (_editor.EnableHistory)
                {
                    var previous = _editor.Project.Documents;
                    var next = _editor.Project.Documents.Add(document);
                    _editor.History.Snapshot(previous, next, (p) => _editor.Project.Documents = p);
                    _editor.Project.Documents = next;
                }
                else
                {
                    _editor.Project.Documents = _editor.Project.Documents.Add(document);
                }

                _editor.Project.CurrentDocument = document;
                _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
            }
            else if (item is EditorContext || item == null)
            {
                if (_editor.EnableHistory)
                {
                    _editor.History.Reset();
                }

                _editor.Unload();

                if (_projectFactory != null)
                {
                    _editor.Load(_projectFactory.GetProject(), string.Empty);
                }
                else
                {
                    _editor.Load(Project.Create(), string.Empty);
                }

                _editor.Invalidate();
            }
        }

        /// <summary>
        /// Close project.
        /// </summary>
        public void OnClose()
        {
            Close();
        }

        /// <summary>
        /// Close application view.
        /// </summary>
        public void OnExit()
        {
            if (_view != null)
            {
                _view.Close();
            }
        }

        /// <summary>
        /// Undo last action.
        /// </summary>
        public void OnUndo()
        {
            try
            {
                if (_editor.EnableHistory && _editor.History.CanUndo())
                {
                    _editor.Deselect();
                    _editor.History.Undo();
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Redo last action.
        /// </summary>
        public void OnRedo()
        {
            try
            {
                if (_editor.EnableHistory && _editor.History.CanRedo())
                {
                    _editor.Deselect();
                    _editor.History.Redo();
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Cut selected document, container or shapes to clipboard.
        /// </summary>
        public void OnCut()
        {
            try
            {
                if (CanCopy())
                {
                    OnCopy();

                    _editor.DeleteSelected();
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Copy document, container or shapes to clipboard.
        /// </summary>
        public void OnCopy()
        {
            try
            {
                if (CanCopy())
                {
                    if (_editor.Renderers[0].State.SelectedShape != null)
                    {
                        Copy(Enumerable.Repeat(_editor.Renderers[0].State.SelectedShape, 1).ToList());
                    }

                    if (_editor.Renderers[0].State.SelectedShapes != null)
                    {
                        Copy(_editor.Renderers[0].State.SelectedShapes.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, container or shapes.
        /// </summary>
        public async void OnPaste()
        {
            try
            {
                if (_textClipboard != null && await CanPaste())
                {
                    var json = await _textClipboard.GetText();
                    if (!string.IsNullOrEmpty(json))
                    {
                        Paste(json);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Cut selected document, container or shapes.
        /// </summary>
        /// <param name="item">The item to cut.</param>
        public void OnCut(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _containerToCopy = container;
                _documentToCopy = default(Document);
                _editor.Delete(container);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _containerToCopy = default(Container);
                _documentToCopy = document;
                _editor.Delete(document);
            }
            else if (item is EditorContext || item == null)
            {
                OnCut();
            }
        }

        /// <summary>
        /// Copy document, container or shapes to clipboard.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        public void OnCopy(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _containerToCopy = container;
                _documentToCopy = default(Document);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _containerToCopy = default(Container);
                _documentToCopy = document;
            }
            else if (item is EditorContext || item == null)
            {
                OnCopy();
            }
        }

        /// <summary>
        /// Paste text from clipboard as document, container or shapes.
        /// </summary>
        /// <param name="item">The item to paste.</param>
        public void OnPaste(object item)
        {
            if (item is Container)
            {
                if (_containerToCopy != null)
                {
                    var container = item as Container;
                    var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(container));
                    if (document != null)
                    {
                        int index = document.Containers.IndexOf(container);
                        var clone = Clone(_containerToCopy);

                        var builder = document.Containers.ToBuilder();
                        builder[index] = clone;
                        document.Containers = builder.ToImmutable();

                        if (_editor.EnableHistory)
                        {
                            var previous = document.Containers;
                            var next = builder.ToImmutable();
                            _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                            document.Containers = next;
                        }
                        else
                        {
                            document.Containers = builder.ToImmutable();
                        }

                        _editor.Project.CurrentContainer = clone;
                    }
                }
            }
            else if (item is Document)
            {
                if (_containerToCopy != null)
                {
                    var document = item as Document;
                    var clone = Clone(_containerToCopy);

                    if (_editor.EnableHistory)
                    {
                        var previous = document.Containers;
                        var next = document.Containers.Add(clone);
                        _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                        document.Containers = next;
                    }
                    else
                    {
                        document.Containers = document.Containers.Add(clone);
                    }

                    _editor.Project.CurrentContainer = clone;
                }
                else if (_documentToCopy != null)
                {
                    var document = item as Document;
                    int index = _editor.Project.Documents.IndexOf(document);
                    var clone = Clone(_documentToCopy);

                    var builder = _editor.Project.Documents.ToBuilder();
                    builder[index] = clone;

                    if (_editor.EnableHistory)
                    {
                        var previous = _editor.Project.Documents;
                        var next = builder.ToImmutable();
                        _editor.History.Snapshot(previous, next, (p) => _editor.Project.Documents = p);
                        _editor.Project.Documents = next;
                    }
                    else
                    {
                        _editor.Project.Documents = builder.ToImmutable();
                    }

                    _editor.Project.CurrentDocument = clone;
                }
            }
            else if (item is EditorContext || item == null)
            {
                OnPaste();
            }
        }

        /// <summary>
        /// Delete selected document, container or shapes.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void OnDelete(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _editor.Delete(container);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _editor.Delete(document);
            }
            else if (item is EditorContext || item == null)
            {
                _editor.DeleteSelected();
            }
        }

        /// <summary>
        /// Select all shapes.
        /// </summary>
        public void OnSelectAll()
        {
            try
            {
                _editor.Deselect(_editor.Project.CurrentContainer);
                _editor.Select(
                    _editor.Project.CurrentContainer,
                    ImmutableHashSet.CreateRange<BaseShape>(_editor.Project.CurrentContainer.CurrentLayer.Shapes));
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        public void OnDeselectAll()
        {
            try
            {
                _editor.Deselect(_editor.Project.CurrentContainer);
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        public void OnClearAll()
        {
            try
            {
                // TODO: Add history snapshot.
                _editor.Project.CurrentContainer.Clear();
                _editor.Project.CurrentContainer.Invalidate();
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Set current record as selected shape data record.
        /// </summary>
        /// <param name="item">The data record item.</param>
        public void OnApplyRecord(object item)
        {
            if (_editor.Project == null || _editor.Project.CurrentContainer == null)
                return;

            if (item is Record)
            {
                _editor.ApplyRecord(item as Record);
            }
        }

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        /// <param name="item">The shape style item.</param>
        public void OnApplyStyle(object item)
        {
            if (_editor.Project == null || _editor.Project.CurrentContainer == null)
                return;

            if (item is ShapeStyle)
            {
                _editor.ApplyStyle(item as ShapeStyle);
            }
        }

        /// <summary>
        /// Add group.
        /// </summary>
        public void OnAddGroup()
        {
            if (_renderers != null && _editor.Project == null || _editor.Project.CurrentGroupLibrary == null)
                return;

            var group = _renderers[0].State.SelectedShape;
            if (group != null && group is XGroup)
            {
                var clone = Clone(group as XGroup);
                if (clone != null)
                {
                    _editor.AddGroup(clone);
                }
            }
        }

        /// <summary>
        /// Remove group.
        /// </summary>
        public void OnRemoveGroup()
        {
            _editor.RemoveCurrentGroup();
        }

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        /// <param name="parameter">The group parameter.</param>
        public void OnInsertGroup(object parameter)
        {
            if (_editor.Project == null || _editor.Project.CurrentContainer == null)
                return;

            if (parameter is XGroup)
            {
                var group = parameter as XGroup;
                DropAsClone(group, 0.0, 0.0);
            }
        }

        /// <summary>
        /// Add template.
        /// </summary>
        public void OnAddTemplate()
        {
            if (_editor.Project == null)
                return;

            var template = default(Container);
            if (_projectFactory != null)
            {
                template = _projectFactory.GetTemplate(_editor.Project, "Empty");
            }
            else
            {
                template = Container.Create(Constants.DefaultContainerName, true);
            }

            _editor.AddTemplate(template);
        }

        /// <summary>
        /// Remove template.
        /// </summary>
        public void OnRemoveTemplate()
        {
            _editor.RemoveCurrentTemplate();
        }

        /// <summary>
        /// Edit current template.
        /// </summary>
        public void OnEditTemplate()
        {
            if (_editor.Project == null || _editor.Project.CurrentTemplate == null)
                return;

            var template = _editor.Project.CurrentTemplate;
            if (template != null)
            {
                _editor.Project.CurrentContainer = template;
                _editor.Project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// Set current template as current container's template.
        /// </summary>
        /// <param name="item">The container item.</param>
        public void OnApplyTemplate(object item)
        {
            if (_editor.Project == null || _editor.Project.CurrentContainer == null)
                return;

            if (item is Container)
            {
                _editor.ApplyTemplate(item as Container);
            }
        }

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public void OnSelectedItemChanged(object item)
        {
            if (_editor.Project == null)
                return;

            _editor.Project.Selected = item;
        }

        /// <summary>
        /// Add container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddContainer(object item)
        {
            if (_editor.Project == null || _editor.Project.CurrentDocument == null)
                return;

            var container = default(Container);
            if (_projectFactory != null)
            {
                container = _projectFactory.GetContainer(_editor.Project, Constants.DefaultContainerName);
            }
            else
            {
                container = Container.Create(Constants.DefaultContainerName);
            }

            _editor.AddContainer(container);
            _editor.Project.CurrentContainer = container;
        }

        /// <summary>
        /// Insert container before current container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertContainerBefore(object item)
        {
            if (_editor.Project == null || _editor.Project.CurrentDocument == null)
                return;

            if (item is Container)
            {
                var selected = item as Container;
                int index = _editor.Project.CurrentDocument.Containers.IndexOf(selected);

                var container = default(Container);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetContainer(_editor.Project, Constants.DefaultContainerName);
                }
                else
                {
                    container = Container.Create(Constants.DefaultContainerName);
                }

                _editor.AddContainerAt(container, index);
                _editor.Project.CurrentContainer = container;
            }
        }

        /// <summary>
        /// Insert container after current container.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertContainerAfter(object item)
        {
            if (_editor.Project == null || _editor.Project.CurrentDocument == null)
                return;

            if (item is Container)
            {
                var selected = item as Container;
                int index = _editor.Project.CurrentDocument.Containers.IndexOf(selected);

                var container = default(Container);
                if (_projectFactory != null)
                {
                    container = _projectFactory.GetContainer(_editor.Project, Constants.DefaultContainerName);
                }
                else
                {
                    container = Container.Create(Constants.DefaultContainerName);
                }

                _editor.AddContainerAt(container, index + 1);
                _editor.Project.CurrentContainer = container;
            }
        }

        /// <summary>
        /// Add document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnAddDocument(object item)
        {
            if (_editor.Project == null)
                return;

            var document = default(Document);
            if (_projectFactory != null)
            {
                document = _projectFactory.GetDocument(_editor.Project, Constants.DefaultDocumentName);
            }
            else
            {
                document = Document.Create(Constants.DefaultDocumentName);
            }

            _editor.AddDocument(document);
            _editor.Project.CurrentDocument = document;
            _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
        }

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentBefore(object item)
        {
            if (_editor.Project == null)
                return;

            if (item is Document)
            {
                var selected = item as Document;
                int index = _editor.Project.Documents.IndexOf(selected);

                var document = default(Document);
                if (_projectFactory != null)
                {
                    document = _projectFactory.GetDocument(_editor.Project, Constants.DefaultDocumentName);
                }
                else
                {
                    document = Document.Create(Constants.DefaultDocumentName);
                }

                _editor.AddDocumentAt(document, index);
                _editor.Project.CurrentDocument = document;
                _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
            }
        }

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        public void OnInsertDocumentAfter(object item)
        {
            if (_editor.Project == null)
                return;

            if (item is Document)
            {
                var selected = item as Document;
                int index = _editor.Project.Documents.IndexOf(selected);

                var document = default(Document);
                if (_projectFactory != null)
                {
                    document = _projectFactory.GetDocument(_editor.Project, Constants.DefaultDocumentName);
                }
                else
                {
                    document = Document.Create(Constants.DefaultDocumentName);
                }

                _editor.AddDocumentAt(document, index + 1);
                _editor.Project.CurrentDocument = document;
                _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.None"/>.
        /// </summary>
        public void OnToolNone()
        {
            _editor.CurrentTool = Tool.None;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Selection"/>.
        /// </summary>
        public void OnToolSelection()
        {
            _editor.CurrentTool = Tool.Selection;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Point"/>.
        /// </summary>
        public void OnToolPoint()
        {
            _editor.CurrentTool = Tool.Point;
        }

        /// <summary>
        ///  Set current tool to <see cref="Tool.Line"/> or current path tool to <see cref="PathTool.Line"/>.
        /// </summary>
        public void OnToolLine()
        {
            if (_editor.CurrentTool == Tool.Path && _editor.CurrentPathTool != PathTool.Line)
            {
                _editor.CurrentPathTool = PathTool.Line;
            }
            else
            {
                _editor.CurrentTool = Tool.Line;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Arc"/> or current path tool to <see cref="PathTool.Arc"/>.
        /// </summary>
        public void OnToolArc()
        {
            if (_editor.CurrentTool == Tool.Path && _editor.CurrentPathTool != PathTool.Arc)
            {
                _editor.CurrentPathTool = PathTool.Arc;
            }
            else
            {
                _editor.CurrentTool = Tool.Arc;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Bezier"/> or current path tool to <see cref="PathTool.Bezier"/>.
        /// </summary>
        public void OnToolBezier()
        {
            if (_editor.CurrentTool == Tool.Path && _editor.CurrentPathTool != PathTool.Bezier)
            {
                _editor.CurrentPathTool = PathTool.Bezier;
            }
            else
            {
                _editor.CurrentTool = Tool.Bezier;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.QBezier"/> or current path tool to <see cref="PathTool.QBezier"/>.
        /// </summary>
        public void OnToolQBezier()
        {
            if (_editor.CurrentTool == Tool.Path && _editor.CurrentPathTool != PathTool.QBezier)
            {
                _editor.CurrentPathTool = PathTool.QBezier;
            }
            else
            {
                _editor.CurrentTool = Tool.QBezier;
            }
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Path"/>.
        /// </summary>
        public void OnToolPath()
        {
            _editor.CurrentTool = Tool.Path;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Rectangle"/>.
        /// </summary>
        public void OnToolRectangle()
        {
            _editor.CurrentTool = Tool.Rectangle;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Ellipse"/>.
        /// </summary>
        public void OnToolEllipse()
        {
            _editor.CurrentTool = Tool.Ellipse;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Text"/>.
        /// </summary>
        public void OnToolText()
        {
            _editor.CurrentTool = Tool.Text;
        }

        /// <summary>
        /// Set current tool to <see cref="Tool.Image"/>.
        /// </summary>
        public void OnToolImage()
        {
            _editor.CurrentTool = Tool.Image;
        }

        /// <summary>
        /// Set current path tool to <see cref="PathTool.Move"/>.
        /// </summary>
        public void OnToolMove()
        {
            if (_editor.CurrentTool == Tool.Path && _editor.CurrentPathTool != PathTool.Move)
            {
                _editor.CurrentPathTool = PathTool.Move;
            }
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsStroked"/> option.
        /// </summary>
        public void OnToggleDefaultIsStroked()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsStroked = !_editor.Project.Options.DefaultIsStroked;
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsFilled"/> option.
        /// </summary>
        public void OnToggleDefaultIsFilled()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsFilled = !_editor.Project.Options.DefaultIsFilled;
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsClosed"/> option.
        /// </summary>
        public void OnToggleDefaultIsClosed()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsClosed = !_editor.Project.Options.DefaultIsClosed;
        }

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsSmoothJoin"/> option.
        /// </summary>
        public void OnToggleDefaultIsSmoothJoin()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsSmoothJoin = !_editor.Project.Options.DefaultIsSmoothJoin;
        }

        /// <summary>
        /// Toggle <see cref="Options.SnapToGrid"/> option.
        /// </summary>
        public void OnToggleSnapToGrid()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.SnapToGrid = !_editor.Project.Options.SnapToGrid;
        }

        /// <summary>
        /// Toggle <see cref="Options.TryToConnect"/> option.
        /// </summary>
        public void OnToggleTryToConnect()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.TryToConnect = !_editor.Project.Options.TryToConnect;
        }

        /// <summary>
        /// Invalidates renderer's cache.
        /// </summary>
        /// <param name="isZooming">The flag indicating whether is zooming.</param>
        public void InvalidateCache(bool isZooming)
        {
            try
            {
                foreach (var renderer in _editor.Renderers)
                {
                    renderer.ClearCache(isZooming);
                }

                _editor.Project.CurrentContainer.Invalidate();
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void Open(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                var project = Project.Open(path, _serializer);

                if (_editor.EnableHistory)
                {
                    _editor.History.Reset();
                }

                _editor.Unload();
                _editor.Load(project, path);

                AddRecent(path, project.Name);
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Save project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        public void Save(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                Project.Save(_editor.Project, path, _serializer);

                AddRecent(path, _editor.Project.Name);

                if (string.IsNullOrEmpty(_editor.ProjectPath))
                {
                    _editor.ProjectPath = path;
                }

                _editor.IsProjectDirty = false;
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Close project.
        /// </summary>
        public void Close()
        {
            if (_editor.EnableHistory)
            {
                _editor.History.Reset();
            }

            _editor.Unload();
        }

        /// <summary>
        /// Export item as Pdf.
        /// </summary>
        /// <param name="path">The Pdf file path.</param>
        /// <param name="item">The item to export.</param>
        public void ExportAsPdf(string path, object item)
        {
            try
            {
                if (_pdfWriter != null)
                {
                    _pdfWriter.Save(path, item, _editor.Project);
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export current container as Dxf.
        /// </summary>
        /// <param name="path">The Dxf file path.</param>
        public void ExportAsDxf(string path)
        {
            try
            {
                if (_dxfWriter != null)
                {
                    _dxfWriter.Save(path, _editor.Project.CurrentContainer, _editor.Project);
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Import object from file.
        /// </summary>
        /// <param name="path">The object file path.</param>
        /// <param name="item">The parent object.</param>
        /// <param name="type">The object type.</param>
        public void ImportObject(string path, object item, ImportType type)
        {
            if (_serializer == null)
                return;

            try
            {
                switch (type)
                {
                    case ImportType.Style:
                        {
                            var sg = item as Library<ShapeStyle>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<ShapeStyle>(json);

                            if (_editor.EnableHistory)
                            {
                                var previous = sg.Items;
                                var next = sg.Items.Add(import);
                                _editor.History.Snapshot(previous, next, (p) => sg.Items = p);
                                sg.Items = next;
                            }
                            else
                            {
                                sg.Items = sg.Items.Add(import);
                            }
                        }
                        break;
                    case ImportType.Styles:
                        {
                            var sg = item as Library<ShapeStyle>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<ShapeStyle>>(json);

                            var builder = sg.Items.ToBuilder();
                            foreach (var style in import)
                            {
                                builder.Add(style);
                            }

                            if (_editor.EnableHistory)
                            {
                                var previous = sg.Items;
                                var next = builder.ToImmutable();
                                _editor.History.Snapshot(previous, next, (p) => sg.Items = p);
                                sg.Items = next;
                            }
                            else
                            {
                                sg.Items = builder.ToImmutable();
                            }
                        }
                        break;
                    case ImportType.StyleLibrary:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<Library<ShapeStyle>>(json);

                            if (_editor.EnableHistory)
                            {
                                var previous = project.StyleLibraries;
                                var next = project.StyleLibraries.Add(import);
                                _editor.History.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                                project.StyleLibraries = next;
                            }
                            else
                            {
                                project.StyleLibraries = project.StyleLibraries.Add(import);
                            }
                        }
                        break;
                    case ImportType.StyleLibraries:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<Library<ShapeStyle>>>(json);

                            var builder = project.StyleLibraries.ToBuilder();
                            foreach (var sg in import)
                            {
                                builder.Add(sg);
                            }

                            if (_editor.EnableHistory)
                            {
                                var previous = project.StyleLibraries;
                                var next = builder.ToImmutable();
                                _editor.History.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                                project.StyleLibraries = next;
                            }
                            else
                            {
                                project.StyleLibraries = builder.ToImmutable();
                            }
                        }
                        break;
                    case ImportType.Group:
                        {
                            var gl = item as Library<XGroup>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<XGroup>(json);

                            var shapes = Enumerable.Repeat(import as XGroup, 1);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            if (_editor.EnableHistory)
                            {
                                var previous = gl.Items;
                                var next = gl.Items.Add(import);
                                _editor.History.Snapshot(previous, next, (p) => gl.Items = p);
                                gl.Items = next;
                            }
                            else
                            {
                                gl.Items = gl.Items.Add(import);
                            }
                        }
                        break;
                    case ImportType.Groups:
                        {
                            var gl = item as Library<XGroup>;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<XGroup>>(json);

                            var shapes = import;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var builder = gl.Items.ToBuilder();
                            foreach (var group in import)
                            {
                                builder.Add(group);
                            }

                            if (_editor.EnableHistory)
                            {
                                var previous = gl.Items;
                                var next = builder.ToImmutable();
                                _editor.History.Snapshot(previous, next, (p) => gl.Items = p);
                                gl.Items = next;
                            }
                            else
                            {
                                gl.Items = builder.ToImmutable();
                            }
                        }
                        break;
                    case ImportType.GroupLibrary:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<Library<XGroup>>(json);

                            var shapes = import.Items;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            if (_editor.EnableHistory)
                            {
                                var previous = project.GroupLibraries;
                                var next = project.GroupLibraries.Add(import);
                                _editor.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                                project.GroupLibraries = next;
                            }
                            else
                            {
                                project.GroupLibraries = project.GroupLibraries.Add(import);
                            }
                        }
                        break;
                    case ImportType.GroupLibraries:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<Library<XGroup>>>(json);

                            var shapes = import.SelectMany(x => x.Items);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var builder = project.GroupLibraries.ToBuilder();
                            foreach (var library in import)
                            {
                                builder.Add(library);
                            }

                            if (_editor.EnableHistory)
                            {
                                var previous = project.GroupLibraries;
                                var next = builder.ToImmutable();
                                _editor.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                                project.GroupLibraries = next;
                            }
                            else
                            {
                                project.GroupLibraries = builder.ToImmutable();
                            }
                        }
                        break;
                    case ImportType.Template:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<Container>(json);

                            var shapes = import.Layers.SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            if (_editor.EnableHistory)
                            {
                                var previous = project.Templates;
                                var next = project.Templates.Add(import);
                                _editor.History.Snapshot(previous, next, (p) => project.Templates = p);
                                project.Templates = next;
                            }
                            else
                            {
                                project.Templates = project.Templates.Add(import);
                            }
                        }
                        break;
                    case ImportType.Templates:
                        {
                            var project = item as Project;
                            var json = Project.ReadUtf8Text(path);
                            var import = _serializer.Deserialize<IList<Container>>(json);

                            var shapes = import.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var builder = project.Templates.ToBuilder();
                            foreach (var template in import)
                            {
                                builder.Add(template);
                            }

                            if (_editor.EnableHistory)
                            {
                                var previous = project.Templates;
                                var next = builder.ToImmutable();
                                _editor.History.Snapshot(previous, next, (p) => project.Templates = p);
                                project.Templates = next;
                            }
                            else
                            {
                                project.Templates = builder.ToImmutable();
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export object to a file.
        /// </summary>
        /// <param name="path">The object file path.</param>
        /// <param name="item">The parent object.</param>
        /// <param name="type">The object type.</param>
        public void ExportObject(string path, object item, ExportType type)
        {
            if (_serializer == null)
                return;

            try
            {
                switch (type)
                {
                    case ExportType.Style:
                        {
                            var json = _serializer.Serialize(item as ShapeStyle);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Styles:
                        {
                            var json = _serializer.Serialize((item as Library<ShapeStyle>).Items);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.StyleLibrary:
                        {
                            var json = _serializer.Serialize((item as Library<ShapeStyle>));
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.StyleLibraries:
                        {
                            var json = _serializer.Serialize((item as Project).StyleLibraries);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Group:
                        {
                            var json = _serializer.Serialize(item as XGroup);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Groups:
                        {
                            var json = _serializer.Serialize((item as Library<XGroup>).Items);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.GroupLibrary:
                        {
                            var json = _serializer.Serialize(item as Library<XGroup>);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.GroupLibraries:
                        {
                            var json = _serializer.Serialize((item as Project).GroupLibraries);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Template:
                        {
                            var json = _serializer.Serialize(item as Container);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                    case ExportType.Templates:
                        {
                            var json = _serializer.Serialize((item as Project).Templates);
                            Project.WriteUtf8Text(path, json);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        public void ImportData(string path)
        {
            if (_editor.Project == null)
                return;

            try
            {
                if (_csvReader == null)
                    return;

                var db = _csvReader.Read(path);
                _editor.AddDatabase(db);
                _editor.Project.CurrentDatabase = db;
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void ExportData(string path, Database database)
        {
            try
            {
                if (_csvWriter == null)
                    return;

                _csvWriter.Write(path, database);
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        public void UpdateData(string path, Database database)
        {
            try
            {
                if (_csvReader == null)
                    return;

                var db = _csvReader.Read(path);
                _editor.ApplyDatabase(database, db);
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Add recent project file.
        /// </summary>
        /// <param name="path">The project path.</param>
        /// <param name="name">The project name.</param>
        private void AddRecent(string path, string name)
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

            builder.Insert(0, RecentProject.Create(name, path));

            RecentProjects = builder.ToImmutable();
            CurrentRecentProject = _recentProjects.FirstOrDefault();
        }

        /// <summary>
        /// Load recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void LoadRecent(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                var json = Project.ReadUtf8Text(path);
                var recent = _serializer.Deserialize<Recent>(json);

                if (recent != null)
                {
                    var remove = recent.RecentProjects.Where(x => System.IO.File.Exists(x.Path) == false).ToList();
                    var builder = recent.RecentProjects.ToBuilder();

                    foreach (var file in remove)
                    {
                        builder.Remove(file);
                    }

                    RecentProjects = builder.ToImmutable();

                    if (recent.CurrentRecentProject != null
                        && System.IO.File.Exists(recent.CurrentRecentProject.Path))
                    {
                        CurrentRecentProject = recent.CurrentRecentProject;
                    }
                    else
                    {
                        CurrentRecentProject = _recentProjects.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Save recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        public void SaveRecent(string path)
        {
            if (_serializer == null)
                return;

            try
            {
                var recent = Recent.Create(_recentProjects, _currentRecentProject);
                var json = _serializer.Serialize(recent);
                Project.WriteUtf8Text(path, json);
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Checks if can copy.
        /// </summary>
        /// <returns>Returns true if can copy.</returns>
        public bool CanCopy()
        {
            return _editor.IsSelectionAvailable();
        }

        /// <summary>
        /// Checks if can paste.
        /// </summary>
        /// <returns>Returns true if can paste.</returns>
        public async Task<bool> CanPaste()
        {
            try
            {
                return _textClipboard != null && await _textClipboard.ContainsText();
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
            return false;
        }

        /// <summary>
        /// Copy selected shapes to clipboard.
        /// </summary>
        /// <param name="shapes"></param>
        private void Copy(IList<BaseShape> shapes)
        {
            try
            {
                if (_textClipboard != null && _serializer != null)
                {
                    var json = _serializer.Serialize(shapes);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _textClipboard.SetText(json);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Paste Json text from clipboard as shapes.
        /// </summary>
        /// <param name="json"></param>
        public void Paste(string json)
        {
            if (_serializer == null)
                return;

            try
            {
                var shapes = _serializer.Deserialize<IList<BaseShape>>(json);
                if (shapes != null && shapes.Count() > 0)
                {
                    Paste(shapes);
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Try to restore shapes styles.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreStyles(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (_editor.Project.StyleLibraries == null)
                    return;

                var styles = _editor.Project.StyleLibraries
                    .Where(sg => sg.Items != null && sg.Items.Length > 0)
                    .SelectMany(sg => sg.Items)
                    .Distinct(new StyleComparer())
                    .ToDictionary(s => s.Name);

                // Reset point shape to container default.
                foreach (var point in Editor.GetAllPoints(shapes, ShapeStateFlags.Connector))
                {
                    point.Shape = _editor.Project.Options.PointShape;
                }

                // Try to restore shape styles.
                foreach (var shape in Editor.GetAllShapes(shapes))
                {
                    if (shape.Style == null)
                        continue;

                    ShapeStyle style;
                    if (styles.TryGetValue(shape.Style.Name, out style))
                    {
                        // Use existing style.
                        shape.Style = style;
                    }
                    else
                    {
                        // Create Imported style library.
                        if (_editor.Project.CurrentStyleLibrary == null)
                        {
                            var sg = Library<ShapeStyle>.Create(Constants.ImportedStyleLibraryName);
                            _editor.Project.StyleLibraries = _editor.Project.StyleLibraries.Add(sg);
                            _editor.Project.CurrentStyleLibrary = sg;
                        }

                        // Add missing style.
                        _editor.Project.CurrentStyleLibrary.Items = _editor.Project.CurrentStyleLibrary.Items.Add(shape.Style);

                        // Recreate styles dictionary.
                        styles = _editor.Project.StyleLibraries
                            .Where(sg => sg.Items != null && sg.Items.Length > 0)
                            .SelectMany(sg => sg.Items)
                            .Distinct(new StyleComparer())
                            .ToDictionary(s => s.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Try to restore shapes records.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        private void TryToRestoreRecords(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (_editor.Project.Databases == null)
                    return;

                var records = _editor.Project.Databases
                    .Where(d => d.Records != null && d.Records.Length > 0)
                    .SelectMany(d => d.Records)
                    .ToDictionary(s => s.Id);

                // Try to restore shape record.
                foreach (var shape in Editor.GetAllShapes(shapes))
                {
                    if (shape.Data.Record == null)
                        continue;

                    Record record;
                    if (records.TryGetValue(shape.Data.Record.Id, out record))
                    {
                        // Use existing record.
                        shape.Data.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (_editor.Project.CurrentDatabase == null)
                        {
                            var db = Database.Create(Constants.ImportedDatabaseName, shape.Data.Record.Columns);
                            _editor.Project.Databases = _editor.Project.Databases.Add(db);
                            _editor.Project.CurrentDatabase = db;
                        }

                        // Add missing data record.
                        _editor.Project.CurrentDatabase.Records = _editor.Project.CurrentDatabase.Records.Add(shape.Data.Record);

                        // Recreate records dictionary.
                        records = _editor.Project.Databases
                            .Where(d => d.Records != null && d.Records.Length > 0)
                            .SelectMany(d => d.Records)
                            .ToDictionary(s => s.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Paste shapes to current container.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        public void Paste(IEnumerable<BaseShape> shapes)
        {
            try
            {
                _editor.Deselect(_editor.Project.CurrentContainer);

                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);

                var layer = _editor.Project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in shapes)
                {
                    builder.Add(shape);
                }

                if (_editor.EnableHistory)
                {
                    var previous = layer.Shapes;
                    var next = builder.ToImmutable();
                    _editor.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;
                }
                else
                {
                    layer.Shapes = builder.ToImmutable();
                }

                if (shapes.Count() == 1)
                {
                    _editor.Select(_editor.Project.CurrentContainer, shapes.FirstOrDefault());
                }
                else
                {
                    _editor.Select(_editor.Project.CurrentContainer, ImmutableHashSet.CreateRange<BaseShape>(shapes));
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Clones the <see cref="XGroup"/> object.
        /// </summary>
        /// <param name="group">The <see cref="XGroup"/> object.</param>
        /// <returns>The cloned <see cref="XGroup"/> object.</returns>
        public XGroup Clone(XGroup group)
        {
            if (_serializer == null)
                return null;

            try
            {
                var json = _serializer.Serialize(group);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _serializer.Deserialize<XGroup>(json);
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
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return null;
        }

        /// <summary>
        /// Clones the <see cref="Container"/> object.
        /// </summary>
        /// <param name="container">The <see cref="Container"/> object.</param>
        /// <returns>The cloned <see cref="Container"/> object.</returns>
        public Container Clone(Container container)
        {
            if (_serializer == null)
                return null;

            try
            {
                var template = container.Template;
                var json = _serializer.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _serializer.Deserialize<Container>(json);
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
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return null;
        }

        /// <summary>
        /// Clones the <see cref="Document"/> object.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> object.</param>
        /// <returns>The cloned <see cref="Document"/> object.</returns>
        public Document Clone(Document document)
        {
            if (_serializer == null)
                return null;

            try
            {
                var templates = document.Containers.Select(c => c.Template).ToArray();
                var json = _serializer.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = _serializer.Deserialize<Document>(json);
                    if (clone != null)
                    {
                        for (int i = 0; i < clone.Containers.Length; i++)
                        {
                            var container = clone.Containers[i];
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
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return null;
        }

        /// <summary>
        /// Process dropped files.
        /// </summary>
        /// <param name="files">The files array.</param>
        /// <returns>Returns true if success.</returns>
        public bool Drop(string[] files)
        {
            try
            {
                if (files != null && files.Length >= 1)
                {
                    bool result = false;
                    foreach (var path in files)
                    {
                        if (string.IsNullOrEmpty(path))
                            continue;

                        string ext = System.IO.Path.GetExtension(path);

                        if (string.Compare(ext, Constants.ProjectExtension, true) == 0)
                        {
                            Open(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.CsvExtension, true) == 0)
                        {
                            ImportData(path);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StyleExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project.CurrentStyleLibrary, ImportType.Style);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StylesExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project.CurrentStyleLibrary, ImportType.Styles);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StyleLibraryExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project, ImportType.StyleLibrary);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.StyleLibrariesExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project, ImportType.StyleLibraries);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project.CurrentGroupLibrary, ImportType.Group);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupsExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project.CurrentGroupLibrary, ImportType.Groups);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupLibraryExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project, ImportType.GroupLibrary);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.GroupLibrariesExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project, ImportType.GroupLibraries);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.TemplateExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project, ImportType.Template);
                            result = true;
                        }
                        else if (string.Compare(ext, Constants.TemplatesExtension, true) == 0)
                        {
                            ImportObject(path, _editor.Project, ImportType.Templates);
                            result = true;
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Drop <see cref="XGroup"/> object in current container at specified location.
        /// </summary>
        /// <param name="group">The <see cref="XGroup"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void DropAsClone(XGroup group, double x, double y)
        {
            try
            {
                double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
                double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;

                var clone = Clone(group);
                if (clone != null)
                {
                    _editor.Deselect(_editor.Project.CurrentContainer);
                    clone.Move(sx, sy);

                    _editor.AddShape(clone);

                    _editor.Select(_editor.Project.CurrentContainer, clone);

                    if (_editor.Project.Options.TryToConnect)
                    {
                        _editor.TryToConnect(clone);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Drop <see cref="Record"/> object in current container at specified location.
        /// </summary>
        /// <param name="record">The <see cref="Record"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void Drop(Record record, double x, double y)
        {
            try
            {
                if (_editor.Renderers[0].State.SelectedShape != null)
                {
                    _editor.ApplyRecord(_editor.Renderers[0].State.SelectedShape, record);
                }
                else if (_editor.Renderers[0].State.SelectedShapes != null && _editor.Renderers[0].State.SelectedShapes.Count > 0)
                {
                    foreach (var shape in _editor.Renderers[0].State.SelectedShapes)
                    {
                        _editor.ApplyRecord(shape, record);
                    }
                }
                else
                {
                    var container = _editor.Project.CurrentContainer;
                    if (container != null)
                    {
                        var result = ShapeBounds.HitTest(container, new Vector2(x, y), _editor.Project.Options.HitTreshold);
                        if (result != null)
                        {
                            _editor.ApplyRecord(result, record);
                        }
                        else
                        {
                            DropAsGroup(record, x, y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Drop <see cref="Record"/> object in current container at specified location as group bound to this record.
        /// </summary>
        /// <param name="record">The <see cref="Record"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void DropAsGroup(Record record, double x, double y)
        {
            var g = XGroup.Create("g");
            g.Data.Record = record;

            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;

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

                    var text = XText.Create(
                        px, py,
                        px + width,
                        py + height,
                        _editor.Project.CurrentStyleLibrary.Selected,
                        _editor.Project.Options.PointShape,
                        binding);

                    g.AddShape(text);

                    py += height;
                }
            }

            var rectangle = XRectangle.Create(
                sx, sy,
                sx + width, sy + (double)length * height,
                _editor.Project.CurrentStyleLibrary.Selected,
                _editor.Project.Options.PointShape);
            g.AddShape(rectangle);

            double ptx = sx + width / 2;
            double pty = sy;

            double pbx = sx + width / 2;
            double pby = sy + (double)length * height;

            double plx = sx;
            double ply = sy + ((double)length * height) / 2;

            double prx = sx + width;
            double pry = sy + ((double)length * height) / 2;

            var pt = XPoint.Create(ptx, pty, _editor.Project.Options.PointShape);
            var pb = XPoint.Create(pbx, pby, _editor.Project.Options.PointShape);
            var pl = XPoint.Create(plx, ply, _editor.Project.Options.PointShape);
            var pr = XPoint.Create(prx, pry, _editor.Project.Options.PointShape);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            _editor.AddShape(g);
        }

        /// <summary>
        /// Drop <see cref="ShapeStyle"/> object in current container at specified location.
        /// </summary>
        /// <param name="style">The <see cref="ShapeStyle"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        public void Drop(ShapeStyle style, double x, double y)
        {
            try
            {
                if (_editor.Renderers[0].State.SelectedShape != null
                    || (_editor.Renderers[0].State.SelectedShapes != null && _editor.Renderers[0].State.SelectedShapes.Count > 0))
                {
                    _editor.ApplyStyle(style);
                }
                else
                {
                    _editor.ApplyStyle(style, x, y);
                }
            }
            catch (Exception ex)
            {
                if (_editor.Log != null)
                {
                    _editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Checks if can undo.
        /// </summary>
        /// <returns>Returns true if can undo.</returns>
        public bool CanUndo()
        {
            return _editor.EnableHistory && _editor.History.CanUndo();
        }

        /// <summary>
        /// Checks if can redo.
        /// </summary>
        /// <returns>Returns true if can redo.</returns>
        public bool CanRedo()
        {
            return _editor.EnableHistory && _editor.History.CanRedo();
        }
    }
}
