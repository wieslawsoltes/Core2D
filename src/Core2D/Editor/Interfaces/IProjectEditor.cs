using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Editor.Bounds;
using Core2D.Editor.Input;
using Core2D.Editor.Recent;
using Core2D.Editor.Tools;
using Core2D.Editor.Tools.Path;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;
using DM = Dock.Model;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor contract.
    /// </summary>
    public interface IProjectEditor : IObservableObject
    {
        /// <summary>
        /// Gets or sets current project.
        /// </summary>
        IProjectContainer Project { get; set; }

        /// <summary>
        /// Gets or sets current project path.
        /// </summary>
        string ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets flag indicating that current project was modified.
        /// </summary>
        bool IsProjectDirty { get; set; }

        /// <summary>
        /// Gets or sets current project collections and objects observer.
        /// </summary>
        ProjectObserver Observer { get; set; }

        /// <summary>
        /// Gets or sets flag indicating that current tool is in idle mode.
        /// </summary>
        bool IsToolIdle { get; set; }

        /// <summary>
        /// Gets or sets current editor tool.
        /// </summary>
        IEditorTool CurrentTool { get; set; }

        /// <summary>
        /// Gets or sets current editor path tool.
        /// </summary>
        IPathTool CurrentPathTool { get; set; }

        /// <summary>
        /// Gets or sets recent projects collection.
        /// </summary>
        ImmutableArray<RecentFile> RecentProjects { get; set; }

        /// <summary>
        /// Gets or sets current recent project.
        /// </summary>
        RecentFile CurrentRecentProject { get; set; }
        /// <summary>
        /// Gets or sets current layout configuration.
        /// </summary>
        DM.IDock Layout { get; set; }

        /// <summary>
        /// Gets or sets about info.
        /// </summary>
        AboutInfo AboutInfo { get; set; }

        /// <summary>
        /// Gets or sets editor tools.
        /// </summary>
        ImmutableArray<IEditorTool> Tools { get; }

        /// <summary>
        /// Gets or sets editor path tools.
        /// </summary>
        ImmutableArray<IPathTool> PathTools { get; }

        /// <summary>
        /// Gets or sets current editor hit test.
        /// </summary>
        IHitTest HitTest { get; }

        /// <summary>
        /// Gets current log.
        /// </summary>
        ILog Log { get; }

        /// <summary>
        /// Gets current data flow.
        /// </summary>
        IDataFlow DataFlow { get; }

        /// <summary>
        /// Gets current renderer's.
        /// </summary>
        IShapeRenderer[] Renderers { get; }

        /// <summary>
        /// Gets current file system.
        /// </summary>
        IFileSystem FileIO { get; }

        /// <summary>
        /// Gets factory.
        /// </summary>
        IFactory Factory { get; }

        /// <summary>
        /// Gets container factory.
        /// </summary>
        IContainerFactory ContainerFactory { get; }

        /// <summary>
        /// Gets shape factory.
        /// </summary>
        IShapeFactory ShapeFactory { get; }

        /// <summary>
        /// Gets text clipboard.
        /// </summary>
        ITextClipboard TextClipboard { get; }

        /// <summary>
        /// Gets Json serializer.
        /// </summary>
        IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// Gets Xaml serializer.
        /// </summary>
        IXamlSerializer XamlSerializer { get; }

        /// <summary>
        /// Gets available file writers.
        /// </summary>
        ImmutableArray<IFileWriter> FileWriters { get; }

        /// <summary>
        /// Gets Csv file reader.
        /// </summary>
        ITextFieldReader<IDatabase> CsvReader { get; }

        /// <summary>
        /// Gets Csv file writer.
        /// </summary>
        ITextFieldWriter<IDatabase> CsvWriter { get; }

        /// <summary>
        /// Gets image key importer.
        /// </summary>
        IImageImporter ImageImporter { get; }

        /// <summary>
        /// Gets code script runner.
        /// </summary>
        IScriptRunner ScriptRunner { get; }

        /// <summary>
        /// Gets project editor platform.
        /// </summary>
        IProjectEditorPlatform Platform { get; }

        /// <summary>
        /// Gets editor canvas platform.
        /// </summary>
        IEditorCanvasPlatform CanvasPlatform { get; }

        /// <summary>
        /// Gets editor layout platform.
        /// </summary>
        IEditorLayoutPlatform LayoutPlatform { get; }

        /// <summary>
        /// Gets style editor.
        /// </summary>
        IStyleEditor StyleEditor { get; }

        /// <summary>
        /// Try to snap input arguments.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        /// <returns>The snapped value if enabled otherwise original position.</returns>
        (double sx, double sy) TryToSnap(InputArgs args);

        /// <summary>
        /// Get object item name.
        /// </summary>
        /// <param name="item">The object item.</param>
        string GetName(object item);

        /// <summary>
        /// Create new project, document or page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        void OnNew(object item);

        /// <summary>
        /// Create new page.
        /// </summary>
        /// <param name="selected">The selected page.</param>
        void OnNewPage(IPageContainer selected);

        /// <summary>
        /// Create new page.
        /// </summary>
        /// <param name="selected">The selected document.</param>
        void OnNewPage(IDocumentContainer selected);

        /// <summary>
        /// Create new document.
        /// </summary>
        void OnNewDocument();

        /// <summary>
        /// Create new project.
        /// </summary>
        void OnNewProject();

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        void OnOpenProject(string path);

        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="project">The project to open.</param>
        /// <param name="path">The project file path.</param>
        void OnOpenProject(IProjectContainer project, string path);

        /// <summary>
        /// Close project.
        /// </summary>
        void OnCloseProject();

        /// <summary>
        /// Save project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        void OnSaveProject(string path);

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="project">The target project.</param>
        /// <param name="path">The database file path.</param>
        void OnImportData(IProjectContainer project, string path);

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        void OnImportData(string path);

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        void OnExportData(string path, IDatabase database);

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="path">The database file path.</param>
        /// <param name="database">The database object.</param>
        void OnUpdateData(string path, IDatabase database);

        /// <summary>
        /// Import object.
        /// </summary>
        /// <param name="item">The object to import.</param>
        /// <param name="restore">Try to restore objects by name.</param>
        void OnImportObject(object item, bool restore);

        /// <summary>
        /// Import Xaml from a file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        void OnImportXaml(string path);

        /// <summary>
        /// Import Xaml string.
        /// </summary>
        /// <param name="xaml">The xaml string.</param>
        void OnImportXamlString(string xaml);

        /// <summary>
        /// Export Xaml to a file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        /// <param name="item">The object item.</param>
        void OnExportXaml(string path, object item);

        /// <summary>
        /// Import Json from a file.
        /// </summary>
        /// <param name="path">The json file path.</param>
        void OnImportJson(string path);

        /// <summary>
        /// Export Json to a file.
        /// </summary>
        /// <param name="path">The json file path.</param>
        /// <param name="item">The object item.</param>
        void OnExportJson(string path, object item);

        /// <summary>
        /// Export item.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="item">The item to export.</param>
        /// <param name="writer">The file writer.</param>
        void OnExport(string path, object item, IFileWriter writer);

        /// <summary>
        /// Execute code script.
        /// </summary>
        /// <param name="csharp">The script code.</param>
        void OnExecuteCode(string csharp);

        /// <summary>
        /// Execute code script in repl.
        /// </summary>
        /// <param name="path">The script code.</param>
        void OnExecuteRepl(string csharp);

        /// <summary>
        /// Reset previous script repl.
        /// </summary>
        void OnResetRepl();

        /// <summary>
        /// Execute code script from file.
        /// </summary>
        /// <param name="path">The code file path.</param>
        void OnExecuteScriptFile(string path);

        /// <summary>
        /// Execute code scripts from files.
        /// </summary>
        /// <param name="paths">The code file paths.</param>
        void OnExecuteScriptFile(string[] paths);

        /// <summary>
        /// Execute code script.
        /// </summary>
        /// <param name="script">The script object.</param>
        void OnExecuteScript(IScript script);

        /// <summary>
        /// Undo last action.
        /// </summary>
        void OnUndo();

        /// <summary>
        /// Redo last action.
        /// </summary>
        void OnRedo();

        /// <summary>
        /// Cut selected document, page or shapes.
        /// </summary>
        /// <param name="item">The item to cut.</param>
        void OnCut(object item);

        /// <summary>
        /// Copy document, page or shapes to clipboard.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        void OnCopy(object item);

        /// <summary>
        /// Paste text from clipboard as document, page or shapes.
        /// </summary>
        /// <param name="item">The item to paste.</param>
        void OnPaste(object item);

        /// <summary>
        /// Delete selected document, page, layer or shapes.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        void OnDelete(object item);

        /// <summary>
        /// Select all shapes.
        /// </summary>
        void OnSelectAll();

        /// <summary>
        /// De-select all shapes.
        /// </summary>
        void OnDeselectAll();

        /// <summary>
        /// Remove all shapes.
        /// </summary>
        void OnClearAll();

        /// <summary>
        /// Cancel current action.
        /// </summary>
        void OnCancel();

        /// <summary>
        /// Group selected shapes.
        /// </summary>
        void OnGroupSelected();

        /// <summary>
        /// Ungroup selected shapes.
        /// </summary>
        void OnUngroupSelected();

        /// <summary>
        /// Move selected shapes up.
        /// </summary>
        void OnMoveUpSelected();

        /// <summary>
        /// Move selected shapes down.
        /// </summary>
        void OnMoveDownSelected();

        /// <summary>
        /// Move selected shapes left.
        /// </summary>
        void OnMoveLeftSelected();

        /// <summary>
        /// Move selected shapes right.
        /// </summary>
        void OnMoveRightSelected();

        /// <summary>
        /// Stack horizontally selected shapes.
        /// </summary>
        void OnStackHorizontallySelected();

        /// <summary>
        /// Stack vertically selected shapes.
        /// </summary>
        void OnStackVerticallySelected();

        /// <summary>
        /// Distribute horizontally selected shapes.
        /// </summary>
        void OnDistributeHorizontallySelected();

        /// <summary>
        /// Distribute vertically selected shapes.
        /// </summary>
        void OnDistributeVerticallySelected();

        /// <summary>
        /// Align left selected shapes.
        /// </summary>
        void OnAlignLeftSelected();

        /// <summary>
        /// Align centered selected shapes.
        /// </summary>
        void OnAlignCenteredSelected();

        /// <summary>
        /// Align left selected shapes.
        /// </summary>
        void OnAlignRightSelected();

        /// <summary>
        /// Align top selected shapes.
        /// </summary>
        void OnAlignTopSelected();

        /// <summary>
        /// Align center selected shapes.
        /// </summary>
        void OnAlignCenterSelected();

        /// <summary>
        /// Align bottom selected shapes.
        /// </summary>
        void OnAlignBottomSelected();

        /// <summary>
        /// Bring selected shapes to the top of the stack.
        /// </summary>
        void OnBringToFrontSelected();

        /// <summary>
        /// Bring selected shapes one step closer to the front of the stack.
        /// </summary>
        void OnBringForwardSelected();

        /// <summary>
        /// Bring selected shapes one step down within the stack.
        /// </summary>
        void OnSendBackwardSelected();

        /// <summary>
        /// Bring selected shapes to the bottom of the stack.
        /// </summary>
        void OnSendToBackSelected();

        /// <summary>
        /// Set current tool to <see cref="ToolNone"/>.
        /// </summary>
        void OnToolNone();

        /// <summary>
        /// Set current tool to <see cref="ToolSelection"/>.
        /// </summary>
        void OnToolSelection();

        /// <summary>
        /// Set current tool to <see cref="ToolPoint"/>.
        /// </summary>
        void OnToolPoint();

        /// <summary>
        /// Set current tool to <see cref="ToolLine"/> or current path tool to <see cref="PathToolLine"/>.
        /// </summary>
        void OnToolLine();

        /// <summary>
        /// Set current tool to <see cref="ToolArc"/> or current path tool to <see cref="PathToolArc"/>.
        /// </summary>
        void OnToolArc();

        /// <summary>
        /// Set current tool to <see cref="ToolCubicBezier"/> or current path tool to <see cref="PathToolCubicBezier"/>.
        /// </summary>
        void OnToolCubicBezier();

        /// <summary>
        /// Set current tool to <see cref="ToolQuadraticBezier"/> or current path tool to <see cref="PathToolQuadraticBezier"/>.
        /// </summary>
        void OnToolQuadraticBezier();

        /// <summary>
        /// Set current tool to <see cref="ToolPath"/>.
        /// </summary>
        void OnToolPath();

        /// <summary>
        /// Set current tool to <see cref="ToolRectangle"/>.
        /// </summary>
        void OnToolRectangle();

        /// <summary>
        /// Set current tool to <see cref="ToolEllipse"/>.
        /// </summary>
        void OnToolEllipse();

        /// <summary>
        /// Set current tool to <see cref="ToolText"/>.
        /// </summary>
        void OnToolText();

        /// <summary>
        /// Set current tool to <see cref="ToolImage"/>.
        /// </summary>
        void OnToolImage();

        /// <summary>
        /// Set current path tool to <see cref="PathToolMove"/>.
        /// </summary>
        void OnToolMove();

        /// <summary>
        /// Reset current tool.
        /// </summary>
        void OnResetTool();

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsStroked"/> option.
        /// </summary>
        void OnToggleDefaultIsStroked();

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsFilled"/> option.
        /// </summary>
        void OnToggleDefaultIsFilled();

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsClosed"/> option.
        /// </summary>
        void OnToggleDefaultIsClosed();

        /// <summary>
        /// Toggle <see cref="Options.DefaultIsSmoothJoin"/> option.
        /// </summary>
        void OnToggleDefaultIsSmoothJoin();

        /// <summary>
        /// Toggle <see cref="Options.SnapToGrid"/> option.
        /// </summary>
        void OnToggleSnapToGrid();

        /// <summary>
        /// Toggle <see cref="Options.TryToConnect"/> option.
        /// </summary>
        void OnToggleTryToConnect();

        /// <summary>
        /// Add database.
        /// </summary>
        void OnAddDatabase();

        /// <summary>
        /// Remove database.
        /// </summary>
        /// <param name="db">The database to remove.</param>
        void OnRemoveDatabase(IDatabase db);

        /// <summary>
        /// Add column to database.
        /// </summary>
        /// <param name="db">The records database.</param>
        void OnAddColumn(IDatabase db);

        /// <summary>
        /// Remove column from database.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        void OnRemoveColumn(IColumn column);

        /// <summary>
        /// Add record to database.
        /// </summary>
        /// <param name="db">The records database.</param>
        void OnAddRecord(IDatabase db);

        /// <summary>
        /// Remove record from database.
        /// </summary>
        /// <param name="record">The data record.</param>
        void OnRemoveRecord(IRecord record);

        /// <summary>
        /// Reset data context record.
        /// </summary>
        /// <param name="data">The data context.</param>
        void OnResetRecord(IContext data);

        /// <summary>
        /// Set current record as selected shape(s) or current page data record.
        /// </summary>
        /// <param name="record">The data record.</param>
        void OnApplyRecord(IRecord record);

        /// <summary>
        /// Add property to data context.
        /// </summary>
        /// <param name="data">The data context.</param>
        void OnAddProperty(IContext data);

        /// <summary>
        /// Remove property from data context.
        /// </summary>
        /// <param name="property">The property to remove.</param>
        void OnRemoveProperty(IProperty property);

        /// <summary>
        /// Add group library.
        /// </summary>
        void OnAddGroupLibrary();

        /// <summary>
        /// Remove group library.
        /// </summary>
        /// <param name="library">The group library to remove.</param>
        void OnRemoveGroupLibrary(ILibrary<IGroupShape> library);

        /// <summary>
        /// Add group.
        /// </summary>
        /// <param name="library">The group library.</param>
        void OnAddGroup(ILibrary<IGroupShape> library);

        /// <summary>
        /// Remove group.
        /// </summary>
        /// <param name="group">The group item.</param>
        void OnRemoveGroup(IGroupShape group);

        /// <summary>
        /// Insert current group to container.
        /// </summary>
        /// <param name="group">The group instance.</param>
        void OnInsertGroup(IGroupShape group);

        /// <summary>
        /// Add layer to container.
        /// </summary>
        /// <param name="container">The container instance.</param>
        void OnAddLayer(IPageContainer container);

        /// <summary>
        /// Remove layer.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        void OnRemoveLayer(ILayerContainer layer);

        /// <summary>
        /// Add style library.
        /// </summary>
        void OnAddStyleLibrary();

        /// <summary>
        /// Remove style library.
        /// </summary>
        /// <param name="library">The style library to remove.</param>
        void OnRemoveStyleLibrary(ILibrary<IShapeStyle> library);

        /// <summary>
        /// Add style.
        /// </summary>
        /// <param name="library">The style library.</param>
        void OnAddStyle(ILibrary<IShapeStyle> library);

        /// <summary>
        /// Remove style.
        /// </summary>
        /// <param name="style">The style to remove.</param>
        void OnRemoveStyle(IShapeStyle style);

        /// <summary>
        /// Set current style as selected shape style.
        /// </summary>
        /// <param name="style">The shape style item.</param>
        void OnApplyStyle(IShapeStyle style);

        /// <summary>
        /// Set current data as selected shape data.
        /// </summary>
        /// <param name="data">The data item.</param>
        void OnApplyData(IContext data);

        /// <summary>
        /// Add shape.
        /// </summary>
        /// <param name="shape">The shape instance.</param>
        void OnAddShape(IBaseShape shape);

        /// <summary>
        /// Remove shape.
        /// </summary>
        /// <param name="shape">The shape instance.</param>
        void OnRemoveShape(IBaseShape shape);

        /// <summary>
        /// Add template.
        /// </summary>
        void OnAddTemplate();

        /// <summary>
        /// Remove template.
        /// </summary>
        /// <param name="template">The template object.</param>
        void OnRemoveTemplate(IPageContainer template);

        /// <summary>
        /// Edit template.
        /// </summary>
        void OnEditTemplate(IPageContainer template);

        /// <summary>
        /// Set page template.
        /// </summary>
        /// <param name="template">The template object.</param>
        void OnApplyTemplate(IPageContainer template);

        /// <summary>
        /// Add script.
        /// </summary>
        void OnAddScript();

        /// <summary>
        /// Remove script.
        /// </summary>
        /// <param name="script">The script object.</param>
        void OnRemoveScript(IScript script);

        /// <summary>
        /// Add image key.
        /// </summary>
        /// <param name="path">The image path.</param>
        Task<string> OnAddImageKey(string path);

        /// <summary>
        /// Remove image key.
        /// </summary>
        /// <param name="key">The image key.</param>
        void OnRemoveImageKey(string key);

        /// <summary>
        /// Notifies when selected project tree item changed.
        /// </summary>
        /// <param name="item">The selected item.</param>
        void OnSelectedItemChanged(IObservableObject item);

        /// <summary>
        /// Add page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        void OnAddPage(object item);

        /// <summary>
        /// Insert page before current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        void OnInsertPageBefore(object item);

        /// <summary>
        /// Insert page after current page.
        /// </summary>
        /// <param name="item">The parent item.</param>
        void OnInsertPageAfter(object item);

        /// <summary>
        /// Add document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        void OnAddDocument(object item);

        /// <summary>
        /// Insert document before current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        void OnInsertDocumentBefore(object item);

        /// <summary>
        /// Insert document after current document.
        /// </summary>
        /// <param name="item">The parent item.</param>
        void OnInsertDocumentAfter(object item);

        /// <summary>
        /// Load project.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="path">The project path.</param>
        void OnLoad(IProjectContainer project, string path = null);

        /// <summary>
        /// Unload project.
        /// </summary>
        void OnUnload();

        /// <summary>
        /// Invalidate renderer's cache.
        /// </summary>
        /// <param name="isZooming">The flag indicating whether is zooming.</param>
        void OnInvalidateCache(bool isZooming);

        /// <summary>
        /// Add recent project file.
        /// </summary>
        /// <param name="path">The project path.</param>
        /// <param name="name">The project name.</param>
        void OnAddRecent(string path, string name);

        /// <summary>
        /// Load recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        void OnLoadRecent(string path);

        /// <summary>
        /// Save recent project files.
        /// </summary>
        /// <param name="path">The recent projects path.</param>
        void OnSaveRecent(string path);

        /// <summary>
        /// Load layout configuration.
        /// </summary>
        /// <param name="path">The layout configuration path.</param>
        void OnLoadLayout(string path);

        /// <summary>
        /// Save layout configuration.
        /// </summary>
        /// <param name="path">The layout configuration path.</param>
        void OnSaveLayout(string path);

        /// <summary>
        /// Navigate to view.
        /// </summary>
        /// <param name="view">The view to navigate to.</param>
        void OnNavigate(object view);

        /// <summary>
        /// Check if undo action is available.
        /// </summary>
        /// <returns>Returns true if undo action is available.</returns>
        bool CanUndo();

        /// <summary>
        /// Check if redo action is available.
        /// </summary>
        /// <returns>Returns true if redo action is available.</returns>
        bool CanRedo();

        /// <summary>
        /// Checks if can copy.
        /// </summary>
        /// <returns>Returns true if can copy.</returns>
        bool CanCopy();

        /// <summary>
        /// Checks if can paste.
        /// </summary>
        /// <returns>Returns true if can paste.</returns>
        Task<bool> CanPaste();

        /// <summary>
        /// Copy selected shapes to clipboard.
        /// </summary>
        /// <param name="shapes"></param>
        void OnCopyShapes(IList<IBaseShape> shapes);

        /// <summary>
        /// Paste text from clipboard as shapes.
        /// </summary>
        /// <param name="text">The text string.</param>
        void OnTryPaste(string text);

        /// <summary>
        /// Paste shapes to current container.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        void OnPasteShapes(IEnumerable<IBaseShape> shapes);

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        void OnSelect(IEnumerable<IBaseShape> shapes);

        /// <summary>
        /// Clone the <see cref="BaseShape"/> object.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="IBaseShape"/> object.</param>
        /// <returns>The cloned <see cref="IBaseShape"/> object.</returns>
        T CloneShape<T>(T shape) where T : IBaseShape;

        /// <summary>
        /// Clone the <see cref="LayerContainer"/> object.
        /// </summary>
        /// <param name="container">The <see cref="LayerContainer"/> object.</param>
        /// <returns>The cloned <see cref="LayerContainer"/> object.</returns>
        ILayerContainer Clone(ILayerContainer container);

        /// <summary>
        /// Clone the <see cref="PageContainer"/> object.
        /// </summary>
        /// <param name="container">The <see cref="PageContainer"/> object.</param>
        /// <returns>The cloned <see cref="PageContainer"/> object.</returns>
        IPageContainer Clone(IPageContainer container);

        /// <summary>
        /// Clone the <see cref="DocumentContainer"/> object.
        /// </summary>
        /// <param name="document">The <see cref="DocumentContainer"/> object.</param>
        /// <returns>The cloned <see cref="DocumentContainer"/> object.</returns>
        IDocumentContainer Clone(IDocumentContainer document);

        /// <summary>
        /// Process dropped files.
        /// </summary>
        /// <param name="files">The files array.</param>
        /// <returns>Returns true if success.</returns>
        bool OnDropFiles(string[] files);

        /// <summary>
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <param name="shape">The <see cref="IBaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        bool OnDropShape(IBaseShape shape, double x, double y, bool bExecute = true);

        /// <summary>
        /// Drop <see cref="BaseShape"/> object in current container at specified location.
        /// </summary>
        /// <typeparam name="T">The shape type.</typeparam>
        /// <param name="shape">The <see cref="BaseShape"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        void OnDropShapeAsClone<T>(T shape, double x, double y) where T : IBaseShape;

        /// <summary>
        /// Drop <see cref="IRecord"/> object in current container at specified location.
        /// </summary>
        /// <param name="record">The <see cref="IRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        bool OnDropRecord(IRecord record, double x, double y, bool bExecute = true);

        /// <summary>
        /// Drop <see cref="IRecord"/> object in current container at specified location as group bound to this record.
        /// </summary>
        /// <param name="record">The <see cref="IRecord"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        void OnDropRecordAsGroup(IRecord record, double x, double y);

        /// <summary>
        /// Drop <see cref="IShapeStyle"/> object in current container at specified location.
        /// </summary>
        /// <param name="style">The <see cref="IShapeStyle"/> object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        bool OnDropStyle(IShapeStyle style, double x, double y, bool bExecute = true);

        /// <summary>
        /// Drop <see cref="PageContainer"/> object in current container at specified location.
        /// </summary>
        /// <param name="template">The template object.</param>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="bExecute">The flag indicating whether to execute action.</param>
        /// <returns>Returns true if success.</returns>
        bool OnDropTemplate(IPageContainer template, double x, double y, bool bExecute = true);

        /// <summary>
        /// Remove selected shapes.
        /// </summary>
        void OnDeleteSelected();

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        void Deselect();

        /// <summary>
        /// Select shape.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shape">The shape to select.</param>
        void Select(ILayerContainer layer, IBaseShape shape);

        /// <summary>
        /// Select shapes.
        /// </summary>
        /// <param name="layer">The owner layer.</param>
        /// <param name="shapes">The shapes to select.</param>
        void Select(ILayerContainer layer, ISet<IBaseShape> shapes);

        /// <summary>
        /// De-select shape(s).
        /// </summary>
        /// <param name="layer">The layer object.</param>
        void Deselect(ILayerContainer layer);

        /// <summary>
        /// Try to select shape at specified coordinates.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="x">The X coordinate in layer.</param>
        /// <param name="y">The Y coordinate in layer.</param>
        /// <param name="deselect">The flag indicating whether to deselect shapes.</param>
        /// <returns>True if selecting shape was successful.</returns>
        bool TryToSelectShape(ILayerContainer layer, double x, double y, bool deselect = true);

        /// <summary>
        /// Try to select shapes inside rectangle.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="rectangle">The selection rectangle.</param>
        /// <param name="deselect">The flag indicating whether to deselect shapes.</param>
        /// <param name="includeSelected">The flag indicating whether to include selected shapes.</param>
        /// <returns>True if selecting shapes was successful.</returns>
        bool TryToSelectShapes(ILayerContainer layer, IRectangleShape rectangle, bool deselect = true, bool includeSelected = false);

        /// <summary>
        /// Hover shape.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        /// <param name="shape">The shape to hover.</param>
        void Hover(ILayerContainer layer, IBaseShape shape);

        /// <summary>
        /// De-hover shape.
        /// </summary>
        /// <param name="layer">The layer object.</param>
        void Dehover(ILayerContainer layer);

        /// <summary>
        /// Try to hover shape at specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <returns>True if hovering shape was successful.</returns>
        bool TryToHoverShape(double x, double y);

        /// <summary>
        /// Try to get connection point at specified location.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>The connected point if success.</returns>
        IPointShape TryToGetConnectionPoint(double x, double y);

        /// <summary>
        /// Try to split line at specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate in container.</param>
        /// <param name="y">The Y coordinate in container.</param>
        /// <param name="point">The point used for split line start or end.</param>
        /// <param name="select">The flag indicating whether to select split line.</param>
        /// <returns>True if line split was successful.</returns>
        bool TryToSplitLine(double x, double y, IPointShape point, bool select = false);

        /// <summary>
        /// Try to split lines using group connectors.
        /// </summary>
        /// <param name="line">The line to split.</param>
        /// <param name="p0">The first connector point.</param>
        /// <param name="p1">The second connector point.</param>
        /// <returns>True if line split was successful.</returns>
        bool TryToSplitLine(ILineShape line, IPointShape p0, IPointShape p1);

        /// <summary>
        /// Try to connect lines to connectors.
        /// </summary>
        /// <param name="lines">The lines to connect.</param>
        /// <param name="connectors">The connectors array.</param>
        /// <param name="threshold">The connection threshold.</param>
        /// <returns>True if connection was successful.</returns>
        bool TryToConnectLines(IEnumerable<ILineShape> lines, ImmutableArray<IPointShape> connectors, double threshold);

        /// <summary>
        /// Group shapes.
        /// </summary>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="name">The group name.</param>
        IGroupShape Group(ISet<IBaseShape> shapes, string name);

        /// <summary>
        /// Ungroup shapes.
        /// </summary>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        bool Ungroup(IBaseShape shape, ISet<IBaseShape> shapes);

        /// <summary>
        /// Bring a shape to the top of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        void BringToFront(IBaseShape source);

        /// <summary>
        /// Move a shape one step closer to the front of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        void BringForward(IBaseShape source);

        /// <summary>
        /// Move a shape one step down within the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        void SendBackward(IBaseShape source);

        /// <summary>
        /// Move a shape to the bottom of the stack.
        /// </summary>
        /// <param name="source">The source shape.</param>
        void SendToBack(IBaseShape source);

        /// <summary>
        /// Move shapes by specified offset.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void MoveShapesBy(IEnumerable<IBaseShape> shapes, double dx, double dy);

        /// <summary>
        /// Move shape(s) by specified offset.
        /// </summary>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void MoveBy(IBaseShape shape, ISet<IBaseShape> shapes, double dx, double dy);

        /// <summary>
        /// Move items in the library.
        /// </summary>
        /// <typeparam name="T">The type of the library.</typeparam>
        /// <param name="library">The items library.</param>
        /// <param name="sourceIndex">The source item index.</param>
        /// <param name="targetIndex">The target item index.</param>
        void MoveItem<T>(ILibrary<T> library, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap items in the library.
        /// </summary>
        /// <typeparam name="T">The type of the library.</typeparam>
        /// <param name="library">The items library.</param>
        /// <param name="sourceIndex">The source item index.</param>
        /// <param name="targetIndex">The target item index.</param>
        void SwapItem<T>(ILibrary<T> library, int sourceIndex, int targetIndex);

        /// <summary>
        /// Insert item into the library.
        /// </summary>
        /// <typeparam name="T">The type of the library.</typeparam>
        /// <param name="library">The items library.</param>
        /// <param name="T">The item to insert.</param>
        /// <param name="index">The insert index.</param>
        void InsertItem<T>(ILibrary<T> library, T item, int index);
    }
}
