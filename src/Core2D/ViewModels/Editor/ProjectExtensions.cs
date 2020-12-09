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
        public static void AddDocument(this ProjectContainerViewModel project, DocumentContainerViewModel document)
        {
            if (project?.Documents != null && document != null)
            {
                var previous = project.Documents;
                var next = project.Documents.Add(document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        public static void AddDocumentAt(this ProjectContainerViewModel project, DocumentContainerViewModel document, int index)
        {
            if (project?.Documents != null && document != null && index >= 0)
            {
                var previous = project.Documents;
                var next = project.Documents.Insert(index, document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        public static void RemoveDocument(this ProjectContainerViewModel project, DocumentContainerViewModel document)
        {
            if (project?.Documents != null && document != null)
            {
                var previous = project.Documents;
                var next = project.Documents.Remove(document);
                project?.History?.Snapshot(previous, next, (p) => project.Documents = p);
                project.Documents = next;
            }
        }

        public static void ReplaceDocument(this ProjectContainerViewModel project, DocumentContainerViewModel document, int index)
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

        public static void AddPage(this ProjectContainerViewModel project, DocumentContainerViewModel document, PageContainerViewModel page)
        {
            if (document != null && page != null)
            {
                var previous = document.Pages;
                var next = document.Pages.Add(page);
                project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        public static void AddPageAt(this ProjectContainerViewModel project, DocumentContainerViewModel document, PageContainerViewModel page, int index)
        {
            if (document != null && page != null && index >= 0)
            {
                var previous = document.Pages;
                var next = document.Pages.Insert(index, page);
                project?.History?.Snapshot(previous, next, (p) => document.Pages = p);
                document.Pages = next;
            }
        }

        public static DocumentContainerViewModel RemovePage(this ProjectContainerViewModel project, PageContainerViewModel page)
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

        public static void ReplacePage(this ProjectContainerViewModel project, DocumentContainerViewModel document, PageContainerViewModel page, int index)
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

        public static void AddTemplate(this ProjectContainerViewModel project, PageContainerViewModel template)
        {
            if (project?.Templates != null && template != null)
            {
                var previous = project.Templates;
                var next = project.Templates.Add(template);
                project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
        }

        public static void AddTemplates(this ProjectContainerViewModel project, IEnumerable<PageContainerViewModel> templates)
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

        public static void RemoveTemplate(this ProjectContainerViewModel project, PageContainerViewModel template)
        {
            if (project?.Templates != null && template != null)
            {
                var previous = project.Templates;
                var next = project.Templates.Remove(template);
                project?.History?.Snapshot(previous, next, (p) => project.Templates = p);
                project.Templates = next;
            }
        }

        public static void ApplyTemplate(this ProjectContainerViewModel project, PageContainerViewModel page, PageContainerViewModel template)
        {
            if (page != null && template != null)
            {
                var previous = page.Template;
                var next = template;
                project?.History?.Snapshot(previous, next, (p) => page.Template = p);
                page.Template = next;
            }
        }

        public static void AddScript(this ProjectContainerViewModel project, ScriptViewModel scriptViewModel)
        {
            if (project?.Scripts != null && scriptViewModel != null)
            {
                var previous = project.Scripts;
                var next = project.Scripts.Add(scriptViewModel);
                project?.History?.Snapshot(previous, next, (p) => project.Scripts = p);
                project.Scripts = next;
            }
        }

        public static void AddScripts(this ProjectContainerViewModel project, IEnumerable<ScriptViewModel> scripts)
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

        public static void RemoveScript(this ProjectContainerViewModel project, ScriptViewModel scriptViewModel)
        {
            if (project?.Scripts != null && scriptViewModel != null)
            {
                var previous = project.Scripts;
                var next = project.Scripts.Remove(scriptViewModel);
                project?.History?.Snapshot(previous, next, (p) => project.Scripts = p);
                project.Scripts = next;
            }
        }

        public static void AddLayer(this ProjectContainerViewModel project, PageContainerViewModel containerViewModel, LayerContainerViewModel layer)
        {
            if (containerViewModel != null && containerViewModel.Layers != null && layer != null)
            {
                var previous = containerViewModel.Layers;
                var next = containerViewModel.Layers.Add(layer);
                project?.History?.Snapshot(previous, next, (p) => containerViewModel.Layers = p);
                containerViewModel.Layers = next;
            }
        }

        public static void RemoveLayer(this ProjectContainerViewModel project, LayerContainerViewModel layer)
        {
            if (layer.Owner is PageContainerViewModel container && container.Layers != null)
            {
                var previous = container.Layers;
                var next = container.Layers.Remove(layer);
                project?.History?.Snapshot(previous, next, (p) => container.Layers = p);
                container.Layers = next;
            }
        }

        public static void ClearLayer(this ProjectContainerViewModel project, LayerContainerViewModel layer)
        {
            if (layer != null)
            {
                var previous = layer.Shapes;
                var next = ImmutableArray.Create<BaseShapeViewModel>();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void AddShape(this ProjectContainerViewModel project, LayerContainerViewModel layer, BaseShapeViewModel shapeViewModel)
        {
            if (layer != null && layer.Shapes != null && shapeViewModel != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Add(shapeViewModel);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void AddShapeAt(this ProjectContainerViewModel project, LayerContainerViewModel layer, BaseShapeViewModel shapeViewModel, int index)
        {
            if (layer?.Shapes != null && shapeViewModel != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Insert(index, shapeViewModel);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void AddShapes(this ProjectContainerViewModel project, LayerContainerViewModel layer, IEnumerable<BaseShapeViewModel> shapes)
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

        public static void RemoveShape(this ProjectContainerViewModel project, LayerContainerViewModel layer, BaseShapeViewModel shapeViewModel)
        {
            if (layer?.Shapes != null && shapeViewModel != null)
            {
                var previous = layer.Shapes;
                var next = layer.Shapes.Remove(shapeViewModel);
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static LayerContainerViewModel RemoveShape(this ProjectContainerViewModel project, BaseShapeViewModel shapeViewModel)
        {
            if (project?.Documents != null && shapeViewModel != null)
            {
                var layer = project.Documents.SelectMany(d => d.Pages).SelectMany(p => p.Layers).FirstOrDefault(l => l.Shapes.Contains(shapeViewModel));
                if (layer != null)
                {
                    project.RemoveShape(layer, shapeViewModel);
                }
                return layer;
            }
            return null;
        }

        public static void ReplaceShape(this ProjectContainerViewModel project, LayerContainerViewModel layer, BaseShapeViewModel shapeViewModel, int index)
        {
            if (layer != null && shapeViewModel != null && index >= 0)
            {
                var builder = layer.Shapes.ToBuilder();
                builder[index] = shapeViewModel;

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void SwapShape(this ProjectContainerViewModel project, LayerContainerViewModel layer, BaseShapeViewModel shapeViewModel, int insertIndex, int removeIndex)
        {
            if (layer != null && shapeViewModel != null && insertIndex >= 0 && removeIndex >= 0)
            {
                var builder = layer.Shapes.ToBuilder();
                builder.Insert(insertIndex, shapeViewModel);
                builder.RemoveAt(removeIndex);

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;
            }
        }

        public static void AddProperty(this ProjectContainerViewModel project, IDataObject data, PropertyViewModel propertyViewModel)
        {
            if (data?.Properties != null && propertyViewModel != null)
            {
                var previous = data.Properties;
                var next = data.Properties.Add(propertyViewModel);
                project?.History?.Snapshot(previous, next, (p) => data.Properties = p);
                data.Properties = next;
            }
        }

        public static void RemoveProperty(this ProjectContainerViewModel project, PropertyViewModel propertyViewModel)
        {
            if (propertyViewModel.Owner is IDataObject data && data.Properties != null)
            {
                var previous = data.Properties;
                var next = data.Properties.Remove(propertyViewModel);
                project?.History?.Snapshot(previous, next, (p) => data.Properties = p);
                data.Properties = next;
            }
        }

        public static void AddDatabase(this ProjectContainerViewModel project, DatabaseViewModel db)
        {
            if (project?.Databases != null && db != null)
            {
                var previous = project.Databases;
                var next = project.Databases.Add(db);
                project?.History?.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;
            }
        }

        public static void RemoveDatabase(this ProjectContainerViewModel project, DatabaseViewModel db)
        {
            if (project?.Databases != null && db != null)
            {
                var previous = project.Databases;
                var next = project.Databases.Remove(db);
                project?.History?.Snapshot(previous, next, (p) => project.Databases = p);
                project.Databases = next;
            }
        }

        public static void UpdateDatabase(this ProjectContainerViewModel project, DatabaseViewModel destination, DatabaseViewModel source)
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

        public static void AddColumn(this ProjectContainerViewModel project, DatabaseViewModel db, ColumnViewModel columnViewModel)
        {
            if (db?.Columns != null && columnViewModel != null)
            {
                var previous = db.Columns;
                var next = db.Columns.Add(columnViewModel);
                project?.History?.Snapshot(previous, next, (p) => db.Columns = p);
                db.Columns = next;
            }
        }

        public static void RemoveColumn(this ProjectContainerViewModel project, ColumnViewModel columnViewModel)
        {
            if (columnViewModel?.Owner is DatabaseViewModel db && db.Columns != null)
            {
                var previous = db.Columns;
                var next = db.Columns.Remove(columnViewModel);
                project?.History?.Snapshot(previous, next, (p) => db.Columns = p);
                db.Columns = next;
            }
        }

        public static void AddRecord(this ProjectContainerViewModel project, DatabaseViewModel db, RecordViewModel recordViewModel)
        {
            if (db?.Records != null)
            {
                var previous = db.Records;
                var next = db.Records.Add(recordViewModel);
                project?.History?.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
        }

        public static void RemoveRecord(this ProjectContainerViewModel project, RecordViewModel recordViewModel)
        {
            if (recordViewModel?.Owner is DatabaseViewModel db && db.Records != null)
            {
                var previous = db.Records;
                var next = db.Records.Remove(recordViewModel);
                project?.History?.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
        }

        public static void ResetRecord(this ProjectContainerViewModel project, IDataObject data)
        {
            var record = data?.RecordViewModel;
            if (record != null)
            {
                var previous = record;
                var next = default(RecordViewModel);
                project?.History?.Snapshot(previous, next, (p) => data.RecordViewModel = p);
                data.RecordViewModel = next;
            }
        }

        public static void ApplyRecord(this ProjectContainerViewModel project, IDataObject data, RecordViewModel recordViewModel)
        {
            if (data != null && recordViewModel != null)
            {
                var previous = data.RecordViewModel;
                var next = recordViewModel;
                project?.History?.Snapshot(previous, next, (p) => data.RecordViewModel = p);
                data.RecordViewModel = next;
            }
        }

        public static void AddGroupLibrary(this ProjectContainerViewModel project, LibraryViewModel<GroupShapeViewModel> libraryViewModel)
        {
            if (project?.GroupLibraries != null && libraryViewModel != null)
            {
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Add(libraryViewModel);
                project?.History?.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        public static void AddGroupLibraries(this ProjectContainerViewModel project, IEnumerable<LibraryViewModel<GroupShapeViewModel>> libraries)
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

        public static void RemoveGroupLibrary(this ProjectContainerViewModel project, LibraryViewModel<GroupShapeViewModel> libraryViewModel)
        {
            if (project?.GroupLibraries != null && libraryViewModel != null)
            {
                var previous = project.GroupLibraries;
                var next = project.GroupLibraries.Remove(libraryViewModel);
                project?.History?.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                project.GroupLibraries = next;
            }
        }

        public static void AddStyleLibrary(this ProjectContainerViewModel project, LibraryViewModel<ShapeStyleViewModel> libraryViewModel)
        {
            if (project?.StyleLibraries != null && libraryViewModel != null)
            {
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Add(libraryViewModel);
                project?.History?.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        public static void AddStyleLibraries(this ProjectContainerViewModel project, IEnumerable<LibraryViewModel<ShapeStyleViewModel>> libraries)
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

        public static void RemoveStyleLibrary(this ProjectContainerViewModel project, LibraryViewModel<ShapeStyleViewModel> libraryViewModel)
        {
            if (project?.CurrentStyleLibrary != null && libraryViewModel != null)
            {
                var previous = project.StyleLibraries;
                var next = project.StyleLibraries.Remove(libraryViewModel);
                project?.History?.Snapshot(previous, next, (p) => project.StyleLibraries = p);
                project.StyleLibraries = next;
            }
        }

        public static void AddStyle(this ProjectContainerViewModel project, LibraryViewModel<ShapeStyleViewModel> libraryViewModel, ShapeStyleViewModel styleViewModel)
        {
            AddItem(project, libraryViewModel, styleViewModel);
        }

        public static LibraryViewModel<ShapeStyleViewModel> RemoveStyle(this ProjectContainerViewModel project, ShapeStyleViewModel styleViewModel)
        {
            if (project?.StyleLibraries != null && styleViewModel != null)
            {
                var library = project.StyleLibraries.FirstOrDefault(l => l.Items.Contains(styleViewModel));
                if (library?.Items != null)
                {
                    var previous = library.Items;
                    var next = library.Items.Remove(styleViewModel);
                    project?.History?.Snapshot(previous, next, (p) => library.Items = p);
                    library.Items = next;
                }
                return library;
            }
            return null;
        }

        public static void ApplyStyle(this ProjectContainerViewModel project, BaseShapeViewModel shapeViewModel, ShapeStyleViewModel styleViewModel)
        {
            if (shapeViewModel != null && styleViewModel != null)
            {
                if (shapeViewModel is GroupShapeViewModel group)
                {
                    var shapes = ProjectContainerViewModel.GetAllShapes(group.Shapes);
                    foreach (var child in shapes)
                    {
                        var previous = child.StyleViewModel;
                        var next = (ShapeStyleViewModel)styleViewModel.Copy(null);
                        project?.History?.Snapshot(previous, next, (p) => child.StyleViewModel = p);
                        child.StyleViewModel = next;
                    }
                }
                else
                {
                    var previous = shapeViewModel.StyleViewModel;
                    var next = (ShapeStyleViewModel)styleViewModel.Copy(null);
                    project?.History?.Snapshot(previous, next, (p) => shapeViewModel.StyleViewModel = p);
                    shapeViewModel.StyleViewModel = next;
                }
            }
        }

        public static void AddGroup(this ProjectContainerViewModel project, LibraryViewModel<GroupShapeViewModel> libraryViewModel, GroupShapeViewModel group)
        {
            AddItem(project, libraryViewModel, group);
        }

        public static LibraryViewModel<GroupShapeViewModel> RemoveGroup(this ProjectContainerViewModel project, GroupShapeViewModel group)
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

        public static void AddItem<T>(this ProjectContainerViewModel project, LibraryViewModel<T> libraryViewModel, T item)
        {
            if (libraryViewModel?.Items != null && item != null)
            {
                var previous = libraryViewModel.Items;
                var next = libraryViewModel.Items.Add(item);
                project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
                libraryViewModel.Items = next;
            }
        }

        public static void AddItems<T>(this ProjectContainerViewModel project, LibraryViewModel<T> libraryViewModel, IEnumerable<T> items)
        {
            if (libraryViewModel?.Items != null && items != null)
            {
                var builder = libraryViewModel.Items.ToBuilder();
                builder.AddRange(items);

                var previous = libraryViewModel.Items;
                var next = builder.ToImmutable();
                project?.History?.Snapshot(previous, next, (p) => libraryViewModel.Items = p);
                libraryViewModel.Items = next;
            }
        }
    }
}
