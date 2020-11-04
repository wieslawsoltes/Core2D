using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    public static class ProjectExtensions
    {
        public static void AddDocument(this ProjectContainer project, DocumentContainer document)
        {
            if (project?.Documents != null && document != null)
            {
                var previous = project.Documents;
                var next = project.Documents.Add(document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        public static void AddDocumentAt(this ProjectContainer project, DocumentContainer document, int index)
        {
            if (project?.Documents != null && document != null && index >= 0)
            {
                var previous = project.Documents;
                var next = project.Documents.Insert(index, document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        public static void RemoveDocument(this ProjectContainer project, DocumentContainer document)
        {
            if (project?.Documents != null && document != null)
            {
                var previous = project.Documents;
                var next = project.Documents.Remove(document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        public static void ReplaceDocument(this ProjectContainer project, DocumentContainer document, int index)
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

        public static void AddPage(this ProjectContainer project, DocumentContainer document, PageContainer page)
        {
            if (document != null && page != null)
            {
                var previous = document.Pages;
                var next = document.Pages.Add(page);
                project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        public static void AddPageAt(this ProjectContainer project, DocumentContainer document, PageContainer page, int index)
        {
            if (document != null && page != null && index >= 0)
            {
                var previous = document.Pages;
                var next = document.Pages.Insert(index, page);
                project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        public static DocumentContainer RemovePage(this ProjectContainer project, PageContainer page)
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

        public static void ReplacePage(this ProjectContainer project, DocumentContainer document, PageContainer page, int index)
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

        public static void AddTemplate(this ProjectContainer project, PageContainer template)
        {
            if (project?.Templates != null && template != null)
            {
                var previous = project.Templates;
                var next = project.Templates.Add(template);
                project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
        }

        public static void AddTemplates(this ProjectContainer project, IEnumerable<PageContainer> templates)
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

        public static void RemoveTemplate(this ProjectContainer project, PageContainer template)
        {
            if (project?.Templates != null && template != null)
            {
                var previous = project.Templates;
                var next = project.Templates.Remove(template);
                project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
        }

        public static void ApplyTemplate(this ProjectContainer project, PageContainer page, PageContainer template)
        {
            if (page != null && template != null)
            {
                var previous = page.Template;
                var next = template;
                project?.History?.Snapshot(previous, next, (p) => page.Template = p);
                page.Template = next;
            }
        }

        public static void AddScript(this ProjectContainer project, Script script)
        {
            if (project?.Scripts != null && script != null)
            {
                var previous = project.Scripts;
                var next = project.Scripts.Add(script);
                project?.History?.Snapshot(previous, next, (p) => project.Scripts = p);
                project.Scripts = next;
            }
        }

        public static void AddScripts(this ProjectContainer project, IEnumerable<Script> scripts)
        {
            if (project?.Scripts != null && scripts != null)
            {
                var builder = project.Scripts.ToBuilder();
                builder.AddRange(scripts);

                var previous = project.Scripts;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => project.Scripts = p);
                project.Scripts = next;
            }
        }

        public static void RemoveScript(this ProjectContainer project, Script script)
        {
            if (project?.Scripts != null && script != null)
            {
                var previous = project.Scripts;
                var next = project.Scripts.Remove(script);
                project?.History?.Snapshot(previous, next, (p) => project.Scripts = p);
                project.Scripts = next;
            }
        }

        public static void AddLayer(this ProjectContainer project, PageContainer container, LayerContainer layer)
        {
            if (container != null && container.Layers != null && layer != null)
            {
                var previous = container.Layers;
                var next = container.Layers.Add(layer);
                project?.History?.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
        }

        public static void RemoveLayer(this ProjectContainer project, LayerContainer layer)
        {
            if (layer.Owner is PageContainer container && container.Layers != null)
            {
                var previous = container.Layers;
                var next = container.Layers.Remove(layer);
                project?.History?.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
        }

        public static void ClearLayer(this ProjectContainer project, LayerContainer layer)
        {
            if (layer != null)
            {
                var previous = layer.Shapes;
                var next = ImmutableArray.Create<BaseShape>();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void AddShape(this ProjectContainer project, LayerContainer layer, BaseShape shape)
        {
            if (layer != null && layer.Shapes != null && shape != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Add(shape);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void AddShapeAt(this ProjectContainer project, LayerContainer layer, BaseShape shape, int index)
        {
            if (layer?.Shapes != null && shape != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Insert(index, shape);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void AddShapes(this ProjectContainer project, LayerContainer layer, IEnumerable<BaseShape> shapes)
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

        public static void RemoveShape(this ProjectContainer project, LayerContainer layer, BaseShape shape)
        {
            if (layer?.Shapes != null && shape != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(shape);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static LayerContainer RemoveShape(this ProjectContainer project, BaseShape shape)
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

        public static void ReplaceShape(this ProjectContainer project, LayerContainer layer, BaseShape shape, int index)
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

        public static void SwapShape(this ProjectContainer project, LayerContainer layer, BaseShape shape, int insertIndex, int removeIndex)
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

        public static void AddProperty(this ProjectContainer project, IDataObject data, Property property)
        {
            if (data?.Properties != null && property != null)
            {
                var previous = data.Properties;
                var next = data.Properties.Add(property);
                project?.History?.Snapshot(previous, next, (p) => data.Properties = p);
                data.Properties = next;
            }
        }

        public static void RemoveProperty(this ProjectContainer project, Property property)
        {
            if (property.Owner is IDataObject data && data.Properties != null)
            {
                var previous = data.Properties;
                var next = data.Properties.Remove(property);
                project?.History?.Snapshot(previous, next, (p) => data.Properties = p);
                data.Properties = next;
            }
        }

        public static void AddDatabase(this ProjectContainer project, Database db)
        {
            if (project?.Databases != null && db != null)
            {
                var previous = project.Databases;
                var next = project.Databases.Add(db);
                project?.History?.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;
            }
        }

        public static void RemoveDatabase(this ProjectContainer project, Database db)
        {
            if (project?.Databases != null && db != null)
            {
                var previous = project.Databases;
                var next = project.Databases.Remove(db);
                project?.History?.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;
            }
        }

        public static void UpdateDatabase(this ProjectContainer project, Database destination, Database source)
        {
            if (destination != null && source != null)
            {
                bool isDirty = destination.Update(source, out var records);

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

        public static void AddColumn(this ProjectContainer project, Database db, Column column)
        {
            if (db?.Columns != null && column != null)
            {
                var previous = db.Columns;
                var next = db.Columns.Add(column);
                project?.History?.Snapshot(previous, next, (p) => db.Columns = p);
                db.Columns = next;
            }
        }

        public static void RemoveColumn(this ProjectContainer project, Column column)
        {
            if (column?.Owner is Database db && db.Columns != null)
            {
                var previous = db.Columns;
                var next = db.Columns.Remove(column);
                project?.History?.Snapshot(previous, next, (p) => db.Columns = p);
                db.Columns = next;
            }
        }

        public static void AddRecord(this ProjectContainer project, Database db, Record record)
        {
            if (db?.Records != null)
            {
                var previous = db.Records;
                var next = db.Records.Add(record);
                project?.History?.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
        }

        public static void RemoveRecord(this ProjectContainer project, Record record)
        {
            if (record?.Owner is Database db && db.Records != null)
            {
                var previous = db.Records;
                var next = db.Records.Remove(record);
                project?.History?.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
        }

        public static void ResetRecord(this ProjectContainer project, IDataObject data)
        {
            var record = data?.Record;
            if (record != null)
            {
                var previous = record;
                var next = default(Record);
                project?.History?.Snapshot(previous, next, (p) => data.Record = p);
                data.Record = next;
            }
        }

        public static void ApplyRecord(this ProjectContainer project, IDataObject data, Record record)
        {
            if (data != null && record != null)
            {
                var previous = data.Record;
                var next = record;
                project?.History?.Snapshot(previous, next, (p) => data.Record = p);
                data.Record = next;
            }
        }

        public static void AddGroupLibrary(this ProjectContainer project, Library<GroupShape> library)
        {
            if (project?.GroupLibraries != null && library != null)
            {
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Add(library);
                project?.History?.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        public static void AddGroupLibraries(this ProjectContainer project, IEnumerable<Library<GroupShape>> libraries)
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

        public static void RemoveGroupLibrary(this ProjectContainer project, Library<GroupShape> library)
        {
            if (project?.GroupLibraries != null && library != null)
            {
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Remove(library);
                project?.History?.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        public static void AddStyleLibrary(this ProjectContainer project, Library<ShapeStyle> library)
        {
            if (project?.StyleLibraries != null && library != null)
            {
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Add(library);
                project?.History?.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        public static void AddStyleLibraries(this ProjectContainer project, IEnumerable<Library<ShapeStyle>> libraries)
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

        public static void RemoveStyleLibrary(this ProjectContainer project, Library<ShapeStyle> library)
        {
            if (project?.CurrentStyleLibrary != null && library != null)
            {
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Remove(library);
                project?.History?.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        public static void AddStyle(this ProjectContainer project, Library<ShapeStyle> library, ShapeStyle style)
        {
            AddItem(project, library, style);
        }

        public static Library<ShapeStyle> RemoveStyle(this ProjectContainer project, ShapeStyle style)
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

        public static void ApplyStyle(this ProjectContainer project, BaseShape shape, ShapeStyle style)
        {
            if (shape != null && style != null)
            {
                if (shape is GroupShape group)
                {
                    var shapes = ProjectContainer.GetAllShapes(group.Shapes);
                    foreach (var child in shapes)
                    {
                        var previous = child.Style;
                        var next = (ShapeStyle)style.Copy(null);
                        project?.History?.Snapshot(previous, next, (p) => child.Style = p);
                        child.Style = next;
                    }
                }
                else
                {
                    var previous = shape.Style;
                    var next = (ShapeStyle)style.Copy(null);
                    project?.History?.Snapshot(previous, next, (p) => shape.Style = p);
                    shape.Style = next;
                }
            }
        }

        public static void AddGroup(this ProjectContainer project, Library<GroupShape> library, GroupShape group)
        {
            AddItem(project, library, group);
        }

        public static Library<GroupShape> RemoveGroup(this ProjectContainer project, GroupShape group)
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

        public static void AddItem<T>(this ProjectContainer project, Library<T> library, T item)
        {
            if (library?.Items != null && item != null)
            {
                var previous = library.Items;
                var next = library.Items.Add(item);
                project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                library.Items = next;
            }
        }

        public static void AddItems<T>(this ProjectContainer project, Library<T> library, IEnumerable<T> items)
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
