// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Project extension methods.
    /// </summary>
    public static class ProjectExtensions
    {
        /// <summary>
        /// Add document.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        public static void AddDocument(this XProject project, XDocument document)
        {
            if (project?.Documents != null && document != null)
            {
                var previous = project.Documents;
                var next = project.Documents.Add(document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        /// <summary>
        /// Add document at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        /// <param name="index">The document index.</param>
        public static void AddDocumentAt(this XProject project, XDocument document, int index)
        {
            if (project?.Documents != null && document != null && index >= 0)
            {
                var previous = project.Documents;
                var next = project.Documents.Insert(index, document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        /// <summary>
        /// Remove document object from project <see cref="XProject.Documents"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document object to remove from project <see cref="XProject.Documents"/> collection.</param>
        public static void RemoveDocument(this XProject project, XDocument document)
        {
            if (project?.Documents != null && document != null)
            {
                var previous = project.Documents;
                var next = project.Documents.Remove(document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        /// <summary>
        /// Replace document at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        /// <param name="index">The document index.</param>
        public static void ReplaceDocument(this XProject project, XDocument document, int index)
        {
            if (document != null && index >= 0)
            {
                var builder = project.Documents.ToBuilder();
                builder[index] = document;

                var previous = project.Documents;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        /// <summary>
        /// Add page.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        /// <param name="page">The page instance.</param>
        public static void AddPage(this XProject project, XDocument document, XContainer page)
        {
            if (document != null && page != null)
            {
                var previous = document.Pages;
                var next = document.Pages.Add(page);
                project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        /// <summary>
        /// Add page at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        /// <param name="page">The page instance.</param>
        /// <param name="index">The page index.</param>
        public static void AddPageAt(this XProject project, XDocument document, XContainer page, int index)
        {
            if (document != null && page != null && index >= 0)
            {
                var previous = document.Pages;
                var next = document.Pages.Insert(index, page);
                project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        /// <summary>
        /// Remove page object from owner document <see cref="XDocument.Pages"/> collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="page">The page object to remove from document <see cref="XDocument.Pages"/> collection.</param>
        /// <returns>The owner document.</returns>
        public static XDocument RemovePage(this XProject project, XContainer page)
        {
            if (project?.Documents != null && page != null)
            {
                var document = project.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                if (document != null)
                {
                    var previous = document.Pages;
                    var next = document.Pages.Remove(page);
                    project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                    document.Pages = next;
                }
                return document;
            }
            return null;
        }

        /// <summary>
        /// Replace page at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="document">The document instance.</param>
        /// <param name="page">The page instance.</param>
        /// <param name="index">The page index.</param>
        public static void ReplacePage(this XProject project, XDocument document, XContainer page, int index)
        {
            if (document != null && page != null && index >= 0)
            {
                var builder = document.Pages.ToBuilder();
                builder[index] = page;

                var previous = document.Pages;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        /// <summary>
        /// Add template.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="template">The template instance.</param>
        public static void AddTemplate(this XProject project, XContainer template)
        {
            if (project?.Templates != null && template != null)
            {
                var previous = project.Templates;
                var next = project.Templates.Add(template);
                project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
        }

        /// <summary>
        /// Add templates.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="templates">The templates collection.</param>
        public static void AddTemplates(this XProject project, IEnumerable<XContainer> templates)
        {
            if (project?.Templates != null && templates != null)
            {
                var builder = project.Templates.ToBuilder();
                builder.AddRange(templates);

                var previous = project.Templates;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
        }

        /// <summary>
        /// Remove template.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="template">The template instance</param>
        public static void RemoveTemplate(this XProject project, XContainer template)
        {
            if (project?.Templates != null && template != null)
            {
                var previous = project.Templates;
                var next = project.Templates.Remove(template);
                project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
        }

        /// <summary>
        /// Set page template.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="page">The page instance.</param>
        /// <param name="template">The template instance.</param>
        public static void ApplyTemplate(this XProject project, XContainer page, XContainer template)
        {
            if (page != null && template != null)
            {
                var previous = page.Template;
                var next = template;
                project?.History?.Snapshot(previous, next, (p) => page.Template = p);
                page.Template = next;
            }
        }

        /// <summary>
        /// Add layer.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="container">The container instance.</param>
        /// <param name="layer">The layer instance.</param>
        public static void AddLayer(this XProject project, XContainer container, XLayer layer)
        {
            if (container != null && container.Layers != null && layer != null)
            {
                var previous = container.Layers;
                var next = container.Layers.Add(layer);
                project?.History?.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
        }

        /// <summary>
        /// Remove layer.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        public static void RemoveLayer(this XProject project, XLayer layer)
        {
            var container = layer?.Owner;
            if (container != null && container.Layers != null)
            {
                var previous = container.Layers;
                var next = container.Layers.Remove(layer);
                project?.History?.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
        }

        /// <summary>
        /// Clear layer.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        public static void ClearLayer(this XProject project, XLayer layer)
        {
            if (layer != null)
            {
                var previous = layer.Shapes;
                var next = ImmutableArray.Create<BaseShape>();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Add shape.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        /// <param name="shape">The shape instance.</param>
        public static void AddShape(this XProject project, XLayer layer, BaseShape shape)
        {
            if (layer != null && layer.Shapes != null && shape != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Add(shape);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Add shape at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        /// <param name="shape">The shape instance.</param>
        /// <param name="index">The shape index.</param>
        public static void AddShapeAt(this XProject project, XLayer layer, BaseShape shape, int index)
        {
            if (layer?.Shapes != null && shape != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Insert(index, shape);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Add shapes.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        /// <param name="shapes">The shapes collection.</param>
        public static void AddShapes(this XProject project, XLayer layer, IEnumerable<BaseShape> shapes)
        {
            if (layer?.Shapes != null && shapes != null)
            {
                var builder = layer.Shapes.ToBuilder();
                builder.AddRange(shapes);

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Remove shape.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        /// <param name="shape">The shape instance.</param>
        public static void RemoveShape(this XProject project, XLayer layer, BaseShape shape)
        {
            if (layer?.Shapes != null && shape != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(shape);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Remove shape.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape">The shape instance.</param>
        /// <returns>The owner layer.</returns>
        public static XLayer RemoveShape(this XProject project, BaseShape shape)
        {
            if (project?.Documents != null && shape != null)
            {
                var layer = project.Documents.SelectMany(d => d.Pages).SelectMany(p => p.Layers).FirstOrDefault(l => l.Shapes.Contains(shape));
                if (layer != null)
                {
                    project.RemoveShape(layer, shape);
                }
                return layer;
            }
            return null;
        }

        /// <summary>
        /// Replace shape at specified index.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        /// <param name="shape">The shape instance.</param>
        /// <param name="index">The shape index.</param>
        public static void ReplaceShape(this XProject project, XLayer layer, BaseShape shape, int index)
        {
            if (layer != null && shape != null && index >= 0)
            {
                var builder = layer.Shapes.ToBuilder();
                builder[index] = shape;

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Swap shape at specified indexes.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="layer">The layer instance.</param>
        /// <param name="shape">The shape instance.</param>
        /// <param name="insertIndex">The shape insert index.</param>
        /// <param name="removeIndex">The shape remove index.</param>
        public static void SwapShape(this XProject project, XLayer layer, BaseShape shape, int insertIndex, int removeIndex)
        {
            if (layer != null && shape != null && insertIndex >= 0 && removeIndex >= 0)
            {
                var builder = layer.Shapes.ToBuilder();
                builder.Insert(insertIndex, shape);
                builder.RemoveAt(removeIndex);

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        /// <summary>
        /// Add property.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="data">The data instance.</param>
        /// <param name="property">The property instance.</param>
        public static void AddProperty(this XProject project, XContext data, XProperty property)
        {
            if (data?.Properties != null && property != null)
            {
                var previous = data.Properties;
                var next = data.Properties.Add(property);
                project?.History?.Snapshot(previous, next, (p) => data.Properties = p);
                data.Properties = next;
            }
        }

        /// <summary>
        /// Remove property.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="property">The property instance.</param>
        public static void RemoveProperty(this XProject project, XProperty property)
        {
            var data = property?.Owner;
            if (data != null && data.Properties != null)
            {
                var previous = data.Properties;
                var next = data.Properties.Remove(property);
                project?.History?.Snapshot(previous, next, (p) => data.Properties = p);
                data.Properties = next;
            }
        }

        /// <summary>
        /// Add database.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="db">The database instance.</param>
        public static void AddDatabase(this XProject project, XDatabase db)
        {
            if (project?.Databases != null && db != null)
            {
                var previous = project.Databases;
                var next = project.Databases.Add(db);
                project?.History?.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;
            }
        }

        /// <summary>
        /// Remove database.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="db">The <see cref="XDatabase"/> to remove.</param>
        public static void RemoveDatabase(this XProject project, XDatabase db)
        {
            if (project?.Databases != null && db != null)
            {
                var previous = project.Databases;
                var next = project.Databases.Remove(db);
                project?.History?.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;
            }
        }

        /// <summary>
        /// Update the destination database using data from source database.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="destination">The destination database.</param>
        /// <param name="source">The source database.</param>
        public static void UpdateDatabase(this XProject project, XDatabase destination, XDatabase source)
        {
            if (destination != null && source != null)
            {
                bool isDirty = XDatabase.Update(destination, source, out ImmutableArray<XRecord>.Builder records);

                if (isDirty && records != null)
                {
                    var builder = project.Databases.ToBuilder();
                    var index = builder.IndexOf(destination);
                    destination.Records = records.ToImmutable();
                    builder[index] = destination;

                    var previous = project.Databases;
                    var next = builder.ToImmutable();
                    project?.History?.Snapshot(previous, next, (p) => project.Databases = p);
                    project.Databases = next;
                }
            }
        }

        /// <summary>
        /// Add column to database columns collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="db">The database instance.</param>
        /// <param name="column">The column instance.</param>
        public static void AddColumn(this XProject project, XDatabase db, XColumn column)
        {
            if (db?.Columns != null && column != null)
            {
                var previous = db.Columns;
                var next = db.Columns.Add(column);
                project?.History?.Snapshot(previous, next, (p) => db.Columns = p);
                db.Columns = next;
            }
        }

        /// <summary>
        /// Remove column from database columns collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="column">The <see cref="XColumn"/> to remove.</param>
        public static void RemoveColumn(this XProject project, XColumn column)
        {
            var db = column?.Owner;
            if (db != null && db.Columns != null)
            {
                var previous = db.Columns;
                var next = db.Columns.Remove(column);
                project?.History?.Snapshot(previous, next, (p) => db.Columns = p);
                db.Columns = next;
            }
        }

        /// <summary>
        /// Add record to database records collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="db">The database instance.</param>
        /// <param name="record">The record instance.</param>
        public static void AddRecord(this XProject project, XDatabase db, XRecord record)
        {
            if (db?.Records != null)
            {
                var previous = db.Records;
                var next = db.Records.Add(record);
                project?.History?.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
        }

        /// <summary>
        /// Remove record from database records collection.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="record">The record instance.</param>
        public static void RemoveRecord(this XProject project, XRecord record)
        {
            var db = record?.Owner;
            if (db != null && db.Records != null)
            {
                var previous = db.Records;
                var next = db.Records.Remove(record);
                project?.History?.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
        }

        /// <summary>
        /// Reset data record.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="data">The data instance.</param>
        public static void ResetRecord(this XProject project, XContext data)
        {
            var record = data?.Record;
            if (record != null)
            {
                var previous = record;
                var next = default(XRecord);
                project?.History?.Snapshot(previous, next, (p) => data.Record = p);
                data.Record = next;
            }
        }

        /// <summary>
        /// Set data record.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="data">The data instance.</param>
        /// <param name="record">The record instance.</param>
        public static void ApplyRecord(this XProject project, XContext data, XRecord record)
        {
            if (data != null && record != null)
            {
                var previous = data.Record;
                var next = record;
                project?.History?.Snapshot(previous, next, (p) => data.Record = p);
                data.Record = next;
            }
        }

        /// <summary>
        /// Set shape data.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape">The shape instance.</param>
        /// <param name="data">The data instance.</param>
        public static void ApplyData(this XProject project, BaseShape shape, XContext data)
        {
            if (shape != null && data != null)
            {
                var previous = shape.Data;
                var next = data;
                project?.History?.Snapshot(previous, next, (p) => shape.Data = p);
                shape.Data = data;
            }
        }

        /// <summary>
        /// Add group library.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The group library instance.</param>
        public static void AddGroupLibrary(this XProject project, XLibrary<GroupShape> library)
        {
            if (project?.GroupLibraries != null && library != null)
            {
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Add(library);
                project?.History?.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        /// <summary>
        /// Add group libraries.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="libraries">The group libraries collection.</param>
        public static void AddGroupLibraries(this XProject project, IEnumerable<XLibrary<GroupShape>> libraries)
        {
            if (project?.GroupLibraries != null && libraries != null)
            {
                var builder = project.GroupLibraries.ToBuilder();
                builder.AddRange(libraries);

                var previous = project.GroupLibraries;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        /// <summary>
        /// Remove group library.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The group library instance.</param>
        public static void RemoveGroupLibrary(this XProject project, XLibrary<GroupShape> library)
        {
            if (project?.GroupLibraries != null && library != null)
            {
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Remove(library);
                project?.History?.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        /// <summary>
        /// Add style library.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The style library instance.</param>
        public static void AddStyleLibrary(this XProject project, XLibrary<ShapeStyle> library)
        {
            if (project?.StyleLibraries != null && library != null)
            {
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Add(library);
                project?.History?.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        /// <summary>
        /// Add style libraries.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="libraries">The style libraries collection.</param>
        public static void AddStyleLibraries(this XProject project, IEnumerable<XLibrary<ShapeStyle>> libraries)
        {
            if (project?.StyleLibraries != null && libraries != null)
            {
                var builder = project.StyleLibraries.ToBuilder();
                builder.AddRange(libraries);

                var previous = project.StyleLibraries;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        /// <summary>
        /// Remove style library.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The style library instance.</param>
        public static void RemoveStyleLibrary(this XProject project, XLibrary<ShapeStyle> library)
        {
            if (project?.CurrentStyleLibrary != null && library != null)
            {
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Remove(library);
                project?.History?.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        /// <summary>
        /// Add style.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The style library instance.</param>
        /// <param name="style">The style instance.</param>
        public static void AddStyle(this XProject project, XLibrary<ShapeStyle> library, ShapeStyle style)
        {
            AddItem(project, library, style);
        }

        /// <summary>
        /// Remove style.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="style">The style instance.</param>
        /// <returns>The owner style library.</returns>
        public static XLibrary<ShapeStyle> RemoveStyle(this XProject project, ShapeStyle style)
        {
            if (project?.StyleLibraries != null && style != null)
            {
                var library = project.StyleLibraries.FirstOrDefault(l => l.Items.Contains(style));
                if (library?.Items != null)
                {
                    var previous = library.Items;
                    var next = library.Items.Remove(style);
                    project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                    library.Items = next;
                }
                return library;
            }
            return null;
        }

        /// <summary>
        /// Set shape style.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="shape">The shape instance.</param>
        /// <param name="style">The style instance.</param>
        public static void ApplyStyle(this XProject project, BaseShape shape, ShapeStyle style)
        {
            if (shape != null && style != null)
            {
                if (shape is GroupShape)
                {
                    var shapes = XProject.GetAllShapes((shape as GroupShape).Shapes);
                    foreach (var child in shapes)
                    {
                        var previous = child.Style;
                        var next = style;
                        project?.History?.Snapshot(previous, next, (p) => child.Style = p);
                        child.Style = next;
                    }
                }
                else
                {
                    var previous = shape.Style;
                    var next = style;
                    project?.History?.Snapshot(previous, next, (p) => shape.Style = p);
                    shape.Style = next;
                }
            }
        }

        /// <summary>
        /// Add group.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The group library instance.</param>
        /// <param name="group">The group instance.</param>
        public static void AddGroup(this XProject project, XLibrary<GroupShape> library, GroupShape group)
        {
            AddItem(project, library, group);
        }

        /// <summary>
        /// Remove group.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="group">The group instance.</param>
        /// <returns>The owner group library.</returns>
        public static XLibrary<GroupShape> RemoveGroup(this XProject project, GroupShape group)
        {
            if (project?.GroupLibraries != null && group != null)
            {
                var library = project.GroupLibraries.FirstOrDefault(l => l.Items.Contains(group));
                if (library?.Items != null)
                {
                    var previous = library.Items;
                    var next = library.Items.Remove(group);
                    project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                    library.Items = next;
                }
                return library;
            }
            return null;
        }

        /// <summary>
        /// Add item.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The library instance.</param>
        /// <param name="item">The item instance.</param>
        public static void AddItem<T>(this XProject project, XLibrary<T> library, T item)
        {
            if (library?.Items != null && item != null)
            {
                var previous = library.Items;
                var next = library.Items.Add(item);
                project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                library.Items = next;
            }
        }

        /// <summary>
        /// Add items.
        /// </summary>
        /// <param name="project">The project instance.</param>
        /// <param name="library">The library instance.</param>
        /// <param name="items">The items collection.</param>
        public static void AddItems<T>(this XProject project, XLibrary<T> library, IEnumerable<T> items)
        {
            if (library?.Items != null && items != null)
            {
                var builder = library.Items.ToBuilder();
                builder.AddRange(items);

                var previous = library.Items;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                library.Items = next;
            }
        }
    }
}
