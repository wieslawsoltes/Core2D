// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Data;
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
        public const string DefaultLayerName = "Layer";

        /// <summary>
        /// Default <see cref="IPageContainer"/> template name.
        /// </summary>
        public const string DefaultTemplateName = "Template";

        /// <summary>
        /// Default <see cref="IPageContainer"/> page name.
        /// </summary>
        public const string DefaultPageName = "Page";

        /// <summary>
        /// Default <see cref="IDocumentContainer"/> name.
        /// </summary>
        public const string DefaultDocumentName = "Document";

        /// <summary>
        /// Default <see cref="IDatabase"/> name.
        /// </summary>
        public const string DefaultDatabaseName = "Db";

        /// <summary>
        /// Default <see cref="IColumn"/> name.
        /// </summary>
        public const string DefaulColumnName = "Column";

        /// <summary>
        /// Default <see cref="IProperty"/> name.
        /// </summary>
        public const string DefaulPropertyName = "Property";

        /// <summary>
        /// Default property value.
        /// </summary>
        public const string DefaulValue = "<empty>";

        /// <summary>
        /// Default <see cref="ILibrary{IGroupShape}"/> name.
        /// </summary>
        public const string DefaulGroupLibraryName = "Groups";

        /// <summary>
        /// Default <see cref="IGroupShape"/> name.
        /// </summary>
        public const string DefaulGroupName = "Group";

        /// <summary>
        /// Default <see cref="ILibrary{ShapeStyle}"/> name.
        /// </summary>
        public const string DefaulStyleLibraryName = "Styles";

        /// <summary>
        /// Default <see cref="IShapeStyle"/> name.
        /// </summary>
        public const string DefaulStyleName = "Style";

        /// <summary>
        /// Default imported <see cref="ILibrary{ShapeStyle}"/> name.
        /// </summary>
        public const string ImportedStyleLibraryName = "Imported";

        /// <summary>
        /// Default imported <see cref="IDatabase"/> name.
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
        /// Xaml file extension.
        /// </summary>
        public const string XamlExtension = ".xaml";

        /// <summary>
        /// Json file extension.
        /// </summary>
        public const string JsonExtension = ".json";

        /// <summary>
        /// Script file extension.
        /// </summary>
        public const string ScriptExtension = ".csx";
    }
}
