// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading.Tasks;

namespace Core2D.Editor.Interfaces
{
    /// <summary>
    /// Editor application contract.
    /// </summary>
    public interface IEditorApplication
    {
        /// <summary>
        /// Get the image key from file path.
        /// </summary>
        /// <returns>The image key.</returns>
        Task<string> OnGetImageKeyAsync();

        /// <summary>
        /// Open project from file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnOpenAsync(string path);

        /// <summary>
        /// Save current project to external file.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnSaveAsync();

        /// <summary>
        /// Save current project to external file.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnSaveAsAsync();

        /// <summary>
        /// Import xaml from file.
        /// </summary>
        /// <param name="path">The xaml file path.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnImportXamlAsync(string path);

        /// <summary>
        /// Export xaml to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnExportXamlAsync(object item);

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnExportAsync(object item);

        /// <summary>
        /// Import records into new database.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnImportDataAsync();

        /// <summary>
        /// Export records to external file.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnExportDataAsync();

        /// <summary>
        /// Update records in current database.
        /// </summary>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnUpdateDataAsync();

        /// <summary>
        /// Import item object from external file.
        /// </summary>
        /// <param name="item">The item object to import.</param>
        /// <param name="type">The type of item object.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnImportObjectAsync(object item, CoreType type);

        /// <summary>
        /// Export item object to external file.
        /// </summary>
        /// <param name="item">The item object to export.</param>
        /// <param name="type">The type of item object.</param>
        /// <returns>The await <see cref="Task"/>.</returns>
        Task OnExportObjectAsync(object item, CoreType type);

        /// <summary>
        /// Save currently selected shapes as enhanced metafile.
        /// </summary>
        /// <param name="path">The file path.</param>
        Task OnExportAsEmfAsync(string path);

        /// <summary>
        /// Copy currently selected shapes to clipboard as enhanced metafile.
        /// </summary>
        Task OnCopyAsEmfAsync();

        /// <summary>
        /// Reset view size to defaults.
        /// </summary>
        Task OnZoomResetAsync();

        /// <summary>
        /// Auto-fit view to the available extents.
        /// </summary>
        Task OnZoomAutoFitAsync();

        /// <summary>
        /// Load docking manager layout.
        /// </summary>
        Task OnLoadWindowLayout();

        /// <summary>
        /// Save docking manager layout.
        /// </summary>
        Task OnSaveWindowLayoutAsync();

        /// <summary>
        /// Reset docking manager layout.
        /// </summary>
        Task OnResetWindowLayoutAsync();

        /// <summary>
        /// Show object browser.
        /// </summary>
        Task OnShowObjectBrowserAsync();

        /// <summary>
        /// Close application view.
        /// </summary>
        void OnCloseView();
    }
}
