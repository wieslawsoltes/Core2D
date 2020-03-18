using Core2D.Containers;
using Core2D.Data;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines project editor platform contract.
    /// </summary>
    public interface IProjectEditorPlatform
    {
        /// <summary>
        /// Open project.
        /// </summary>
        /// <param name="path">The project file path.</param>
        void OnOpen(string path);

        /// <summary>
        /// Save project.
        /// </summary>
        void OnSave();

        /// <summary>
        /// Save project as.
        /// </summary>
        void OnSaveAs();

        /// <summary>
        /// Import json.
        /// </summary>
        /// <param name="path">The project json path.</param>
        void OnImportJson(string path);

        /// <summary>
        /// Import object.
        /// </summary>
        /// <param name="path">The object file path.</param>
        void OnImportObject(string path);

        /// <summary>
        /// Import xaml.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        void OnImportXaml(string path);

        /// <summary>
        /// Export json.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExportJson(object item);

        /// <summary>
        /// Export object.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExportObject(object item);

        /// <summary>
        /// Export xaml.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExportXaml(object item);

        /// <summary>
        /// Export project, document or page.
        /// </summary>
        /// <param name="item">The object to export.</param>
        void OnExport(object item);

        /// <summary>
        /// Execute script.
        /// </summary>
        /// <param name="path">The script file path.</param>
        void OnExecuteScript(string path);

        /// <summary>
        /// Close application view.
        /// </summary>
        void OnExit();

        /// <summary>
        /// Copy page or selected shapes to clipboard as Emf.
        /// </summary>
        /// <param name="item">The object to copy as EMF.</param>
        void OnCopyAsEmf(object item);

        /// <summary>
        /// Import database.
        /// </summary>
        /// <param name="project">The target project.</param>
        void OnImportData(IProjectContainer project);

        /// <summary>
        /// Export database.
        /// </summary>
        /// <param name="db">The database to export.</param>
        void OnExportData(IDatabase db);

        /// <summary>
        /// Update database.
        /// </summary>
        /// <param name="db">The database to update.</param>
        void OnUpdateData(IDatabase db);

        /// <summary>
        /// Show document viewer.
        /// </summary>
        void OnDocumentViewer();

        /// <summary>
        /// Show object browser.
        /// </summary>
        void OnObjectBrowser();

        /// <summary>
        /// Show script editor.
        /// </summary>
        void OnScriptEditor();

        /// <summary>
        /// Show about dialog.
        /// </summary>
        void OnAboutDialog();

        /// <summary>
        /// Auto-fit view to the available extents.
        /// </summary>
        void OnZoomAutoFit();

        /// <summary>
        /// Reset view size to defaults.
        /// </summary>
        void OnZoomReset();

        /// <summary>
        /// Load layout configuration.
        /// </summary>
        void OnLoadLayout();

        /// <summary>
        /// Save layout configuration.
        /// </summary>
        void OnSaveLayout();

        /// <summary>
        /// Reset layout configuration.
        /// </summary>
        void OnResetLayout();
    }
}
