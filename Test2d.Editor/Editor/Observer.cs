// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define VERBOSE
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Observer
    {
        private readonly Editor _editor;
        private readonly Action _invalidateContainer;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        /// <summary>
        /// 
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public Observer(Editor editor)
        {
            _editor = editor;

            _invalidateContainer = () =>
            {
                if (IsPaused)
                {
                    return;
                }
            };

            _invalidateStyles = () =>
            {
                if (IsPaused)
                {
                    return;
                }

                if (_editor.Project != null && _editor.Project.CurrentContainer != null)
                {
                    foreach (var renderer in _editor.Renderers)
                    {
                        renderer.ClearCache(isZooming: false);
                    }
                    _editor.Project.CurrentContainer.Invalidate();
                }
            };

            _invalidateLayers = () =>
            {
                if (IsPaused)
                {
                    return;
                }

                if (_editor.Project != null && _editor.Project.CurrentContainer != null)
                {
                    _editor.Project.CurrentContainer.Invalidate();
                }
            };

            _invalidateShapes = () =>
            {
                if (IsPaused)
                {
                    return;
                }

                if (_editor.Project != null && _editor.Project.CurrentContainer != null)
                {
                    _editor.Project.CurrentContainer.Invalidate();
                }
            };

            if (_editor.Project != null)
            {
                Add(_editor.Project);
            }
        }

        [Conditional("VERBOSE")]
        private void Verbose(string text)
        {
            Debug.WriteLine(text);
        }

        /// <summary>
        /// 
        /// </summary>
        private void MarkAsDirty()
        {
            if (_editor != null)
            {
                _editor.IsProjectDirty = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabaseObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Database: " + sender.GetType() + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Columns")
            {
                var database = sender as Database;
                Remove(database.Columns);
                Add(database.Columns);
            }

            if (e.PropertyName == "Records")
            {
                var database = sender as Database;
                Remove(database.Records);
                Add(database.Records);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Column: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Record: " + sender.GetType() + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Columns")
            {
                var record = sender as Record;
                Remove(record.Columns);
                Add(record.Columns);
            }

            if (e.PropertyName == "Values")
            {
                var record = sender as Record;
                Remove(record.Values);
                Add(record.Values);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValueObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Value: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Project: " + sender.GetType() + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Databases")
            {
                var project = sender as Project;
                Remove(project.Databases);
                Add(project.Databases);
            }

            if (e.PropertyName == "StyleLibraries")
            {
                var project = sender as Project;
                Remove(project.StyleLibraries);
                Add(project.StyleLibraries);
            }

            if (e.PropertyName == "GroupLibraries")
            {
                var project = sender as Project;
                Remove(project.GroupLibraries);
                Add(project.GroupLibraries);
            }

            if (e.PropertyName == "Templates")
            {
                var project = sender as Project;
                Remove(project.Templates);
                Add(project.Templates);
            }

            if (e.PropertyName == "Documents")
            {
                var project = sender as Project;
                Remove(project.Documents);
                Add(project.Documents);
            }

            _invalidateShapes();

            // NOTE: Do not mark project as dirty when 'Current*' property changes.
            if (!e.PropertyName.StartsWith("Current"))
            {
                MarkAsDirty();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Document: " + sender.GetType() + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Containers")
            {
                var document = sender as Document;
                Remove(document.Containers);
                Add(document.Containers);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContainerObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Container: " + (sender is Container ? (sender as Container).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Properties")
            {
                var container = sender as Container;
                Remove(container.Properties);
                Add(container.Properties);
            }

            if (e.PropertyName == "Layers")
            {
                var container = sender as Container;
                Remove(container.Layers);
                Add(container.Layers);
            }

            _invalidateContainer();

            // NOTE: Do not mark project as dirty when current shape changes.
            // NOTE: Do not mark project as dirty when current layer changes.
            if (e.PropertyName != "CurrentShape"
                && e.PropertyName != "CurrentLayer")
            {
                MarkAsDirty();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContainerBackgroudObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Background: " + sender.GetType().ToString() + ", Property: " + e.PropertyName);

            _editor.Project.CurrentContainer.Notify("Background");
            if (_editor.Project.CurrentContainer.Template != null)
            {
                _editor.Project.CurrentContainer.Template.Notify("Background");
            }
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Layer: " + (sender is Layer ? (sender as Layer).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Shapes")
            {
                var layer = sender as Layer;
                Remove(layer.Shapes);
                Add(layer.Shapes);
            }

            _invalidateLayers();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShapeObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Shape: " + sender.GetType() + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Bindings")
            {
                var shape = sender as BaseShape;
                Remove(shape.Bindings);
                Add(shape.Bindings);
            }

            if (e.PropertyName == "Properties")
            {
                var shape = sender as BaseShape;
                Remove(shape.Properties);
                Add(shape.Properties);
            }

            _invalidateShapes();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StyleLibraryObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Style Library: " + (sender is StyleLibrary ? (sender as StyleLibrary).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Styles")
            {
                var sg = sender as StyleLibrary;
                Remove(sg.Styles);
                Add(sg.Styles);
            }

            _invalidateStyles();

            // NOTE: Do not mark project as dirty when current style changes.
            if (e.PropertyName != "CurrentStyle")
            {
                MarkAsDirty();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupLibraryObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Group Library: " + (sender is GroupLibrary ? (sender as GroupLibrary).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);

            if (e.PropertyName == "Groups")
            {
                var sg = sender as GroupLibrary;
                Remove(sg.Groups);
                Add(sg.Groups);
            }

            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StyleObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Style: " + (sender is ShapeStyle ? (sender as ShapeStyle).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);
            _invalidateStyles();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BindingObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Property: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
            MarkAsDirty();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Verbose("Property: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
            MarkAsDirty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public void Add(Database database)
        {
            if (database == null)
                return;

            database.PropertyChanged += DatabaseObserver;

            Verbose("Add Database: " + database.Name);

            if (database.Columns != null)
            {
                Add(database.Columns);
            }

            if (database.Records != null)
            {
                Add(database.Records);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public void Remove(Database database)
        {
            if (database == null)
                return;

            database.PropertyChanged -= DatabaseObserver;

            Verbose("Remove Database: " + database.Name);

            if (database.Columns != null)
            {
                Remove(database.Columns);
            }

            if (database.Records != null)
            {
                Remove(database.Records);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        public void Add(Column column)
        {
            if (column == null)
                return;

            column.PropertyChanged += ColumnObserver;

            Verbose("Add Column: " + column.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        public void Remove(Column column)
        {
            if (column == null)
                return;

            column.PropertyChanged -= ColumnObserver;

            Verbose("Remove Column: " + column.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        public void Add(Record record)
        {
            if (record == null)
                return;

            record.PropertyChanged += RecordObserver;

            if (record.Values != null)
            {
                Add(record.Values);
            }

            Verbose("Add Record: " + record.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        public void Remove(Record record)
        {
            if (record == null)
                return;

            record.PropertyChanged -= RecordObserver;

            if (record.Values != null)
            {
                Remove(record.Values);
            }

            Verbose("Remove Record: " + record.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Add(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged += ValueObserver;

            Verbose("Add Value");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Remove(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged -= ValueObserver;

            Verbose("Remove Value");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public void Add(Options options)
        {
            if (options == null)
                return;

            if (options.PointShape != null)
            {
                Add(options.PointShape);
            }

            if (options.PointStyle != null)
            {
                Add(options.PointStyle);
            }

            if (options.SelectionStyle != null)
            {
                Add(options.SelectionStyle);
            }

            if (options.HelperStyle != null)
            {
                Add(options.HelperStyle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public void Remove(Options options)
        {
            if (options == null)
                return;

            if (options.PointShape != null)
            {
                Remove(options.PointShape);
            }

            if (options.PointStyle != null)
            {
                Remove(options.PointStyle);
            }

            if (options.SelectionStyle != null)
            {
                Remove(options.SelectionStyle);
            }

            if (options.HelperStyle != null)
            {
                Remove(options.HelperStyle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public void Add(Project project)
        {
            if (project == null)
                return;

            project.PropertyChanged += ProjectObserver;
            
            Verbose("Add Project: " + project.Name);

            Add(project.Options);

            if (project.Databases != null)
            {
                foreach (var database in project.Databases)
                {
                    Add(database);
                }
            }
            
            if (project.Documents != null)
            {
                foreach (var document in project.Documents)
                {
                    Add(document);
                }
            }
            
            if (project.Templates != null)
            {
                foreach (var template in project.Templates)
                {
                    Add(template);
                }
            }
            
            if (project.StyleLibraries != null)
            {
                foreach (var sg in project.StyleLibraries)
                {
                    Add(sg);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public void Remove(Project project)
        {
            if (project == null)
                return;

            project.PropertyChanged -= ProjectObserver;

            Verbose("Remove Project: " + project.Name);

            Remove(project.Options);

            if (project.Databases != null)
            {
                foreach (var database in project.Databases)
                {
                    Remove(database);
                }
            }
            
            if (project.Documents != null)
            {
                foreach (var document in project.Documents)
                {
                    Remove(document);
                }
            }

            if (project.Templates != null)
            {
                foreach (var template in project.Templates)
                {
                    Remove(template);
                }
            }

            if (project.StyleLibraries != null)
            {
                foreach (var sg in project.StyleLibraries)
                {
                    Remove(sg);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        public void Add(Document document)
        {
            if (document == null)
                return;

            document.PropertyChanged += DocumentObserver;

            Verbose("Add Document: " + document.Name);

            if (document.Containers != null)
            {
                foreach (var container in document.Containers)
                {
                    Add(container);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        public void Remove(Document document)
        {
            if (document == null)
                return;

            document.PropertyChanged -= DocumentObserver;

            Verbose("Remove Document: " + document.Name);

            if (document.Containers != null)
            {
                foreach (var container in document.Containers)
                {
                    Remove(container);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Add(Container container)
        {
            if (container == null)
                return;
            
            container.PropertyChanged += ContainerObserver;
            
            if (container.Background != null)
            {
                container.Background.PropertyChanged += ContainerBackgroudObserver;
            }
            
            Verbose("Add Container: " + container.Name);
            
            if (container.Layers != null)
            {
                Add(container.Layers);
            }
            
            if (container.Properties != null)
            {
                Add(container.Properties);
            }
            
            //Add(container.WorkingLayer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void Remove(Container container)
        {
            if (container == null)
                return;
            
            container.PropertyChanged -= ContainerObserver;
            
            if (container.Background != null)
            {
                container.Background.PropertyChanged -= ContainerBackgroudObserver;
            }
            
            Verbose("Remove Container: " + container.Name);
            
            if (container.Layers != null)
            {
                Add(container.Layers);
            }
            
            if (container.Properties != null)
            {
                Remove(container.Properties);
            }
            
            //Remove(container.WorkingLayer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        public void Add(Layer layer)
        {
            if (layer == null)
                return;
            
            layer.PropertyChanged += LayerObserver;
            
            Verbose("Add Layer: " + layer.Name);
            
            if (layer.Shapes != null)
            {
                Add(layer.Shapes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        public void Remove(Layer layer)
        {
            if (layer == null)
                return;
            
            layer.PropertyChanged -= LayerObserver;
            
            Verbose("Remove Layer: " + layer.Name);
            
            if (layer.Shapes != null)
            {
                Remove(layer.Shapes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public void Add(BaseShape shape)
        {
            if (shape == null)
                return;
            
            shape.PropertyChanged += ShapeObserver;

            if (shape.Bindings != null)
            {
                Add(shape.Bindings);
            }
            
            if (shape.Properties != null)
            {
                Add(shape.Properties);
            }
            
            if (shape is XPoint)
            {
                var point = shape as XPoint;
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;
                
                if (line.Start != null)
                {
                    line.Start.PropertyChanged += ShapeObserver;
                }
                
                if (line.End != null)
                {
                    line.End.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XRectangle)
            {
                var rectangle = shape as XRectangle;
                
                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged += ShapeObserver;
                }
                
                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XEllipse)
            {
                var ellipse = shape as XEllipse;

                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged += ShapeObserver;
                }
                
                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XArc)
            {
                var arc = shape as XArc;
                
                if (arc.Point1 != null)
                {
                    arc.Point1.PropertyChanged += ShapeObserver;
                }
                
                if (arc.Point2 != null)
                {
                    arc.Point2.PropertyChanged += ShapeObserver;
                }
                
                if (arc.Point3 != null)
                {
                    arc.Point3.PropertyChanged += ShapeObserver;
                }
                
                if (arc.Point4 != null)
                {
                    arc.Point4.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XBezier)
            {
                var bezier = shape as XBezier;

                if (bezier.Point1 != null)
                {
                    bezier.Point1.PropertyChanged += ShapeObserver;
                }
                
                if (bezier.Point2 != null)
                {
                    bezier.Point2.PropertyChanged += ShapeObserver;
                }
                
                if (bezier.Point3 != null)
                {
                    bezier.Point3.PropertyChanged += ShapeObserver;
                }
                
                if (bezier.Point4 != null)
                {
                    bezier.Point4.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XQBezier)
            {
                var qbezier = shape as XQBezier;

                if (qbezier.Point1 != null)
                {
                    qbezier.Point1.PropertyChanged += ShapeObserver;
                }
                
                if (qbezier.Point2 != null)
                {
                    qbezier.Point2.PropertyChanged += ShapeObserver;
                }
                
                if (qbezier.Point3 != null)
                {
                    qbezier.Point3.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XText)
            {
                var text = shape as XText;

                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged += ShapeObserver;
                }
                
                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XImage)
            {
                var image = shape as XImage;
 
                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged += ShapeObserver;
                }
                
                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged += ShapeObserver;
                }
            }
            else if (shape is XPath)
            {
                // TODO: Observer path sub properties.
            }
            else if (shape is XGroup)
            {
                var group = shape as XGroup;
                
                if (group != null)
                {
                    if (group.Shapes != null)
                    {
                        Add(group.Shapes);
                    }
                    
                    if (group.Connectors != null)
                    {
                        Add(group.Connectors);
                    }
                }
            }

            Verbose("Add Shape: " + shape.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public void Remove(BaseShape shape)
        {
            if (shape == null)
                return;
            
            shape.PropertyChanged -= ShapeObserver;

            if (shape.Bindings != null)
            {
                Remove(shape.Bindings);
            }
            
            if (shape.Properties != null)
            {
                Remove(shape.Properties);
            }
            
            if (shape is XPoint)
            {
                var point = shape as XPoint;
                if (point.Shape != null)
                {
                    point.Shape.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;
                
                if (line.Start != null)
                {
                    line.Start.PropertyChanged -= ShapeObserver;
                }
                
                if (line.End != null)
                {
                    line.End.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XRectangle)
            {
                var rectangle = shape as XRectangle;
                
                if (rectangle.TopLeft != null)
                {
                    rectangle.TopLeft.PropertyChanged -= ShapeObserver;
                }
                
                if (rectangle.BottomRight != null)
                {
                    rectangle.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XEllipse)
            {
                var ellipse = shape as XEllipse;

                if (ellipse.TopLeft != null)
                {
                    ellipse.TopLeft.PropertyChanged -= ShapeObserver;
                }
                
                if (ellipse.BottomRight != null)
                {
                    ellipse.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XArc)
            {
                var arc = shape as XArc;
                
                if (arc.Point1 != null)
                {
                    arc.Point1.PropertyChanged -= ShapeObserver;
                }
                
                if (arc.Point2 != null)
                {
                    arc.Point2.PropertyChanged -= ShapeObserver;
                }
                
                if (arc.Point3 != null)
                {
                    arc.Point3.PropertyChanged -= ShapeObserver;
                }
                
                if (arc.Point4 != null)
                {
                    arc.Point4.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XBezier)
            {
                var bezier = shape as XBezier;

                if (bezier.Point1 != null)
                {
                    bezier.Point1.PropertyChanged -= ShapeObserver;
                }
                
                if (bezier.Point2 != null)
                {
                    bezier.Point2.PropertyChanged -= ShapeObserver;
                }
                
                if (bezier.Point3 != null)
                {
                    bezier.Point3.PropertyChanged -= ShapeObserver;
                }
                
                if (bezier.Point4 != null)
                {
                    bezier.Point4.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XQBezier)
            {
                var qbezier = shape as XQBezier;

                if (qbezier.Point1 != null)
                {
                    qbezier.Point1.PropertyChanged -= ShapeObserver;
                }
                
                if (qbezier.Point2 != null)
                {
                    qbezier.Point2.PropertyChanged -= ShapeObserver;
                }
                
                if (qbezier.Point3 != null)
                {
                    qbezier.Point3.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XText)
            {
                var text = shape as XText;

                if (text.TopLeft != null)
                {
                    text.TopLeft.PropertyChanged -= ShapeObserver;
                }
                
                if (text.BottomRight != null)
                {
                    text.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XImage)
            {
                var image = shape as XImage;
 
                if (image.TopLeft != null)
                {
                    image.TopLeft.PropertyChanged -= ShapeObserver;
                }
                
                if (image.BottomRight != null)
                {
                    image.BottomRight.PropertyChanged -= ShapeObserver;
                }
            }
            else if (shape is XPath)
            {
                // TODO: Stop observing path sub properties;
            }
            else if (shape is XGroup)
            {
                var group = shape as XGroup;
                
                if (group != null)
                {
                    if (group.Shapes != null)
                    {
                        Remove(group.Shapes);
                    }
                    
                    if (group.Connectors != null)
                    {
                        Remove(group.Connectors);
                    }
                }
            }

            Verbose("Remove Shape: " + shape.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        public void Add(StyleLibrary sg)
        {
            if (sg == null)
                return;
            
            if (sg.Styles != null)
            {
                Add(sg.Styles);
            }
            
            sg.PropertyChanged += StyleLibraryObserver;
            Verbose("Add Style Library: " + sg.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        public void Remove(StyleLibrary sg)
        {
            if (sg == null)
                return;
            
            if (sg.Styles != null)
            {
                Remove(sg.Styles);
            }
            
            sg.PropertyChanged -= StyleLibraryObserver;
            Verbose("Remove Style Library: " + sg.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gl"></param>
        public void Add(GroupLibrary gl)
        {
            if (gl == null)
                return;

            if (gl.Groups != null)
            {
                Add(gl.Groups);
            }

            gl.PropertyChanged += GroupLibraryObserver;
            Verbose("Add Group Library: " + gl.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gl"></param>
        public void Remove(GroupLibrary gl)
        {
            if (gl == null)
                return;

            if (gl.Groups != null)
            {
                Remove(gl.Groups);
            }

            gl.PropertyChanged -= GroupLibraryObserver;
            Verbose("Remove Group Library: " + gl.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        public void Add(ShapeStyle style)
        {
            if (style == null)
                return;
            
            style.PropertyChanged += StyleObserver;
            
            if (style.Stroke != null)
            {
                style.Stroke.PropertyChanged += StyleObserver;
            }
            
            if (style.Fill != null)
            {
                style.Fill.PropertyChanged += StyleObserver;
            }
            
            if (style.LineStyle != null)
            {
                style.LineStyle.PropertyChanged += StyleObserver;
            }
            
            if (style.StartArrowStyle != null)
            {
                style.StartArrowStyle.PropertyChanged += StyleObserver;
                
                if (style.StartArrowStyle.Stroke != null)
                {
                    style.StartArrowStyle.Stroke.PropertyChanged += StyleObserver;
                }
                
                if (style.StartArrowStyle.Fill != null)
                {
                    style.StartArrowStyle.Fill.PropertyChanged += StyleObserver;
                }
            }
        
            if (style.EndArrowStyle != null)
            {
                style.EndArrowStyle.PropertyChanged += StyleObserver;
                
                if (style.EndArrowStyle.Stroke != null)
                {
                    style.EndArrowStyle.Stroke.PropertyChanged += StyleObserver;
                }
                
                if (style.EndArrowStyle.Fill != null)
                {
                    style.EndArrowStyle.Fill.PropertyChanged += StyleObserver;
                }
            }
        
            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged += StyleObserver;
            }
            
            Verbose("Add Style: " + style.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        public void Remove(ShapeStyle style)
        {
            if (style == null)
                return;
            
            style.PropertyChanged -= StyleObserver;
            
            if (style.Stroke != null)
            {
                style.Stroke.PropertyChanged -= StyleObserver;
            }
            
            if (style.Fill != null)
            {
                style.Fill.PropertyChanged -= StyleObserver;
            }
            
            if (style.LineStyle != null)
            {
                style.LineStyle.PropertyChanged -= StyleObserver;
            }
        
            if (style.StartArrowStyle != null)
            {
                style.StartArrowStyle.PropertyChanged -= StyleObserver;
                
                if (style.StartArrowStyle.Stroke != null)
                {
                    style.StartArrowStyle.Stroke.PropertyChanged -= StyleObserver;
                }
                
                if (style.StartArrowStyle.Fill != null)
                {
                    style.StartArrowStyle.Fill.PropertyChanged -= StyleObserver;
                }
            }
        
            if (style.EndArrowStyle != null)
            {
                style.EndArrowStyle.PropertyChanged -= StyleObserver;
                
                if (style.EndArrowStyle.Stroke != null)
                {
                    style.EndArrowStyle.Stroke.PropertyChanged -= StyleObserver;
                }
                
                if (style.EndArrowStyle.Fill != null)
                {
                    style.EndArrowStyle.Fill.PropertyChanged -= StyleObserver;
                }
            }
            
            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged -= StyleObserver;
            }
            
            Verbose("Removee Style: " + style.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        public void Add(ShapeBinding binding)
        {
            if (binding == null)
                return;
            
            binding.PropertyChanged += BindingObserver;
            Verbose("Add Bnding: " + binding.Property + ", path: " + binding.Path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        public void Remove(ShapeBinding binding)
        {
            if (binding == null)
                return;
            
            binding.PropertyChanged += BindingObserver;
            Verbose("Remove Bnding: " + binding.Property + ", path: " + binding.Path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void Add(ShapeProperty property)
        {
            if (property == null)
                return;
            
            property.PropertyChanged += PropertyObserver;
            Verbose("Add Property: " + property.Name + ", type: " + property.Value.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void Remove(ShapeProperty property)
        {
            if (property == null)
                return;
            
            property.PropertyChanged += PropertyObserver;
            Verbose("Remove Property: " + property.Name + ", type: " + property.Value.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databases"></param>
        public void Add(IEnumerable<Database> databases)
        {
            if (databases == null)
                return;
            
            foreach (var database in databases)
            {
                Add(database);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databases"></param>
        public void Remove(IEnumerable<Database> databases)
        {
            if (databases == null)
                return;
            
            foreach (var database in databases)
            {
                Remove(database);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        public void Add(IEnumerable<Column> columns)
        {
            if (columns == null)
                return;
            
            foreach (var column in columns)
            {
                Add(column);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        public void Remove(IEnumerable<Column> columns)
        {
            if (columns == null)
                return;
            
            foreach (var column in columns)
            {
                Remove(column);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="records"></param>
        public void Add(IEnumerable<Record> records)
        {
            if (records == null)
                return;
            
            foreach (var record in records)
            {
                Add(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="records"></param>
        public void Remove(IEnumerable<Record> records)
        {
            if (records == null)
                return;
            
            foreach (var record in records)
            {
                Remove(record);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Add(IEnumerable<Value> values)
        {
            if (values == null)
                return;
            
            foreach (var value in values)
            {
                Add(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Remove(IEnumerable<Value> values)
        {
            if (values == null)
                return;
            
            foreach (var value in values)
            {
                Remove(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        public void Add(IEnumerable<Document> documents)
        {
            if (documents == null)
                return;
            
            foreach (var document in documents)
            {
                Add(document);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        public void Remove(IEnumerable<Document> documents)
        {
            if (documents == null)
                return;
            
            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containers"></param>
        public void Add(IEnumerable<Container> containers)
        {
            if (containers == null)
                return;
            
            foreach (var container in containers)
            {
                Add(container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containers"></param>
        public void Remove(IEnumerable<Container> containers)
        {
            if (containers == null)
                return;
            
            foreach (var container in containers)
            {
                Remove(container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layers"></param>
        public void Add(IEnumerable<Layer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layers"></param>
        public void Remove(IEnumerable<Layer> layers)
        {
            if (layers == null)
                return;
            
            foreach (var layer in layers)
            {
                Remove(layer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        public void Add(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                return;
            
            foreach (var shape in shapes)
            {
                Add(shape);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        public void Remove(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                return;
            
            foreach (var shape in shapes)
            {
                Remove(shape);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="styles"></param>
        public void Add(IEnumerable<ShapeStyle> styles)
        {
            if (styles == null)
                return;
            
            foreach (var style in styles)
            {
                Add(style);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="styles"></param>
        public void Remove(IEnumerable<ShapeStyle> styles)
        {
            if (styles == null)
                return;
            
            foreach (var style in styles)
            {
                Remove(style);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sgs"></param>
        public void Add(IEnumerable<StyleLibrary> sgs)
        {
            if (sgs == null)
                return;
            
            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sgs"></param>
        public void Remove(IEnumerable<StyleLibrary> sgs)
        {
            if (sgs == null)
                return;
            
            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gl"></param>
        public void Add(IEnumerable<GroupLibrary> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Add(g);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gl"></param>
        public void Remove(IEnumerable<GroupLibrary> gl)
        {
            if (gl == null)
                return;

            foreach (var g in gl)
            {
                Remove(g);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindings"></param>
        public void Add(IEnumerable<ShapeBinding> bindings)
        {
            if (bindings == null)
                return;
            
            foreach (var binding in bindings)
            {
                Add(binding);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindings"></param>
        public void Remove(IEnumerable<ShapeBinding> bindings)
        {
            if (bindings == null)
                return;
            
            foreach (var binding in bindings)
            {
                Remove(binding);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        public void Add(IEnumerable<ShapeProperty> properties)
        {
            if (properties == null)
                return;
            
            foreach (var property in properties)
            {
                Add(property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        public void Remove(IEnumerable<ShapeProperty> properties)
        {
            if (properties == null)
                return;
            
            foreach (var property in properties)
            {
                Remove(property);
            }
        }
    }
}
