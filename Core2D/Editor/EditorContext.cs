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
        /// 
        /// </summary>
        public Commands Commands
        {
            get { return _commands; }
            set { Update(ref _commands, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public Editor Editor
        {
            get { return _editor; }
            set { Update(ref _editor, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IView View
        {
            get { return _view; }
            set { Update(ref _view, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IRenderer[] Renderers
        {
            get { return _renderers; }
            set { Update(ref _renderers, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IProjectFactory ProjectFactory
        {
            get { return _projectFactory; }
            set { Update(ref _projectFactory, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ITextClipboard TextClipboard
        {
            get { return _textClipboard; }
            set { Update(ref _textClipboard, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ISerializer Serializer
        {
            get { return _serializer; }
            set { Update(ref _serializer, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IFileWriter PdfWriter
        {
            get { return _pdfWriter; }
            set { Update(ref _pdfWriter, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IFileWriter DxfWriter
        {
            get { return _dxfWriter; }
            set { Update(ref _dxfWriter, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ITextFieldReader<Database> CsvReader
        {
            get { return _csvReader; }
            set { Update(ref _csvReader, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ITextFieldWriter<Database> CsvWriter
        {
            get { return _csvWriter; }
            set { Update(ref _csvWriter, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ImmutableArray<RecentProject> RecentProjects
        {
            get { return _recentProjects; }
            set { Update(ref _recentProjects, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public RecentProject CurrentRecentProject
        {
            get { return _currentRecentProject; }
            set { Update(ref _currentRecentProject, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="logFileName"></param>
        /// <param name="createProject"></param>
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
        ///
        /// </summary>
        /// <returns></returns>
        public bool IsEditMode()
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        ~EditorContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        public void OnClose()
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnExit()
        {
            if (_view != null)
            {
                _view.Close();
            }
        }

        /// <summary>
        /// 
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
        /// 
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
        ///
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
        ///
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
        ///
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        ///
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
        /// 
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
        ///
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
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
        /// 
        /// </summary>
        public void OnRemoveGroup()
        {
            _editor.RemoveCurrentGroup();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public void OnRemoveTemplate()
        {
            _editor.RemoveCurrentTemplate();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void OnSelectedItemChanged(object item)
        {
            if (_editor.Project == null)
                return;

            _editor.Project.Selected = item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        /// <param name="item"></param>
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
        /// 
        /// </summary>
        public void OnToolNone()
        {
            _editor.CurrentTool = Tool.None;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToolSelection()
        {
            _editor.CurrentTool = Tool.Selection;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToolPoint()
        {
            _editor.CurrentTool = Tool.Point;
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        public void OnToolPath()
        {
            _editor.CurrentTool = Tool.Path;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToolRectangle()
        {
            _editor.CurrentTool = Tool.Rectangle;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToolEllipse()
        {
            _editor.CurrentTool = Tool.Ellipse;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToolText()
        {
            _editor.CurrentTool = Tool.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToolImage()
        {
            _editor.CurrentTool = Tool.Image;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToolMove()
        {
            if (_editor.CurrentTool == Tool.Path && _editor.CurrentPathTool != PathTool.Move)
            {
                _editor.CurrentPathTool = PathTool.Move;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToggleDefaultIsStroked()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsStroked = !_editor.Project.Options.DefaultIsStroked;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToggleDefaultIsFilled()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsFilled = !_editor.Project.Options.DefaultIsFilled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToggleDefaultIsClosed()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsClosed = !_editor.Project.Options.DefaultIsClosed;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToggleDefaultIsSmoothJoin()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.DefaultIsSmoothJoin = !_editor.Project.Options.DefaultIsSmoothJoin;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToggleSnapToGrid()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.SnapToGrid = !_editor.Project.Options.SnapToGrid;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnToggleTryToConnect()
        {
            if (_editor.Project == null || _editor.Project.Options == null)
                return;

            _editor.Project.Options.TryToConnect = !_editor.Project.Options.TryToConnect;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isZooming"></param>
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
        ///
        /// </summary>
        /// <param name="path"></param>
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
        ///
        /// </summary>
        /// <param name="path"></param>
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
        /// 
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
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
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
        ///
        /// </summary>
        /// <param name="path"></param>
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
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
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
                            var sg = item as StyleLibrary;
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<ShapeStyle>(json);

                            if (_editor.EnableHistory)
                            {
                                var previous = sg.Styles;
                                var next = sg.Styles.Add(import);
                                _editor.History.Snapshot(previous, next, (p) => sg.Styles = p);
                                sg.Styles = next;
                            }
                            else
                            {
                                sg.Styles = sg.Styles.Add(import);
                            }
                        }
                        break;
                    case ImportType.Styles:
                        {
                            var sg = item as StyleLibrary;
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<IList<ShapeStyle>>(json);

                            var builder = sg.Styles.ToBuilder();
                            foreach (var style in import)
                            {
                                builder.Add(style);
                            }

                            if (_editor.EnableHistory)
                            {
                                var previous = sg.Styles;
                                var next = builder.ToImmutable();
                                _editor.History.Snapshot(previous, next, (p) => sg.Styles = p);
                                sg.Styles = next;
                            }
                            else
                            {
                                sg.Styles = builder.ToImmutable();
                            }
                        }
                        break;
                    case ImportType.StyleLibrary:
                        {
                            var project = item as Project;
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<StyleLibrary>(json);

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
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<IList<StyleLibrary>>(json);

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
                            var gl = item as GroupLibrary;
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<XGroup>(json);

                            var shapes = Enumerable.Repeat(import as XGroup, 1);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            if (_editor.EnableHistory)
                            {
                                var previous = gl.Groups;
                                var next = gl.Groups.Add(import);
                                _editor.History.Snapshot(previous, next, (p) => gl.Groups = p);
                                gl.Groups = next;
                            }
                            else
                            {
                                gl.Groups = gl.Groups.Add(import);
                            }
                        }
                        break;
                    case ImportType.Groups:
                        {
                            var gl = item as GroupLibrary;
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<IList<XGroup>>(json);

                            var shapes = import;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);

                            var builder = gl.Groups.ToBuilder();
                            foreach (var group in import)
                            {
                                builder.Add(group);
                            }

                            if (_editor.EnableHistory)
                            {
                                var previous = gl.Groups;
                                var next = builder.ToImmutable();
                                _editor.History.Snapshot(previous, next, (p) => gl.Groups = p);
                                gl.Groups = next;
                            }
                            else
                            {
                                gl.Groups = builder.ToImmutable();
                            }
                        }
                        break;
                    case ImportType.GroupLibrary:
                        {
                            var project = item as Project;
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<GroupLibrary>(json);

                            var shapes = import.Groups;
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
                            var json = Utf8TextFile.Read(path);
                            var import = _serializer.Deserialize<IList<GroupLibrary>>(json);

                            var shapes = import.SelectMany(x => x.Groups);
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
                            var json = Utf8TextFile.Read(path);
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
                            var json = Utf8TextFile.Read(path);
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
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
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
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.Styles:
                        {
                            var json = _serializer.Serialize((item as StyleLibrary).Styles);
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.StyleLibrary:
                        {
                            var json = _serializer.Serialize((item as StyleLibrary));
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.StyleLibraries:
                        {
                            var json = _serializer.Serialize((item as Project).StyleLibraries);
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.Group:
                        {
                            var json = _serializer.Serialize(item as XGroup);
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.Groups:
                        {
                            var json = _serializer.Serialize((item as GroupLibrary).Groups);
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.GroupLibrary:
                        {
                            var json = _serializer.Serialize(item as GroupLibrary);
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.GroupLibraries:
                        {
                            var json = _serializer.Serialize((item as Project).GroupLibraries);
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.Template:
                        {
                            var json = _serializer.Serialize(item as Container);
                            Utf8TextFile.Write(path, json);
                        }
                        break;
                    case ExportType.Templates:
                        {
                            var json = _serializer.Serialize((item as Project).Templates);
                            Utf8TextFile.Write(path, json);
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
        ///
        /// </summary>
        /// <param name="path"></param>
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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="database"></param>
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
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="database"></param>
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
                var json = Utf8TextFile.Read(path);
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
                Utf8TextFile.Write(path, json);
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
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanCopy()
        {
            return _editor.IsSelectionAvailable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
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
        ///
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
        ///
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
        ///
        /// </summary>
        /// <param name="shapes"></param>
        private void TryToRestoreStyles(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (_editor.Project.StyleLibraries == null)
                    return;

                var styles = _editor.Project.StyleLibraries
                    .Where(sg => sg.Styles != null && sg.Styles.Length > 0)
                    .SelectMany(sg => sg.Styles)
                    .Distinct(new StyleComparer())
                    .ToDictionary(s => s.Name);

                // reset point shape to container default
                foreach (var point in Editor.GetAllPoints(shapes, ShapeStateFlags.Connector))
                {
                    point.Shape = _editor.Project.Options.PointShape;
                }

                // try to restore shape styles
                foreach (var shape in Editor.GetAllShapes(shapes))
                {
                    if (shape.Style == null)
                        continue;

                    ShapeStyle style;
                    if (styles.TryGetValue(shape.Style.Name, out style))
                    {
                        // use existing style
                        shape.Style = style;
                    }
                    else
                    {
                        // create Imported style library
                        if (_editor.Project.CurrentStyleLibrary == null)
                        {
                            var sg = StyleLibrary.Create(Constants.ImportedStyleLibraryName);
                            _editor.Project.StyleLibraries = _editor.Project.StyleLibraries.Add(sg);
                            _editor.Project.CurrentStyleLibrary = sg;
                        }

                        // add missing style
                        _editor.Project.CurrentStyleLibrary.Styles = _editor.Project.CurrentStyleLibrary.Styles.Add(shape.Style);

                        // recreate styles dictionary
                        styles = _editor.Project.StyleLibraries
                            .Where(sg => sg.Styles != null && sg.Styles.Length > 0)
                            .SelectMany(sg => sg.Styles)
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
        ///
        /// </summary>
        /// <param name="shapes"></param>
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

                // try to restore shape record
                foreach (var shape in Editor.GetAllShapes(shapes))
                {
                    if (shape.Data.Record == null)
                        continue;

                    Record record;
                    if (records.TryGetValue(shape.Data.Record.Id, out record))
                    {
                        // use existing record
                        shape.Data.Record = record;
                    }
                    else
                    {
                        // create Imported database
                        if (_editor.Project.CurrentDatabase == null)
                        {
                            var db = Database.Create(Constants.ImportedDatabaseName, shape.Data.Record.Columns);
                            _editor.Project.Databases = _editor.Project.Databases.Add(db);
                            _editor.Project.CurrentDatabase = db;
                        }

                        // add missing data record
                        _editor.Project.CurrentDatabase.Records = _editor.Project.CurrentDatabase.Records.Add(shape.Data.Record);

                        // recreate records dictionary
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
        ///
        /// </summary>
        /// <param name="shapes"></param>
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
        ///
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
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
        ///
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
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
        ///
        /// </summary>
        /// <param name="record"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
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
                            if (_editor.CurrentTool == Tool.None
                                || _editor.CurrentTool == Tool.Selection
                                || _editor.CurrentTool == Tool.Image
                                || _editor.CurrentTool == Tool.Path)
                            {
                                DropAsGroup(record, x, y);
                            }
                            else
                            {
                                DropAsShapeAndBind(record, x, y);
                            }
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
        /// 
        /// </summary>
        /// <param name="record"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DropAsShapeAndBind(Record record, double x, double y)
        {
            switch (_editor.CurrentTool)
            {
                case Tool.Point:
                    {
                        var point = XPoint.Create(x, y, _editor.Project.Options.PointShape);
                        point.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            point.Data.Bindings = point.Data.Bindings.Add(Binding.Create("X", record.Columns[0].Name, point.Data));
                            point.Data.Bindings = point.Data.Bindings.Add(Binding.Create("Y", record.Columns[1].Name, point.Data));
                        }

                        _editor.AddShape(point);
                    }
                    break;
                case Tool.Line:
                    {
                        var line = XLine.Create(
                            x, y,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape);
                        line.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            line.Data.Bindings = line.Data.Bindings.Add(Binding.Create("Start.X", record.Columns[0].Name, line.Data));
                            line.Data.Bindings = line.Data.Bindings.Add(Binding.Create("Start.Y", record.Columns[1].Name, line.Data));
                        }

                        if (record.Columns.Length >= 4)
                        {
                            line.Data.Bindings = line.Data.Bindings.Add(Binding.Create("End.X", record.Columns[2].Name, line.Data));
                            line.Data.Bindings = line.Data.Bindings.Add(Binding.Create("End.Y", record.Columns[3].Name, line.Data));
                        }

                        _editor.AddShape(line);
                    }
                    break;
                case Tool.Rectangle:
                    {
                        var rectangle = XRectangle.Create(
                            x, y,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape);
                        rectangle.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            rectangle.Data.Bindings = rectangle.Data.Bindings.Add(Binding.Create("TopLeft.X", record.Columns[0].Name, rectangle.Data));
                            rectangle.Data.Bindings = rectangle.Data.Bindings.Add(Binding.Create("TopLeft.Y", record.Columns[1].Name, rectangle.Data));
                        }

                        if (record.Columns.Length >= 4)
                        {
                            rectangle.Data.Bindings = rectangle.Data.Bindings.Add(Binding.Create("BottomRight.X", record.Columns[2].Name, rectangle.Data));
                            rectangle.Data.Bindings = rectangle.Data.Bindings.Add(Binding.Create("BottomRight.Y", record.Columns[3].Name, rectangle.Data));
                        }

                        _editor.AddShape(rectangle);
                    }
                    break;
                case Tool.Ellipse:
                    {
                        var ellipse = XEllipse.Create(
                            x, y,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape);
                        ellipse.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            ellipse.Data.Bindings = ellipse.Data.Bindings.Add(Binding.Create("TopLeft.X", record.Columns[0].Name, ellipse.Data));
                            ellipse.Data.Bindings = ellipse.Data.Bindings.Add(Binding.Create("TopLeft.Y", record.Columns[1].Name, ellipse.Data));
                        }

                        if (record.Columns.Length >= 4)
                        {
                            ellipse.Data.Bindings = ellipse.Data.Bindings.Add(Binding.Create("BottomRight.X", record.Columns[2].Name, ellipse.Data));
                            ellipse.Data.Bindings = ellipse.Data.Bindings.Add(Binding.Create("BottomRight.Y", record.Columns[3].Name, ellipse.Data));
                        }

                        _editor.AddShape(ellipse);
                    }
                    break;
                case Tool.Arc:
                    {
                        var arc = XArc.Create(
                            x, y,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape);
                        arc.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point1.X", record.Columns[0].Name, arc.Data));
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point1.Y", record.Columns[1].Name, arc.Data));
                        }

                        if (record.Columns.Length >= 4)
                        {
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point2.X", record.Columns[2].Name, arc.Data));
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point2.Y", record.Columns[3].Name, arc.Data));
                        }

                        if (record.Columns.Length >= 6)
                        {
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point3.X", record.Columns[4].Name, arc.Data));
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point3.Y", record.Columns[5].Name, arc.Data));
                        }

                        if (record.Columns.Length >= 8)
                        {
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point4.X", record.Columns[6].Name, arc.Data));
                            arc.Data.Bindings = arc.Data.Bindings.Add(Binding.Create("Point4.Y", record.Columns[7].Name, arc.Data));
                        }

                        _editor.AddShape(arc);
                    }
                    break;
                case Tool.Bezier:
                    {
                        var bezier = XBezier.Create(
                            x, y,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape);
                        bezier.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point1.X", record.Columns[0].Name, bezier.Data));
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point1.Y", record.Columns[1].Name, bezier.Data));
                        }

                        if (record.Columns.Length >= 4)
                        {
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point2.X", record.Columns[2].Name, bezier.Data));
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point2.Y", record.Columns[3].Name, bezier.Data));
                        }

                        if (record.Columns.Length >= 6)
                        {
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point3.X", record.Columns[4].Name, bezier.Data));
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point3.Y", record.Columns[5].Name, bezier.Data));
                        }

                        if (record.Columns.Length >= 8)
                        {
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point4.X", record.Columns[6].Name, bezier.Data));
                            bezier.Data.Bindings = bezier.Data.Bindings.Add(Binding.Create("Point4.Y", record.Columns[7].Name, bezier.Data));
                        }

                        _editor.AddShape(bezier);
                    }
                    break;
                case Tool.QBezier:
                    {
                        var qbezier = XQBezier.Create(
                            x, y,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape);
                        qbezier.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            qbezier.Data.Bindings = qbezier.Data.Bindings.Add(Binding.Create("Point1.X", record.Columns[0].Name, qbezier.Data));
                            qbezier.Data.Bindings = qbezier.Data.Bindings.Add(Binding.Create("Point1.Y", record.Columns[1].Name, qbezier.Data));
                        }

                        if (record.Columns.Length >= 4)
                        {
                            qbezier.Data.Bindings = qbezier.Data.Bindings.Add(Binding.Create("Point2.X", record.Columns[2].Name, qbezier.Data));
                            qbezier.Data.Bindings = qbezier.Data.Bindings.Add(Binding.Create("Point2.Y", record.Columns[3].Name, qbezier.Data));
                        }

                        if (record.Columns.Length >= 6)
                        {
                            qbezier.Data.Bindings = qbezier.Data.Bindings.Add(Binding.Create("Point3.X", record.Columns[4].Name, qbezier.Data));
                            qbezier.Data.Bindings = qbezier.Data.Bindings.Add(Binding.Create("Point3.Y", record.Columns[5].Name, qbezier.Data));
                        }

                        _editor.AddShape(qbezier);
                    }
                    break;
                case Tool.Text:
                    {
                        var text = XText.Create(
                            x, y,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape,
                            "Text");
                        text.Data.Record = record;

                        if (record.Columns.Length >= 2)
                        {
                            text.Data.Bindings = text.Data.Bindings.Add(Binding.Create("TopLeft.X", record.Columns[0].Name, text.Data));
                            text.Data.Bindings = text.Data.Bindings.Add(Binding.Create("TopLeft.Y", record.Columns[1].Name, text.Data));
                        }

                        if (record.Columns.Length >= 4)
                        {
                            text.Data.Bindings = text.Data.Bindings.Add(Binding.Create("BottomRight.X", record.Columns[2].Name, text.Data));
                            text.Data.Bindings = text.Data.Bindings.Add(Binding.Create("BottomRight.Y", record.Columns[3].Name, text.Data));
                        }

                        _editor.AddShape(text);
                    }
                    break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="record"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
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
                    var text = XText.Create(
                        px, py,
                        px + width, py + height,
                        _editor.Project.CurrentStyleLibrary.CurrentStyle,
                        _editor.Project.Options.PointShape, "");
                    var binding = Binding.Create("Text", record.Columns[i].Name, text.Data);
                    text.Data.Bindings = text.Data.Bindings.Add(binding);
                    g.AddShape(text);

                    py += height;
                }
            }

            var rectangle = XRectangle.Create(
                sx, sy,
                sx + width, sy + (double)length * height,
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
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
        ///
        /// </summary>
        /// <param name="style"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
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
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return _editor.EnableHistory && _editor.History.CanUndo();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return _editor.EnableHistory && _editor.History.CanRedo();
        }
    }
}
