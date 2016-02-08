// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Editor constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Default <see cref="XLayer"/> name.
        /// </summary>
        public const string DefaultLayerName = "Layer";

        /// <summary>
        /// Default <see cref="XTemplate"/> name.
        /// </summary>
        public const string DefaultTemplateName = "Template";

        /// <summary>
        /// Default <see cref="XPage"/> name.
        /// </summary>
        public const string DefaultPageName = "Page";

        /// <summary>
        /// Default <see cref="XDocument"/> name.
        /// </summary>
        public const string DefaultDocumentName = "Document";

        /// <summary>
        /// Default <see cref="XDatabase"/> name.
        /// </summary>
        public const string DefaultDatabaseName = "Db";

        /// <summary>
        /// Default <see cref="XColumn"/> name.
        /// </summary>
        public const string DefaulColumnName = "Column";

        /// <summary>
        /// Default <see cref="XProperty"/> name.
        /// </summary>
        public const string DefaulPropertyName = "Property";

        /// <summary>
        /// Default property value.
        /// </summary>
        public const string DefaulValue = "<empty>";

        /// <summary>
        /// Default <see cref="XLibrary{XGroup}"/> name.
        /// </summary>
        public const string DefaulGroupLibraryName = "Groups";

        /// <summary>
        /// Default <see cref="XGroup"/> name.
        /// </summary>
        public const string DefaulGroupName = "Group";

        /// <summary>
        /// Default <see cref="XLibrary{ShapeStyle}"/> name.
        /// </summary>
        public const string DefaulStyleLibraryName = "Styles";

        /// <summary>
        /// Default <see cref="ShapeStyle"/> name.
        /// </summary>
        public const string DefaulStyleName = "Style";

        /// <summary>
        /// Default imported <see cref="XLibrary{ShapeStyle}"/> name.
        /// </summary>
        public const string ImportedStyleLibraryName = "Imported";

        /// <summary>
        ///  Default imported <see cref="XDatabase"/> name.
        /// </summary>
        public const string ImportedDatabaseName = "Imported";

        /// <summary>
        /// Project file extension.
        /// </summary>
        public const string ProjectExtension = ".project";

        /// <summary>
        /// CSV file extension.
        /// </summary>
        public const string CsvExtension = ".csv";

        /// <summary>
        /// Style file extension.
        /// </summary>
        public const string StyleExtension = ".style";

        /// <summary>
        /// Styles file extension.
        /// </summary>
        public const string StylesExtension = ".styles";

        /// <summary>
        /// Style library file extension.
        /// </summary>
        public const string StyleLibraryExtension = ".stylelibrary";

        /// <summary>
        /// Style libraries file extension.
        /// </summary>
        public const string StyleLibrariesExtension = ".stylelibraries";

        /// <summary>
        /// Group file extension.
        /// </summary>
        public const string GroupExtension = ".group";

        /// <summary>
        /// Groups file extension.
        /// </summary>
        public const string GroupsExtension = ".groups";

        /// <summary>
        /// Group library file extension.
        /// </summary>
        public const string GroupLibraryExtension = ".grouplibrary";

        /// <summary>
        /// Group libraries file extension.
        /// </summary>
        public const string GroupLibrariesExtension = ".grouplibraries";

        /// <summary>
        /// Template file extension.
        /// </summary>
        public const string TemplateExtension = ".template";

        /// <summary>
        /// Templates file extension.
        /// </summary>
        public const string TemplatesExtension = ".templates";

        /// <summary>
        /// XAML file extension.
        /// </summary>
        public const string XamlExtension = ".xaml";
    }
}
