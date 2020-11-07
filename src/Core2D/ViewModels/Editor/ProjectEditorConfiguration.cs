using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    public class ProjectEditorConfiguration : ViewModelBase
    {
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public static string DefaultLayerName = "Layer";

        public static string DefaultTemplateName = "Template";

        public static string DefaultPageName = "Page";

        public static string DefaultDocumentName = "Document";

        public static string DefaultScriptName = "Script";

        public static string DefaultDatabaseName = "Database";

        public static string DefaulColumnName = "Column";

        public static string DefaulPropertyName = "Property";

        public static string DefaulValue = "<empty>";

        public static string DefaulGroupLibraryName = "Groups";

        public static string DefaulGroupName = "Group";

        public static string DefaulStyleLibraryName = "Styles";

        public static string DefaulStyleName = "Style";

        public static string ImportedStyleLibraryName = "Imported";

        public static string ImportedDatabaseName = "Imported";

        public static string ProjectExtension = ".project";

        public static string CsvExtension = ".csv";

        public static string XlsxExtension = ".xlsx";

        public static string JsonExtension = ".json";

        public static string ScriptExtension = ".csx";

        public static string SvgExtension = ".svg";

        public static string[] ImageExtensions = new string[] { ".jpg", ".jpeg", ".png", "webp" };
    }
}
