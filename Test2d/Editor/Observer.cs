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

        private void RecordsCollectionObserver(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Records Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Records Replace");
                    Remove(e.OldItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    Add(e.NewItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Records Reset");
                    break;
            }

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

        private void PropertyObserver(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Property: " + sender.GetType() + ", Property: " + e.PropertyName);
            _invalidateShapes();
        }

        private void Add(Project project)
        {
            Debug("Add Project: " + project.Name);
            (project.Documents as ObservableCollection<Document>).CollectionChanged += DocumentsCollectionObserver;
            
            foreach (var document in project.Documents)
            {
                Add(document);
            }
            
            (project.Templates as ObservableCollection<Container>).CollectionChanged += ContainersCollectionObserver;
            
            foreach (var template in project.Templates)
            {
                Add(template);
            }

            (project.StyleGroups as ObservableCollection<ShapeStyleGroup>).CollectionChanged += StyleGroupsCollectionObserver;

            foreach (var sg in project.StyleGroups)
            {
                Add(sg);
            }
        }

        private void Remove(Project project)
        {
            Debug("Remove Project: " + project.Name);
            (project.Documents as ObservableCollection<Document>).CollectionChanged -= DocumentsCollectionObserver;

            foreach (var document in project.Documents)
            {
                Remove(document);
            }

            (project.Templates as ObservableCollection<Container>).CollectionChanged -= ContainersCollectionObserver;

            foreach (var template in project.Templates)
            {
                Remove(template);
            }

            (project.StyleGroups as ObservableCollection<ShapeStyleGroup>).CollectionChanged -= StyleGroupsCollectionObserver;

            foreach (var sg in project.StyleGroups)
            {
                Remove(sg);
            }
        }

        private void Add(Document document)
        {
            Debug("Add Document: " + document.Name);
            (document.Containers as ObservableCollection<Container>)
                .CollectionChanged += ContainersCollectionObserver;
            
            foreach (var container in document.Containers)
            {
                Add(container);
            }
        }

        private void Remove(Document document)
        {
            Debug("Remove Document: " + document.Name);
            (document.Containers as ObservableCollection<Container>).CollectionChanged -= ContainersCollectionObserver;
            
            foreach (var container in document.Containers)
            {
                Remove(container);
            }
        }

        private void Add(Container container)
        {
            //container.PropertyChanged += ContainerObserver;
            container.Background.PropertyChanged += ContainerBackgroudObserver;
            Debug("Add Container: " + container.Name);
            Add(container.Layers);
            (container.Layers as ObservableCollection<Layer>).CollectionChanged += LayersCollectionObserver;
            Add(container.Properties);
            (container.Properties as ObservableCollection<ShapeProperty>).CollectionChanged += PropertiesCollectionObserver;
            //Add(container.WorkingLayer);
        }

        private void Remove(Container container)
        {
            //container.PropertyChanged -= ContainerObserver;
            container.Background.PropertyChanged -= ContainerBackgroudObserver;
            Debug("Remove Container: " + container.Name);
            Add(container.Layers);
            (container.Layers as ObservableCollection<Layer>).CollectionChanged -= LayersCollectionObserver;
            Remove(container.Properties);
            (container.Properties as ObservableCollection<ShapeProperty>).CollectionChanged -= PropertiesCollectionObserver;
            //Remove(container.WorkingLayer);
        }

        private void Add(Layer layer)
        {
            layer.PropertyChanged += LayerObserver;
            Debug("Add Layer: " + layer.Name);
            Add(layer.Shapes);
            (layer.Shapes as ObservableCollection<BaseShape>).CollectionChanged += ShapesCollectionObserver;
        }

        private void Remove(Layer layer)
        {
            layer.PropertyChanged -= LayerObserver;
            Debug("Remove Layer: " + layer.Name);
            Remove(layer.Shapes);
            (layer.Shapes as ObservableCollection<BaseShape>).CollectionChanged -= ShapesCollectionObserver;
        }

        private void Add(BaseShape shape)
        {
            shape.PropertyChanged += ShapeObserver;
            Add(shape.Properties);
            (shape.Properties as ObservableCollection<ShapeProperty>).CollectionChanged += PropertiesCollectionObserver; 
            
            if (shape is XPoint)
            {
                var point = shape as XPoint;
                point.Shape.PropertyChanged += ShapeObserver;
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;
                line.Start.PropertyChanged += ShapeObserver;
                line.End.PropertyChanged += ShapeObserver;
            }
            else if (shape is XRectangle)
            {
                var rectangle = shape as XRectangle;
                rectangle.TopLeft.PropertyChanged += ShapeObserver;
                rectangle.BottomRight.PropertyChanged += ShapeObserver;
            }
            else if (shape is XEllipse)
            {
                var ellipse = shape as XEllipse;
                ellipse.TopLeft.PropertyChanged += ShapeObserver;
                ellipse.BottomRight.PropertyChanged += ShapeObserver;
            }
            else if (shape is XArc)
            {
                var arc = shape as XArc;
                arc.Point1.PropertyChanged += ShapeObserver;
                arc.Point2.PropertyChanged += ShapeObserver;
                arc.Point3.PropertyChanged += ShapeObserver;
                arc.Point4.PropertyChanged += ShapeObserver;
            }
            else if (shape is XBezier)
            {
                var bezier = shape as XBezier;
                bezier.Point1.PropertyChanged += ShapeObserver;
                bezier.Point2.PropertyChanged += ShapeObserver;
                bezier.Point3.PropertyChanged += ShapeObserver;
                bezier.Point4.PropertyChanged += ShapeObserver;
            }
            else if (shape is XQBezier)
            {
                var qbezier = shape as XQBezier;
                qbezier.Point1.PropertyChanged += ShapeObserver;
                qbezier.Point2.PropertyChanged += ShapeObserver;
                qbezier.Point3.PropertyChanged += ShapeObserver;
            }
            else if (shape is XText)
            {
                var text = shape as XText;
                text.TopLeft.PropertyChanged += ShapeObserver;
                text.BottomRight.PropertyChanged += ShapeObserver;
            }
            else if (shape is XImage)
            {
                var image = shape as XImage;
                image.TopLeft.PropertyChanged += ShapeObserver;
                image.BottomRight.PropertyChanged += ShapeObserver;
            }
            else if (shape is XGroup)
            {
                var group = shape as XGroup;
                Add(group.Shapes);
                Add(group.Connectors);
                Add(group.Database.Records);
                (group.Shapes as ObservableCollection<BaseShape>).CollectionChanged += ShapesCollectionObserver;
                (group.Connectors as ObservableCollection<XPoint>).CollectionChanged += ShapesCollectionObserver;
                (group.Database.Records as ObservableCollection<KeyValuePair<string, ShapeProperty>>).CollectionChanged += RecordsCollectionObserver;
            }

            Debug("Add Shape: " + shape.GetType());
        }

        private void Remove(BaseShape shape)
        {
            shape.PropertyChanged -= ShapeObserver;
            Remove(shape.Properties);
            (shape.Properties as ObservableCollection<ShapeProperty>).CollectionChanged -= PropertiesCollectionObserver;
            
            if (shape is XPoint)
            {
                var point = shape as XPoint;
                point.Shape.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XLine)
            {
                var line = shape as XLine;
                line.Start.PropertyChanged -= ShapeObserver;
                line.End.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XRectangle)
            {
                var rectangle = shape as XRectangle;
                rectangle.TopLeft.PropertyChanged -= ShapeObserver;
                rectangle.BottomRight.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XEllipse)
            {
                var ellipse = shape as XEllipse;
                ellipse.TopLeft.PropertyChanged -= ShapeObserver;
                ellipse.BottomRight.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XArc)
            {
                var arc = shape as XArc;
                arc.Point1.PropertyChanged -= ShapeObserver;
                arc.Point2.PropertyChanged -= ShapeObserver;
                arc.Point3.PropertyChanged -= ShapeObserver;
                arc.Point4.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XBezier)
            {
                var bezier = shape as XBezier;
                bezier.Point1.PropertyChanged -= ShapeObserver;
                bezier.Point2.PropertyChanged -= ShapeObserver;
                bezier.Point3.PropertyChanged -= ShapeObserver;
                bezier.Point4.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XQBezier)
            {
                var qbezier = shape as XQBezier;
                qbezier.Point1.PropertyChanged -= ShapeObserver;
                qbezier.Point2.PropertyChanged -= ShapeObserver;
                qbezier.Point3.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XText)
            {
                var text = shape as XText;
                text.TopLeft.PropertyChanged -= ShapeObserver;
                text.BottomRight.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XImage)
            {
                var image = shape as XImage;
                image.TopLeft.PropertyChanged -= ShapeObserver;
                image.BottomRight.PropertyChanged -= ShapeObserver;
            }
            else if (shape is XGroup)
            {
                var group = shape as XGroup;
                Remove(group.Shapes);
                Remove(group.Connectors);
                Remove(group.Database.Records);
                (group.Shapes as ObservableCollection<BaseShape>).CollectionChanged -= ShapesCollectionObserver;
                (group.Connectors as ObservableCollection<XPoint>).CollectionChanged -= ShapesCollectionObserver;
                (group.Database.Records as ObservableCollection<KeyValuePair<string, ShapeProperty>>).CollectionChanged -= RecordsCollectionObserver;
            }
            
            Debug("Remove Shape: " + shape.GetType());
        }

        private void Add(ShapeStyleGroup sg)
        {
            Add(sg.Styles);
            (sg.Styles as ObservableCollection<ShapeStyle>).CollectionChanged += StylesCollectionObserver;
            sg.PropertyChanged += StyleGroupObserver;
            Debug("Add Style Group: " + sg.Name);
        }

        private void Remove(ShapeStyleGroup sg)
        {
            Remove(sg.Styles);
            (sg.Styles as ObservableCollection<ShapeStyle>).CollectionChanged -= StylesCollectionObserver;
            sg.PropertyChanged -= StyleGroupObserver;
            Debug("Remove Style Group: " + sg.Name);
        }

        private void Add(ShapeStyle style)
        {
            style.PropertyChanged += StyleObserver;
            style.Stroke.PropertyChanged += StyleObserver;
            style.Fill.PropertyChanged += StyleObserver;
            style.LineStyle.PropertyChanged += StyleObserver;
            style.LineStyle.StartArrowStyle.PropertyChanged += StyleObserver;
            style.LineStyle.EndArrowStyle.PropertyChanged += StyleObserver;
            style.TextStyle.PropertyChanged += StyleObserver;
            Debug("Add Style: " + style.Name);
        }

        private void Remove(ShapeStyle style)
        {
            style.PropertyChanged -= StyleObserver;
            style.Stroke.PropertyChanged -= StyleObserver;
            style.Fill.PropertyChanged -= StyleObserver;
            style.LineStyle.PropertyChanged -= StyleObserver;
            style.LineStyle.StartArrowStyle.PropertyChanged -= StyleObserver;
            style.LineStyle.EndArrowStyle.PropertyChanged -= StyleObserver;
            style.TextStyle.PropertyChanged -= StyleObserver;
            Debug("Remove Style: " + style.Name);
        }

        private void Add(ShapeProperty property)
        {
            property.PropertyChanged += PropertyObserver;
            Debug("Add Property: " + property.Name + ", type: " + property.Data.GetType());
        }

        private void Remove(ShapeProperty property)
        {
            property.PropertyChanged += PropertyObserver;
            Debug("Remove Property: " + property.Name + ", type: " + property.Data.GetType());
        }

        private void Add(KeyValuePair<string, ShapeProperty> record)
        {
            record.Value.PropertyChanged += PropertyObserver;
            Debug("Add Record: " + record.Key + ", type: " + record.Value.Data.GetType());
        }

        private void Remove(KeyValuePair<string, ShapeProperty> record)
        {
            record.Value.PropertyChanged += PropertyObserver;
            Debug("Remove Record: " + record.Key + ", type: " + record.Value.Data.GetType());
        }

        private void Add(IEnumerable<Document> documents)
        {
            foreach (var document in documents)
            {
                Add(document);
            }
        }

        private void Remove(IEnumerable<Document> documents)
        {
            foreach (var document in documents)
            {
                Remove(document);
            }
        }

        private void Add(IEnumerable<Container> containers)
        {
            foreach (var container in containers)
            {
                Add(container);
            }
        }

        private void Remove(IEnumerable<Container> containers)
        {
            foreach (var container in containers)
            {
                Remove(container);
            }
        }

        private void Add(IEnumerable<Layer> layers)
        {
            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        private void Remove(IEnumerable<Layer> layers)
        {
            foreach (var layer in layers)
            {
                Remove(layer);
            }
        }

        private void Add(IEnumerable<BaseShape> shapes)
        {
            foreach (var shape in shapes)
            {
                Add(shape);
            }
        }

        private void Remove(IEnumerable<BaseShape> shapes)
        {
            foreach (var shape in shapes)
            {
                Remove(shape);
            }
        }

        private void Add(IEnumerable<ShapeStyle> styles)
        {
            foreach (var style in styles)
            {
                Add(style);
            }
        }

        private void Remove(IEnumerable<ShapeStyle> styles)
        {
            foreach (var style in styles)
            {
                Remove(style);
            }
        }

        private void Add(IEnumerable<ShapeStyleGroup> sgs)
        {
            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        private void Remove(IEnumerable<ShapeStyleGroup> sgs)
        {
            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        private void Add(IEnumerable<ShapeProperty> properties)
        {
            foreach (var property in properties)
            {
                Add(property);
            }
        }

        private void Remove(IEnumerable<ShapeProperty> properties)
        {
            foreach (var property in properties)
            {
                Remove(property);
            }
        }

        private void Add(IEnumerable<KeyValuePair<string, ShapeProperty>> records)
        {
            foreach (var record in records)
            {
                Add(record);
            }
        }

        private void Remove(IEnumerable<KeyValuePair<string, ShapeProperty>> records)
        {
            foreach (var record in records)
            {
                Remove(record);
            }
        }
    }
}
