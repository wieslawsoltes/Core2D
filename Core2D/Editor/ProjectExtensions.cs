// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// The <see cref="Project"/> extension methods.
    /// </summary>
    public static class ProjectExtensions
    {
        /// <summary>
        /// Add document.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        public static void AddDocument(this Project project, Document document)
        {
            var previous = project.Documents;
            var next = project.Documents.Add(document);
            project.History.Snapshot(previous, next, (p) => project.Documents = p);
            project.Documents = next;
        }

        /// <summary>
        /// Add document at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        /// <param name="index">The document index.</param>
        public static void AddDocumentAt(this Project project, Document document, int index)
        {
            var previous = project.Documents;
            var next = project.Documents.Insert(index, document);
            project.History.Snapshot(previous, next, (p) => project.Documents = p);
            project.Documents = next;
        }

        /// <summary>
        /// Remove document object from project <see cref="Project.Documents"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document object to remove from project <see cref="Project.Documents"/> collection.</param>
        public static void RemoveDocument(this Project project, Document document)
        {
            if (project != null && project.Documents != null)
            {
                var previous = project.Documents;
                var next = project.Documents.Remove(document);
                project.History.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;

                project.CurrentDocument = project.Documents.FirstOrDefault();
                if (project.CurrentDocument != null)
                {
                    project.CurrentContainer = project.CurrentDocument.Pages.FirstOrDefault();
                }
                else
                {
                    project.CurrentContainer = default(Container);
                }
                project.Selected = project.CurrentContainer;
            }
        }

        /// <summary>
        /// Add page.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="page">The page instance.</param>
        public static void AddContainer(this Project project, Page page)
        {
            var document = project.CurrentDocument;
            if (document != null)
            {
                var previous = document.Pages;
                var next = document.Pages.Add(page);
                project.History.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        /// <summary>
        /// Add page at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="page">The page instance.</param>
        /// <param name="index">The page index.</param>
        public static void AddPageAt(this Project project, Page page, int index)
        {
            var document = project.CurrentDocument;
            if (document != null)
            {
                var previous = document.Pages;
                var next = document.Pages.Insert(index, page);
                project.History.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        /// <summary>
        /// Remove page object from owner document <see cref="Document.Pages"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="page">The page object to remove from document <see cref="Document.Pages"/> collection.</param>
        public static void RemovePage(this Project project, Page page)
        {
            if (project != null && project.Documents != null)
            {
                var document = project.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                if (document != null)
                {
                    var previous = document.Pages;
                    var next = document.Pages.Remove(page);
                    project.History.Snapshot(previous, next, (p) => document.Pages = p);
                    document.Pages = next;

                    project.CurrentDocument = document;
                    project.CurrentContainer = document.Pages.FirstOrDefault();
                    project.Selected = project.CurrentContainer;
                }
            }
        }

        /// <summary>
        /// Add template.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="template">The template instance.</param>
        public static void AddTemplate(this Project project, Template template)
        {
            var previous = project.Templates;
            var next = project.Templates.Add(template);
            project.History.Snapshot(previous, next, (p) => project.Templates = p);
            project.Templates = next;
        }

        /// <summary>
        /// Remove the <see cref="Project.CurrentTemplate"/> object from the <see cref="Project.Templates"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveTemplate(this Project project)
        {
            var template = project.CurrentTemplate;
            if (template != null)
            {
                var previous = project.Templates;
                var next = project.Templates.Remove(project.CurrentTemplate);
                project.History.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;

                project.CurrentTemplate = project.Templates.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="template"></param>
        public static void ApplyTemplate(this Project project, Template template)
        {
            var container = project.CurrentContainer;
            if (container != null && container is Page)
            {
                var page = container as Page;
                var previous = page.Template;
                var next = template;
                project.History.Snapshot(previous, next, (p) => page.Template = p);
                page.Template = next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="name"></param>
        public static void AddLayer(this Project project, string name = "New")
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = Layer.Create(name, container);
                var previous = container.Layers;
                var next = container.Layers.Add(layer);
                project.History.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer"></param>
        public static void AddLayer(this Project project, Layer layer)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var previous = container.Layers;
                var next = container.Layers.Add(layer);
                project.History.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
        }

        /// <summary>
        /// Remove the <see cref="Project.CurrentContainer"/> <see cref="Container.CurrentLayer"/> object from the <see cref="Project.CurrentContainer"/> <see cref="Container.Layers"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveLayer(this Project project)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var previous = container.Layers;
                    var next = container.Layers.Remove(layer);
                    project.History.Snapshot(previous, next, (p) => container.Layers = p);
                    container.Layers = next;

                    project.CurrentContainer.CurrentLayer = project.CurrentContainer.Layers.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape"></param>
        public static void AddShape(this Project project, BaseShape shape)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var previous = layer.Shapes;
                    var next = layer.Shapes.Add(shape);
                    project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;
                }
            }
        }

        /// <summary>
        /// Remove the <see cref="Project.CurrentContainer"/> <see cref="Container.CurrentShape"/> object from the <see cref="Project.CurrentContainer"/> <see cref="Container.CurrentLayer"/> <see cref="Layer.Shapes"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveShape(this Project project)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var shape = container.CurrentShape;
                var layer = container.CurrentLayer;
                if (shape != null && layer != null)
                {
                    var previous = layer.Shapes;
                    var next = layer.Shapes.Remove(shape);
                    project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    project.CurrentContainer.CurrentShape = project.CurrentContainer.CurrentLayer.Shapes.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="data"></param>
        /// <param name="property"></param>
        public static void AddProperty(this Project project, Data data, Property property)
        {
            var previous = data.Properties;
            var next = data.Properties.Add(property);
            project.History.Snapshot(previous, next, (p) => data.Properties = p);
            data.Properties = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="data"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AddProperty(this Project project, Data data, string name = "New", string value = "")
        {
            if (data != null)
            {
                if (data.Properties == null)
                {
                    data.Properties = ImmutableArray.Create<Property>();
                }

                project.AddProperty(data, Property.Create(name, value, data));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="property"></param>
        public static void RemoveProperty(this Project project, Property property)
        {
            if (property != null)
            {
                var owner = property.Owner;
                if (owner is Data)
                {
                    var data = owner;
                    if (data.Properties != null)
                    {
                        var previous = data.Properties;
                        var next = data.Properties.Remove(property);
                        project.History.Snapshot(previous, next, (p) => data.Properties = p);
                        data.Properties = next;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="name"></param>
        /// <param name="columns"></param>
        public static void AddDatabase(this Project project, string name = "Db", int columns = 2)
        {
            var db = Database.Create(name);

            var builder = ImmutableArray.CreateBuilder<Column>();
            for (int i = 0; i < columns; i++)
            {
                builder.Add(Column.Create("Column" + i, db));
            }
            db.Columns = builder.ToImmutable();

            var previous = project.Databases;
            var next = project.Databases.Add(db);
            project.History.Snapshot(previous, next, (p) => project.Databases = p);
            project.Databases = next;

            project.CurrentDatabase = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="db"></param>
        public static void AddDatabase(this Project project, Database db)
        {
            var previous = project.Databases;
            var next = project.Databases.Add(db);
            project.History.Snapshot(previous, next, (p) => project.Databases = p);
            project.Databases = next;
        }

        /// <summary>
        /// Remove the <see cref="Database"/> object from the <see cref="Project.Databases"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="db">The <see cref="Database"/> to remove.</param>
        public static void RemoveDatabase(this Project project, Database db)
        {
            if (db != null)
            {
                var previous = project.Databases;
                var next = project.Databases.Remove(db);
                project.History.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;

                project.CurrentDatabase = project.Databases.FirstOrDefault();
            }
        }

        /// <summary>
        /// Update the destination <see cref="Database"/> using data from source <see cref="Database"/>.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="destination">The destination database.</param>
        /// <param name="source">The source database.</param>
        public static void UpdateDatabase(this Project project, Database destination, Database source)
        {
            if (source.Columns.Length <= 1)
                return;

            // Check for the Id column.
            if (source.Columns[0].Name != Database.IdColumnName)
                return;

            // Check columns length.
            if (source.Columns.Length - 1 != destination.Columns.Length)
                return;

            // Check column names.
            for (int i = 1; i < source.Columns.Length; i++)
            {
                if (source.Columns[i].Name != destination.Columns[i - 1].Name)
                {
                    return;
                }
            }

            bool isDirty = false;
            var recordsBuilder = destination.Records.ToBuilder();

            // Update or remove existing records.
            for (int i = 0; i < destination.Records.Length; i++)
            {
                var record = destination.Records[i];
                var result = source.Records.FirstOrDefault(r => r.Id == record.Id);
                if (result != null)
                {
                    // Update existing record.
                    for (int j = 1; j < result.Values.Length; j++)
                    {
                        var valuesBuilder = record.Values.ToBuilder();
                        valuesBuilder[j - 1] = result.Values[j];
                        record.Values = valuesBuilder.ToImmutable();
                    }
                    isDirty = true;
                }
                else
                {
                    // Remove existing record.
                    recordsBuilder.Remove(record);
                    isDirty = true;
                }
            }

            // Add new records.
            for (int i = 0; i < source.Records.Length; i++)
            {
                var record = source.Records[i];
                var result = destination.Records.FirstOrDefault(r => r.Id == record.Id);
                if (result == null)
                {
                    var r = source.Records[i];

                    // Use existing columns.
                    r.Columns = destination.Columns;

                    // Skip Id column.
                    r.Values = r.Values.Skip(1).ToImmutableArray();

                    recordsBuilder.Add(r);
                    isDirty = true;
                }
            }

            if (isDirty)
            {
                var builder = project.Databases.ToBuilder();
                var index = builder.IndexOf(destination);
                destination.Records = recordsBuilder.ToImmutable();
                builder[index] = destination;

                var previous = project.Databases;
                var next = builder.ToImmutable();
                project.History.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="db"></param>
        /// <param name="name"></param>
        public static void AddColumn(this Project project, Database db, string name = "Column")
        {
            if (db != null)
            {
                if (db.Columns == null)
                {
                    db.Columns = ImmutableArray.Create<Column>();
                }

                var previous = db.Columns;
                var next = db.Columns.Add(Column.Create(name + db.Columns.Length, db));
                project.History.Snapshot(previous, next, (p) => db.Columns = p);
                db.Columns = next;
            }
        }

        /// <summary>
        /// Remove the <see cref="Column"/> object from <see cref="Column.Owner"/> <see cref="Database.Columns"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="column">The <see cref="Column"/> to remove.</param>
        public static void RemoveColumn(this Project project, Column column)
        {
            if (column != null)
            {
                var db = column.Owner;
                if (db != null && db.Columns != null)
                {
                    var previous = db.Columns;
                    var next = db.Columns.Remove(column);
                    project.History.Snapshot(previous, next, (p) => db.Columns = p);
                    db.Columns = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void AddRecord(this Project project)
        {
            if (project == null || project.CurrentDatabase == null)
                return;

            var db = project.CurrentDatabase;

            var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => Value.Create(c));
            var record = Record.Create(
                db.Columns,
                ImmutableArray.CreateRange<Value>(values),
                db);

            var previous = db.Records;
            var next = db.Records.Add(record);
            project.History.Snapshot(previous, next, (p) => db.Records = p);
            db.Records = next;
        }

        /// <summary>
        /// Remove the <see cref="Project.CurrentDatabase"/> <see cref="Database.CurrentRecord"/> object from the <see cref="Project.CurrentDatabase"/> <see cref="Database.Records"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveRecord(this Project project)
        {
            var db = project.CurrentDatabase;
            if (db != null)
            {
                var record = db.CurrentRecord;
                if (record != null)
                {
                    var previous = db.Records;
                    var next = db.Records.Remove(record);
                    project.History.Snapshot(previous, next, (p) => db.Records = p);
                    db.Records = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="data"></param>
        public static void ResetRecord(this Project project, Data data)
        {
            if (data != null)
            {
                var record = data.Record;
                if (record != null)
                {
                    var previous = record;
                    var next = default(Record);
                    project.History.Snapshot(previous, next, (p) => data.Record = p);
                    data.Record = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="record">The record instance.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        public static void ApplyRecord(this Project project, Record record, BaseShape shape, ImmutableHashSet<BaseShape> shapes)
        {
            if (project == null || project.CurrentContainer == null)
                return;

            var container = project.CurrentContainer;
            if (container != null)
            {
                // Selected shape.
                if (shape != null)
                {
                    var previous = shape.Data.Record;
                    var next = record;
                    project.History.Snapshot(previous, next, (p) => shape.Data.Record = p);
                    shape.Data.Record = next;
                }

                // Selected shapes.
                if (shapes != null && shapes.Count > 0)
                {
                    foreach (var s in shapes)
                    {
                        var previous = s.Data.Record;
                        var next = record;
                        project.History.Snapshot(previous, next, (p) => s.Data.Record = p);
                        s.Data.Record = next;
                    }
                }

                // Current page.
                if (shape == null && shapes == null)
                {
                    var page = container as Page;
                    if (page != null)
                    {
                        var previous = page.Data.Record;
                        var next = record;
                        project.History.Snapshot(previous, next, (p) => page.Data.Record = p);
                        page.Data.Record = next;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape"></param>
        /// <param name="record"></param>
        public static void ApplyRecord(this Project project, BaseShape shape, Record record)
        {
            var previous = shape.Data.Record;
            var next = record;
            project.History.Snapshot(previous, next, (p) => shape.Data.Record = p);
            shape.Data.Record = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="record"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void ApplyRecord(this Project project, Record record, double x, double y)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var result = ShapeBounds.HitTest(container, new Vector2(x, y), project.Options.HitTreshold);
                if (result != null)
                {
                    var previous = result.Data.Record;
                    var next = record;
                    project.History.Snapshot(previous, next, (p) => result.Data.Record = p);
                    result.Data.Record = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="name"></param>
        public static void AddGroupLibrary(this Project project, string name = "New")
        {
            if (project.GroupLibraries != null)
            {
                var gl = Library<XGroup>.Create(name);
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Add(gl);
                project.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        /// <summary>
        /// Remove the <see cref="Project.CurrentGroupLibrary"/> object from the <see cref="Project.GroupLibraries"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveGroupLibrary(this Project project)
        {
            var gl = project.CurrentGroupLibrary;
            if (gl != null)
            {
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Remove(gl);
                project.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;

                project.CurrentGroupLibrary = project.GroupLibraries.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="name"></param>
        public static void AddStyleLibrary(this Project project, string name = "New")
        {
            if (project.StyleLibraries != null)
            {
                var sl = Library<ShapeStyle>.Create(name);
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Add(sl);
                project.History.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        /// <summary>
        /// Remove the <see cref="Project.CurrentStyleLibrary"/> object from the <see cref="Project.StyleLibraries"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveStyleLibrary(this Project project)
        {
            var sl = project.CurrentStyleLibrary;
            if (sl != null)
            {
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Remove(sl);
                project.History.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;

                project.CurrentStyleLibrary = project.StyleLibraries.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="name"></param>
        public static void AddStyle(this Project project, string name = "New")
        {
            var sl = project.CurrentStyleLibrary;
            if (sl != null)
            {
                var previous = sl.Items;
                var next = sl.Items.Add(ShapeStyle.Create(name));
                project.History.Snapshot(previous, next, (p) => sl.Items = p);
                sl.Items = next;
            }
        }

        /// <summary>
        /// Removes the <see cref="Project.CurrentStyleLibrary"/> <see cref="Library{ShapeStyle}.Selected"/> object from the <see cref="Project.CurrentStyleLibrary"/> <see cref="Library{ShapeStyle}.Items"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveStyle(this Project project)
        {
            var sl = project.CurrentStyleLibrary;
            if (sl != null)
            {
                var style = sl.Selected;
                if (style != null)
                {
                    var previous = sl.Items;
                    var next = sl.Items.Remove(style);
                    project.History.Snapshot(previous, next, (p) => sl.Items = p);
                    sl.Items = next;

                    project.CurrentStyleLibrary.Selected = project.CurrentStyleLibrary.Items.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape"></param>
        /// <param name="style"></param>
        public static void ApplyStyle(this Project project, BaseShape shape, ShapeStyle style)
        {
            if (shape != null)
            {
                if (shape is XGroup)
                {
                    var shapes = Editor.GetAllShapes((shape as XGroup).Shapes);
                    foreach (var child in shapes)
                    {
                        var previous = child.Style;
                        var next = style;
                        project.History.Snapshot(previous, next, (p) => child.Style = p);
                        child.Style = next;
                    }
                }
                else
                {
                    var previous = shape.Style;
                    var next = style;
                    project.History.Snapshot(previous, next, (p) => shape.Style = p);
                    shape.Style = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="style">The style instance.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        public static void ApplyStyle(this Project project, ShapeStyle style, BaseShape shape, ImmutableHashSet<BaseShape> shapes)
        {
            if (shape != null)
            {
                project.ApplyStyle(shape, style);
            }

            if (shapes != null && shapes.Count > 0)
            {
                foreach (var s in shapes)
                {
                    project.ApplyStyle(s, style);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="style"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void ApplyStyle(this Project project, ShapeStyle style, double x, double y)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var result = ShapeBounds.HitTest(container, new Vector2(x, y), project.Options.HitTreshold);
                if (result != null)
                {
                    project.ApplyStyle(result, style);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="group"></param>
        public static void AddGroup(this Project project, XGroup group)
        {
            var gl = project.CurrentGroupLibrary;
            if (gl != null)
            {
                var previous = gl.Items;
                var next = gl.Items.Add(group);
                project.History.Snapshot(previous, next, (p) => gl.Items = p);
                gl.Items = next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="name"></param>
        public static void AddGroup(this Project project, string name = "New")
        {
            var gl = project.CurrentGroupLibrary;
            if (gl != null)
            {
                var group = XGroup.Create(name);
                var previous = gl.Items;
                var next = gl.Items.Add(group);
                project.History.Snapshot(previous, next, (p) => gl.Items = p);
                gl.Items = next;
            }
        }

        /// <summary>
        /// Remove the <see cref="Project.CurrentGroupLibrary"/> <see cref="Library{XGroup}.Selected"/> object from the <see cref="Project.CurrentGroupLibrary"/> <see cref="Library{XGroup}.Items"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        public static void RemoveGroup(this Project project)
        {
            var gl = project.CurrentGroupLibrary;
            if (gl != null)
            {
                var group = gl.Selected;
                if (group != null)
                {
                    var previous = gl.Items;
                    var next = gl.Items.Remove(group);
                    project.History.Snapshot(previous, next, (p) => gl.Items = p);
                    gl.Items = next;

                    project.CurrentGroupLibrary.Selected = project.CurrentGroupLibrary.Items.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="name">The group name.</param>
        public static XGroup Group(this Project project, ImmutableHashSet<BaseShape> shapes, string name = "g")
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    // TODO: Group method changes SelectedShapes State properties.
                    var g = XGroup.Group(name, shapes);

                    var builder = layer.Shapes.ToBuilder();
                    foreach (var shape in shapes)
                    {
                        builder.Remove(shape);
                    }
                    builder.Add(g);

                    var previous = layer.Shapes;
                    var next = builder.ToImmutable();
                    project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    return g;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="original"></param>
        /// <param name="groupShapes"></param>
        private static void Ungroup(IEnumerable<BaseShape> shapes, IList<BaseShape> original, bool groupShapes)
        {
            if (shapes != null)
            {
                foreach (var shape in shapes)
                {
                    if (shape is XGroup)
                    {
                        var g = shape as XGroup;
                        Ungroup(g.Shapes, original, groupShapes: true);
                        Ungroup(g.Connectors, original, groupShapes: true);
                        original.Remove(g);
                    }
                    else if (groupShapes)
                    {
                        if (shape is XPoint)
                        {
                            shape.State.Flags &=
                                ~(ShapeStateFlags.Connector
                                | ShapeStateFlags.None
                                | ShapeStateFlags.Input
                                | ShapeStateFlags.Output);
                            shape.State.Flags |= ShapeStateFlags.Standalone;
                            original.Add(shape);
                        }
                        else
                        {
                            shape.State.Flags |= ShapeStateFlags.Standalone;
                            original.Add(shape);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        public static bool Ungroup(this Project project, BaseShape shape, ImmutableHashSet<BaseShape> shapes)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    if (shape != null && shape is XGroup && layer != null)
                    {
                        var builder = layer.Shapes.ToBuilder();

                        var g = shape as XGroup;
                        Ungroup(g.Shapes, builder, groupShapes: true);
                        Ungroup(g.Connectors, builder, groupShapes: true);
                        builder.Remove(g);

                        var previous = layer.Shapes;
                        var next = builder.ToImmutable();
                        project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                        layer.Shapes = next;

                        return true;
                    }

                    if (shapes != null && layer != null)
                    {
                        var builder = layer.Shapes.ToBuilder();

                        Ungroup(shapes, builder, groupShapes: false);

                        var previous = layer.Shapes;
                        var next = builder.ToImmutable();
                        project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                        layer.Shapes = next;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Move shape from source index to target index position in an array. 
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        private static void Move(this Project project, BaseShape source, int sourceIndex, int targetIndex)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    if (items != null)
                    {
                        if (sourceIndex < targetIndex)
                        {
                            var builder = items.ToBuilder();
                            builder.Insert(targetIndex + 1, source);
                            builder.RemoveAt(sourceIndex);

                            var previous = layer.Shapes;
                            var next = builder.ToImmutable();
                            project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                            layer.Shapes = next;
                        }
                        else
                        {
                            int removeIndex = sourceIndex + 1;
                            if (items.Length + 1 > removeIndex)
                            {
                                var builder = items.ToBuilder();
                                builder.Insert(targetIndex, source);
                                builder.RemoveAt(removeIndex);

                                var previous = layer.Shapes;
                                var next = builder.ToImmutable();
                                project.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                                layer.Shapes = next;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bring a shape to the top of the stack.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="source"></param>
        public static void BringToFront(this Project project, BaseShape source)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = items.Length - 1;
                    if (targetIndex >= 0 && sourceIndex != targetIndex)
                    {
                        project.Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move a shape one step closer to the front of the stack.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="source"></param>
        public static void BringForward(this Project project, BaseShape source)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = sourceIndex + 1;
                    if (targetIndex < items.Length)
                    {
                        project.Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move a shape one step down within the stack.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="source"></param>
        public static void SendBackward(this Project project, BaseShape source)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = sourceIndex - 1;
                    if (targetIndex >= 0)
                    {
                        project.Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move a shape to the bottom of the stack.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="source"></param>
        public static void SendToBack(this Project project, BaseShape source)
        {
            var container = project.CurrentContainer;
            if (container != null)
            {
                var layer = container.CurrentLayer;
                if (layer != null)
                {
                    var items = layer.Shapes;
                    int sourceIndex = items.IndexOf(source);
                    int targetIndex = 0;
                    if (sourceIndex != targetIndex)
                    {
                        project.Move(source, sourceIndex, targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Move shape(s) by specified offset.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="shapes">The selected shapes.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        public static void MoveBy(this Project project, BaseShape shape, ImmutableHashSet<BaseShape> shapes, double dx, double dy)
        {
            if (shape != null)
            {
                var state = shape.State;

                switch (project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked))
                            {
                                var items = Enumerable.Repeat(shape, 1);
                                var points = items.SelectMany(s => s.GetPoints()).Distinct().ToList();

                                Editor.MovePointsBy(points, dx, dy);

                                var previous = new { DeltaX = -dx, DeltaY = -dy, Points = points };
                                var next = new { DeltaX = dx, DeltaY = dy, Points = points };
                                project.History.Snapshot(previous, next, (s) => Editor.MovePointsBy(s.Points, s.DeltaX, s.DeltaY));
                            }
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            if (!state.Flags.HasFlag(ShapeStateFlags.Locked) && !state.Flags.HasFlag(ShapeStateFlags.Connector))
                            {
                                var items = Enumerable.Repeat(shape, 1).ToList();

                                Editor.MoveShapesBy(items, dx, dy);

                                var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = items };
                                var next = new { DeltaX = dx, DeltaY = dy, Shapes = items };
                                project.History.Snapshot(previous, next, (s) => Editor.MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
                            }
                        }
                        break;
                }
            }

            if (shapes != null)
            {
                var items = shapes.Where(s => !s.State.Flags.HasFlag(ShapeStateFlags.Locked));

                switch (project.Options.MoveMode)
                {
                    case MoveMode.Point:
                        {
                            var points = items.SelectMany(s => s.GetPoints()).Distinct().ToList();

                            Editor.MovePointsBy(points, dx, dy);

                            var previous = new { DeltaX = -dx, DeltaY = -dy, Points = points };
                            var next = new { DeltaX = dx, DeltaY = dy, Points = points };
                            project.History.Snapshot(previous, next, (s) => Editor.MovePointsBy(s.Points, s.DeltaX, s.DeltaY));
                        }
                        break;
                    case MoveMode.Shape:
                        {
                            Editor.MoveShapesBy(items, dx, dy);

                            var previous = new { DeltaX = -dx, DeltaY = -dy, Shapes = items.ToList() };
                            var next = new { DeltaX = dx, DeltaY = dy, Shapes = items.ToList() };
                            project.History.Snapshot(previous, next, (s) => Editor.MoveShapesBy(s.Shapes, s.DeltaX, s.DeltaY));
                        }
                        break;
                }
            }
        }
    }
}
