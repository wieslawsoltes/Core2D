// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define VERBOSE
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private readonly Editor _currentEditor;
        private readonly Action _invalidateContainer;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public Observer(Editor editor)
        {
            _currentEditor = editor;

            _invalidateContainer = () =>
            {
            };

            _invalidateStyles = () =>
            {
                _currentEditor.Renderer.ClearCache();
                _currentEditor.Project.CurrentContainer.Invalidate();
            };

            _invalidateLayers = () =>
            {
                _currentEditor.Project.CurrentContainer.Invalidate();
            };

            _invalidateShapes = () =>
            {
                _currentEditor.Project.CurrentContainer.Invalidate();
            };

            Add(_currentEditor.Project);
        }

        [System.Diagnostics.Conditional("VERBOSE")]
        private void Debug(string text)
        {
            System.Diagnostics.Debug.Print(text);
        }

        private void DatabasesCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Database>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Databases Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Database>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Databases Replace");
                    Remove(e.OldItems.Cast<Database>());
                    Add(e.NewItems.Cast<Database>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Databases Reset");
                    break;
            }

            //_invalidateLayers();
        }
        
        private void ColumnsCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Column>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Columns Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Column>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Columns Replace");
                    Remove(e.OldItems.Cast<Column>());
                    Add(e.NewItems.Cast<Column>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Columns Reset");
                    break;
            }

            //_invalidateLayers();
        }
        
        private void RecordsCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Record>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Records Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Record>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Records Replace");
                    Remove(e.OldItems.Cast<Record>());
                    Add(e.NewItems.Cast<Record>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Records Reset");
                    break;
            }

            //_invalidateLayers();
        }
        
        private void ValuesCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Value>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Values Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Value>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Values Replace");
                    Remove(e.OldItems.Cast<Value>());
                    Add(e.NewItems.Cast<Value>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Values Reset");
                    break;
            }

            _invalidateLayers();
        }
 
        private void DocumentsCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Document>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Documents Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Document>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Documents Replace");
                    Remove(e.OldItems.Cast<Document>());
                    Add(e.NewItems.Cast<Document>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Documents Reset");
                    break;
            }

            //_invalidateLayers();
        }

        private void ContainersCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Container>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Containers Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Container>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Containers Replace");
                    Remove(e.OldItems.Cast<Container>());
                    Add(e.NewItems.Cast<Container>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Containers Reset");
                    break;
            }

            //_invalidateLayers();
        }

        private void LayersCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Layer>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Layers Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Layer>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Layers Replace");
                    Remove(e.OldItems.Cast<Layer>());
                    Add(e.NewItems.Cast<Layer>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Layers Reset");
                    break;
            }

            _invalidateLayers();
        }

        private void ShapesCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<BaseShape>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Shapes Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<BaseShape>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Shapes Replace");
                    Remove(e.OldItems.Cast<BaseShape>());
                    Add(e.NewItems.Cast<BaseShape>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Shapes Reset");
                    break;
            }

            _invalidateShapes();
        }

        private void StyleGroupsCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<ShapeStyleGroup>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Style Groups Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<ShapeStyleGroup>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Style Groups Replace");
                    Remove(e.OldItems.Cast<ShapeStyleGroup>());
                    Add(e.NewItems.Cast<ShapeStyleGroup>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Style Groups Reset");
                    break;
            }

            _invalidateStyles();
        }

        private void StylesCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<ShapeStyle>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Styles Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<ShapeStyle>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Styles Replace");
                    Remove(e.OldItems.Cast<ShapeStyle>());
                    Add(e.NewItems.Cast<ShapeStyle>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Styles Reset");
                    break;
            }

            _invalidateStyles();
        }

        private void BindingsCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<ShapeBinding>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Bindings Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<ShapeBinding>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Bindings Replace");
                    Remove(e.OldItems.Cast<ShapeBinding>());
                    Add(e.NewItems.Cast<ShapeBinding>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Bindings Reset");
                    break;
            }

            _invalidateShapes();
        }
        
        private void PropertiesCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<ShapeProperty>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Properties Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<ShapeProperty>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Properties Replace");
                    Remove(e.OldItems.Cast<ShapeProperty>());
                    Add(e.NewItems.Cast<ShapeProperty>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Properties Reset");
                    break;
            }

            _invalidateShapes();
        }

        private void DatabaseObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Database: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }
        
        private void ColumnObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Column: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }
        
        private void RecordObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Record: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }
        
        private void ValueObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Value: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }
        
        private void ContainerObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Container: " + (sender is Container ? (sender as Container).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);
            _invalidateContainer();
        }

        private void ContainerBackgroudObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Background: " + sender.GetType().ToString() + ", Property: " + e.PropertyName);
            _currentEditor.Project.CurrentContainer.Notify("Background");
            if (_currentEditor.Project.CurrentContainer.Template != null)
            {
                _currentEditor.Project.CurrentContainer.Template.Notify("Background");
            }
        }

        private void LayerObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Layer: " + (sender is Layer ? (sender as Layer).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);
            _invalidateLayers();
        }

        private void ShapeObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Shape: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }

        private void StyleGroupObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Style Group: " + (sender is ShapeStyle ? (sender as ShapeStyle).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);
            _invalidateStyles();
        }

        private void StyleObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Style: " + (sender is ShapeStyle ? (sender as ShapeStyle).Name : sender.GetType().ToString()) + ", Property: " + e.PropertyName);
            _invalidateStyles();
        }

        private void BindingObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Property: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }
        
        private void PropertyObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Property: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }

        private void Add(Project project)
        {
            if (project == null)
                return;
            
            Debug("Add Project: " + project.Name);
            
            if (project.Databases != null)
            {
                (project.Databases as ObservableCollection<Database>).CollectionChanged += DatabasesCollectionObserver;
                
                foreach (var database in project.Databases)
                {
                    Add(database);
                }
            }
            
            if (project.Documents != null)
            {
                (project.Documents as ObservableCollection<Document>).CollectionChanged += DocumentsCollectionObserver;
                
                foreach (var document in project.Documents)
                {
                    Add(document);
                }
            }
            
            if (project.Templates != null)
            {
                (project.Templates as ObservableCollection<Container>).CollectionChanged += ContainersCollectionObserver;
                
                foreach (var template in project.Templates)
                {
                    Add(template);
                }
            }
            
            if (project.StyleGroups != null)
            {
                (project.StyleGroups as ObservableCollection<ShapeStyleGroup>).CollectionChanged += StyleGroupsCollectionObserver;
    
                foreach (var sg in project.StyleGroups)
                {
                    Add(sg);
                }
            }
        }

        private void Remove(Project project)
        {
            if (project == null)
                return;
            
            Debug("Remove Project: " + project.Name);
            
            if (project.Databases != null)
            {
                (project.Databases as ObservableCollection<Database>).CollectionChanged -= DatabasesCollectionObserver;
                
                foreach (var database in project.Databases)
                {
                    Remove(database);
                }
            }
            
            if (project.Documents != null)
            {
                (project.Documents as ObservableCollection<Document>).CollectionChanged -= DocumentsCollectionObserver;
    
                foreach (var document in project.Documents)
                {
                    Remove(document);
                }
            }

            if (project.Templates != null)
            {
                (project.Templates as ObservableCollection<Container>).CollectionChanged -= ContainersCollectionObserver;
    
                foreach (var template in project.Templates)
                {
                    Remove(template);
                }
            }

            if (project.StyleGroups != null)
            {
                (project.StyleGroups as ObservableCollection<ShapeStyleGroup>).CollectionChanged -= StyleGroupsCollectionObserver;
    
                foreach (var sg in project.StyleGroups)
                {
                    Remove(sg);
                }
            }
        }

        private void Add(Database database)
        {
            if (database == null)
                return;
            
            //database.PropertyChanged += DatabaseObserver;

            Debug("Add Database: " + database.Name);
            
            if (database.Columns != null)
            {
                Add(database.Columns);
                (database.Columns as ObservableCollection<Column>).CollectionChanged += ColumnsCollectionObserver;
            }
            
            if (database.Records != null)
            {
                Add(database.Records);
                (database.Records as ObservableCollection<Record>).CollectionChanged += RecordsCollectionObserver;
            }
        }
        
        private void Remove(Database database)
        {
            if (database == null)
                return;
            
            //database.PropertyChanged -= DatabaseObserver;

            Debug("Remove Database: " + database.Name);
            
            if (database.Columns != null)
            {
                Remove(database.Columns);
                (database.Columns as ObservableCollection<Column>).CollectionChanged -= ColumnsCollectionObserver;
            }
            
            if (database.Records != null)
            {
                Remove(database.Records);
                (database.Records as ObservableCollection<Record>).CollectionChanged -= RecordsCollectionObserver;
            }
        }

        private void Add(Column column)
        {
            if (column == null)
                return;

            column.PropertyChanged += ColumnObserver;

            Debug("Add Column: " + column.Id);
        }
        
        private void Remove(Column column)
        {
            if (column == null)
                return;

            column.PropertyChanged -= ColumnObserver;

            Debug("Remove Column: " + column.Id);
        }

        private void Add(Record record)
        {
            if (record == null)
                return;
            
            record.PropertyChanged += RecordObserver;
            
            if (record.Values != null)
            {
                Add(record.Values);
                (record.Values as ObservableCollection<Value>).CollectionChanged += ValuesCollectionObserver;
            }
            
            Debug("Add Record: " + record.Id);
        }

        private void Remove(Record record)
        {
            if (record == null)
                return;

            record.PropertyChanged -= RecordObserver;
            
            if (record.Values != null)
            {
                Remove(record.Values);
                (record.Values as ObservableCollection<string>).CollectionChanged -= ValuesCollectionObserver;
            }
            
            Debug("Remove Record: " + record.Id);
        }
        
        private void Add(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged += ValueObserver;

            Debug("Add Value");
        }
        
        private void Remove(Value value)
        {
            if (value == null)
                return;

            value.PropertyChanged -= ValueObserver;

            Debug("Remove Value");
        }
        
        private void Add(Document document)
        {
            if (document == null)
                return;
            
            Debug("Add Document: " + document.Name);
            
            if (document.Containers != null)
            {
                (document.Containers as ObservableCollection<Container>)
                    .CollectionChanged += ContainersCollectionObserver;
                
                foreach (var container in document.Containers)
                {
                    Add(container);
                }
            }
        }

        private void Remove(Document document)
        {
            if (document == null)
                return;
            
            Debug("Remove Document: " + document.Name);
            
            if (document.Containers != null)
            {
                (document.Containers as ObservableCollection<Container>).CollectionChanged -= ContainersCollectionObserver;
                
                foreach (var container in document.Containers)
                {
                    Remove(container);
                }
            }
        }

        private void Add(Container container)
        {
            if (container == null)
                return;
            
            //container.PropertyChanged += ContainerObserver;
            
            if (container.Background != null)
            {
                container.Background.PropertyChanged += ContainerBackgroudObserver;
            }
            
            Debug("Add Container: " + container.Name);
            
            if (container.Layers != null)
            {
                Add(container.Layers);
                (container.Layers as ObservableCollection<Layer>).CollectionChanged += LayersCollectionObserver;
            }
            
            if (container.Properties != null)
            {
                Add(container.Properties);
                (container.Properties as ObservableCollection<ShapeProperty>).CollectionChanged += PropertiesCollectionObserver;
            }
            
            //Add(container.WorkingLayer);
        }

        private void Remove(Container container)
        {
            if (container == null)
                return;
            
            //container.PropertyChanged -= ContainerObserver;
            
            if (container.Background != null)
            {
                container.Background.PropertyChanged -= ContainerBackgroudObserver;
            }
            
            Debug("Remove Container: " + container.Name);
            
            if (container.Layers != null)
            {
                Add(container.Layers);
                (container.Layers as ObservableCollection<Layer>).CollectionChanged -= LayersCollectionObserver;
            }
            
            if (container.Properties != null)
            {
                Remove(container.Properties);
                (container.Properties as ObservableCollection<ShapeProperty>).CollectionChanged -= PropertiesCollectionObserver;
            }
            
            //Remove(container.WorkingLayer);
        }

        private void Add(Layer layer)
        {
            if (layer == null)
                return;
            
            layer.PropertyChanged += LayerObserver;
            
            Debug("Add Layer: " + layer.Name);
            
            if (layer.Shapes != null)
            {
                Add(layer.Shapes);
                (layer.Shapes as ObservableCollection<BaseShape>).CollectionChanged += ShapesCollectionObserver;
            }
        }

        private void Remove(Layer layer)
        {
            if (layer == null)
                return;
            
            layer.PropertyChanged -= LayerObserver;
            
            Debug("Remove Layer: " + layer.Name);
            
            if (layer.Shapes != null)
            {
                Remove(layer.Shapes);
                (layer.Shapes as ObservableCollection<BaseShape>).CollectionChanged -= ShapesCollectionObserver;
            }
        }

        private void Add(BaseShape shape)
        {
            if (shape == null)
                return;
            
            shape.PropertyChanged += ShapeObserver;

            if (shape.Bindings != null)
            {
                Add(shape.Bindings);
                (shape.Bindings as ObservableCollection<ShapeBinding>).CollectionChanged += BindingsCollectionObserver;
            }
            
            if (shape.Properties != null)
            {
                Add(shape.Properties);
                (shape.Properties as ObservableCollection<ShapeProperty>).CollectionChanged += PropertiesCollectionObserver; 
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
            else if (shape is XGroup)
            {
                var group = shape as XGroup;
                
                if (group != null)
                {
                    if (group.Shapes != null)
                    {
                        Add(group.Shapes);
                        (group.Shapes as ObservableCollection<BaseShape>).CollectionChanged += ShapesCollectionObserver;
                    }
                    
                    if (group.Connectors != null)
                    {
                        Add(group.Connectors);
                        (group.Connectors as ObservableCollection<XPoint>).CollectionChanged += ShapesCollectionObserver;
                    }
                }
            }

            Debug("Add Shape: " + shape.GetType());
        }

        private void Remove(BaseShape shape)
        {
            if (shape == null)
                return;
            
            shape.PropertyChanged -= ShapeObserver;

            if (shape.Bindings != null)
            {
                Remove(shape.Bindings);
                (shape.Bindings as ObservableCollection<ShapeBinding>).CollectionChanged -= BindingsCollectionObserver;
            }
            
            if (shape.Properties != null)
            {
                Remove(shape.Properties);
                (shape.Properties as ObservableCollection<ShapeProperty>).CollectionChanged -= PropertiesCollectionObserver; 
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
            else if (shape is XGroup)
            {
                var group = shape as XGroup;
                
                if (group != null)
                {
                    if (group.Shapes != null)
                    {
                        Remove(group.Shapes);
                        (group.Shapes as ObservableCollection<BaseShape>).CollectionChanged -= ShapesCollectionObserver;
                    }
                    
                    if (group.Connectors != null)
                    {
                        Remove(group.Connectors);
                        (group.Connectors as ObservableCollection<XPoint>).CollectionChanged -= ShapesCollectionObserver;
                    }
                }
            }

            Debug("Remove Shape: " + shape.GetType());
        }

        private void Add(ShapeStyleGroup sg)
        {
            if (sg == null)
                return;
            
            if (sg.Styles != null)
            {
                Add(sg.Styles);
                (sg.Styles as ObservableCollection<ShapeStyle>).CollectionChanged += StylesCollectionObserver;
            }
            
            sg.PropertyChanged += StyleGroupObserver;
            Debug("Add Style Group: " + sg.Name);
        }

        private void Remove(ShapeStyleGroup sg)
        {
            if (sg == null)
                return;
            
            if (sg.Styles != null)
            {
                Remove(sg.Styles);
                (sg.Styles as ObservableCollection<ShapeStyle>).CollectionChanged -= StylesCollectionObserver;
            }
            
            sg.PropertyChanged -= StyleGroupObserver;
            Debug("Remove Style Group: " + sg.Name);
        }

        private void Add(ShapeStyle style)
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
                
                if (style.LineStyle.StartArrowStyle != null)
                {
                    style.LineStyle.StartArrowStyle.PropertyChanged += StyleObserver;
                }
            
                if (style.LineStyle.EndArrowStyle != null)
                {
                    style.LineStyle.EndArrowStyle.PropertyChanged += StyleObserver;
                }
            }
        
            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged += StyleObserver;
            }
            
            Debug("Add Style: " + style.Name);
        }

        private void Remove(ShapeStyle style)
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
                
                if (style.LineStyle.StartArrowStyle != null)
                {
                    style.LineStyle.StartArrowStyle.PropertyChanged -= StyleObserver;
                }
            
                if (style.LineStyle.EndArrowStyle != null)
                {
                    style.LineStyle.EndArrowStyle.PropertyChanged -= StyleObserver;
                }
            }
        
            if (style.TextStyle != null)
            {
                style.TextStyle.PropertyChanged -= StyleObserver;
            }
            
            Debug("Removee Style: " + style.Name);
        }

        private void Add(ShapeBinding binding)
        {
            if (binding == null)
                return;
            
            binding.PropertyChanged += BindingObserver;
            Debug("Add Bnding: " + binding.Property + ", path: " + binding.Path);
        }

        private void Remove(ShapeBinding binding)
        {
            if (binding == null)
                return;
            
            binding.PropertyChanged += BindingObserver;
            Debug("Remove Bnding: " + binding.Property + ", path: " + binding.Path);
        }
   
        private void Add(ShapeProperty property)
        {
            if (property == null)
                return;
            
            property.PropertyChanged += PropertyObserver;
            Debug("Add Property: " + property.Name + ", type: " + property.Value.GetType());
        }

        private void Remove(ShapeProperty property)
        {
            if (property == null)
                return;
            
            property.PropertyChanged += PropertyObserver;
            Debug("Remove Property: " + property.Name + ", type: " + property.Value.GetType());
        }
   
        private void Add(IEnumerable<Database> databases)
        {
            if (databases == null)
                return;
            
            foreach (var database in databases)
            {
                Add(database);
            }
        }
        
        private void Remove(IEnumerable<Database> databases)
        {
            if (databases == null)
                return;
            
            foreach (var database in databases)
            {
                Remove(database);
            }
        }

        private void Add(IEnumerable<Column> columns)
        {
            if (columns == null)
                return;
            
            foreach (var column in columns)
            {
                Add(column);
            }
        }
        
        private void Remove(IEnumerable<Column> columns)
        {
            if (columns == null)
                return;
            
            foreach (var column in columns)
            {
                Remove(column);
            }
        }

        private void Add(IEnumerable<Record> records)
        {
            if (records == null)
                return;
            
            foreach (var record in records)
            {
                Add(record);
            }
        }
        
        private void Remove(IEnumerable<Record> records)
        {
            if (records == null)
                return;
            
            foreach (var record in records)
            {
                Remove(record);
            }
        }
 
        private void Add(IEnumerable<Value> values)
        {
            if (values == null)
                return;
            
            foreach (var value in values)
            {
                Add(value);
            }
        }
        
        private void Remove(IEnumerable<Value> values)
        {
            if (values == null)
                return;
            
            foreach (var value in values)
            {
                Remove(value);
            }
        }
        
        private void Add(IEnumerable<Document> documents)
        {
            if (documents == null)
                return;
            
            foreach (var document in documents)
            {
                Add(document);
            }
        }

        private void Remove(IEnumerable<Document> documents)
        {
            if (documents == null)
                return;
            
            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        private void Add(IEnumerable<Container> containers)
        {
            if (containers == null)
                return;
            
            foreach (var container in containers)
            {
                Add(container);
            }
        }

        private void Remove(IEnumerable<Container> containers)
        {
            if (containers == null)
                return;
            
            foreach (var container in containers)
            {
                Remove(container);
            }
        }

        private void Add(IEnumerable<Layer> layers)
        {
            if (layers == null)
                return;

            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        private void Remove(IEnumerable<Layer> layers)
        {
            if (layers == null)
                return;
            
            foreach (var layer in layers)
            {
                Remove(layer);
            }
        }

        private void Add(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                return;
            
            foreach (var shape in shapes)
            {
                Add(shape);
            }
        }

        private void Remove(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
                return;
            
            foreach (var shape in shapes)
            {
                Remove(shape);
            }
        }

        private void Add(IEnumerable<ShapeStyle> styles)
        {
            if (styles == null)
                return;
            
            foreach (var style in styles)
            {
                Add(style);
            }
        }

        private void Remove(IEnumerable<ShapeStyle> styles)
        {
            if (styles == null)
                return;
            
            foreach (var style in styles)
            {
                Remove(style);
            }
        }

        private void Add(IEnumerable<ShapeStyleGroup> sgs)
        {
            if (sgs == null)
                return;
            
            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        private void Remove(IEnumerable<ShapeStyleGroup> sgs)
        {
            if (sgs == null)
                return;
            
            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        private void Add(IEnumerable<ShapeBinding> bindings)
        {
            if (bindings == null)
                return;
            
            foreach (var binding in bindings)
            {
                Add(binding);
            }
        }

        private void Remove(IEnumerable<ShapeBinding> bindings)
        {
            if (bindings == null)
                return;
            
            foreach (var binding in bindings)
            {
                Remove(binding);
            }
        }

        private void Add(IEnumerable<ShapeProperty> properties)
        {
            if (properties == null)
                return;
            
            foreach (var property in properties)
            {
                Add(property);
            }
        }

        private void Remove(IEnumerable<ShapeProperty> properties)
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
