using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor configuration.
    /// </summary>
    public class ProjectEditorConfiguration : ObservableObject, ISettings
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Default <see cref="ILayerContainer"/> name.
        /// </summary>
        public static string DefaultLayerName = "Layer";

        /// <summary>
        /// Default <see cref="IPageContainer"/> template name.
        /// </summary>
        public static string DefaultTemplateName = "Template";

        /// <summary>
        /// Default <see cref="IPageContainer"/> page name.
        /// </summary>
        public static string DefaultPageName = "Page";

        /// <summary>
        /// Default <see cref="IDocumentContainer"/> name.
        /// </summary>
        public static string DefaultDocumentName = "Document";

        /// <summary>
        /// Default <see cref="IScript"/> script name.
        /// </summary>
        public static string DefaultScriptName = "Script";

        /// <summary>
        /// Default <see cref="IDatabase"/> name.
        /// </summary>
        public static string DefaultDatabaseName = "Db";

        /// <summary>
        /// Default <see cref="IColumn"/> name.
        /// </summary>
        public static string DefaulColumnName = "Column";

        /// <summary>
        /// Default <see cref="IProperty"/> name.
        /// </summary>
        public static string DefaulPropertyName = "Property";

        /// <summary>
        /// Default property value.
        /// </summary>
        public static string DefaulValue = "<empty>";

        /// <summary>
        /// Default <see cref="ILibrary{IGroupShape}"/> name.
        /// </summary>
        public static string DefaulGroupLibraryName = "Groups";

        /// <summary>
        /// Default <see cref="IGroupShape"/> name.
        /// </summary>
        public static string DefaulGroupName = "Group";

        /// <summary>
        /// Default <see cref="ILibrary{ShapeStyle}"/> name.
        /// </summary>
        public static string DefaulStyleLibraryName = "Styles";

        /// <summary>
        /// Default <see cref="IShapeStyle"/> name.
        /// </summary>
        public static string DefaulStyleName = "Style";

        /// <summary>
        /// Default imported <see cref="ILibrary{ShapeStyle}"/> name.
        /// </summary>
        public static string ImportedStyleLibraryName = "Imported";

        /// <summary>
        /// Default imported <see cref="IDatabase"/> name.
        /// </summary>
        public static string ImportedDatabaseName = "Imported";

        /// <summary>
        /// Project file extension.
        /// </summary>
        public static string ProjectExtension = ".project";

        /// <summary>
        /// CSV file extension.
        /// </summary>
        public static string CsvExtension = ".csv";

        /// <summary>
        /// XLSX file extension.
        /// </summary>
        public static string XlsxExtension = ".xlsx";

        /// <summary>
        /// Xaml file extension.
        /// </summary>
        public static string XamlExtension = ".xaml";

        /// <summary>
        /// Json file extension.
        /// </summary>
        public static string JsonExtension = ".json";

        /// <summary>
        /// Script file extension.
        /// </summary>
        public static string ScriptExtension = ".csx";

        /// <summary>
        /// Image file extension.
        /// </summary>
        public static string[] ImageExtensions = new string[] { ".jpg", ".jpeg", ".png", "webp" };
    }
}
