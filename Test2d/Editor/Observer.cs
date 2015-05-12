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
    public class Observer
    {
        private readonly Editor _editor;
        private readonly Action _invalidateStyles;
        private readonly Action _invalidateLayers;
        private readonly Action _invalidateShapes;

        public Observer(Editor editor)
        {
            _editor = editor;

            _invalidateStyles = () =>
            {
                _editor.Renderer.ClearCache();
                _editor.Project.CurrentContainer.Invalidate();
            };

            _invalidateLayers = () =>
            {
                _editor.Project.CurrentContainer.Invalidate();
            };

            _invalidateShapes = () =>
            {
                _editor.Project.CurrentContainer.Invalidate();
            };

            InitializeStyles(_editor.Project);
            Add(_editor.Project);
        }

        #region Debug

        [System.Diagnostics.Conditional("VERBOSE")]
        private void Debug(string text)
        {
            System.Diagnostics.Debug.Print(text);
        } 

        #endregion

        #region Handlers

        private void StyleGroupsCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<ShapeStyleGroup>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Style Group Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<ShapeStyleGroup>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Style Group Replace");
                    Remove(e.OldItems.Cast<ShapeStyleGroup>());
                    Add(e.NewItems.Cast<ShapeStyleGroup>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Style Group Reset");
                    break;
            }

            _invalidateStyles();
        }

        private void StylesCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
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

        private void DocumentsCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
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
        
         private void ContainersCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
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

        private void LayersCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
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

        private void ShapesCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
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

        private void PropertiesCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<ShapeProperty>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Property Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<ShapeProperty>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Property Replace");
                    Remove(e.OldItems.Cast<ShapeProperty>());
                    Add(e.NewItems.Cast<ShapeProperty>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Property Reset");
                    break;
            }

            _invalidateShapes();
        }
        
        private void DatabaseCollectionObserver(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    Debug("Database Replace");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug("Database Replace");
                    Remove(e.OldItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    Add(e.NewItems.Cast<KeyValuePair<string, ShapeProperty>>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Debug("Database Reset");
                    break;
            }

            _invalidateShapes();
        }

        private void StyleGroupObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug(
                "Style Group: " +
                (sender is ShapeStyle ? (sender as ShapeStyle).Name : sender.GetType().ToString()) +
                ", Property: " +
                e.PropertyName);
            _invalidateStyles();
        }

        private void StyleObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug(
                "Style: " + 
                (sender is ShapeStyle ? (sender as ShapeStyle).Name : sender.GetType().ToString()) + 
                ", Property: " + 
                e.PropertyName);
            _invalidateStyles();
        }

        private void LayerObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug(
                "Layer: " + 
                (sender is Layer ? (sender as Layer).Name : sender.GetType().ToString()) + 
                ", Property: " + 
                e.PropertyName);
            _invalidateLayers();
        }

        private void ShapeObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug(
                "Shape: " + 
                sender.GetType() + 
                ", Property: " + 
                e.PropertyName);
            _invalidateShapes();
        }

        private void PropertyObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug(
                "Property: " +
                sender.GetType() +
                ", Property: " +
                e.PropertyName);
            _invalidateShapes();
        }

        #endregion

        #region Styles

        private void InitializeStyles(Project project)
        {
            (project.StyleGroups as ObservableCollection<ShapeStyleGroup>)
                .CollectionChanged += StyleGroupsCollectionObserver;

            foreach (var sg in project.StyleGroups)
            {
                Add(sg);
            }
        }

        private void Add(ShapeStyleGroup sg)
        {
            Add(sg.Styles);

            (sg.Styles as ObservableCollection<ShapeStyle>)
                .CollectionChanged += StylesCollectionObserver;

            sg.PropertyChanged += StyleGroupObserver;
            Debug("Add Style Group: " + sg.Name);
        }

        private void Remove(ShapeStyleGroup sg)
        {
            Remove(sg.Styles);

            (sg.Styles as ObservableCollection<ShapeStyle>)
                .CollectionChanged -= StylesCollectionObserver;

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

        #endregion

        #region Project

        private void Add(Project project)
        {
            Debug("Add Project: " + project.Name);
            (project.Documents as ObservableCollection<Document>)
                .CollectionChanged += DocumentsCollectionObserver;
            
            foreach (var document in project.Documents)
            {
                Add(document);
            }
            
            (project.Templates as ObservableCollection<Container>)
                .CollectionChanged += ContainersCollectionObserver;
            
            foreach (var template in project.Templates)
            {
                Add(template);
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
        
        private void Add(Container container)
        {
            Debug("Add Container: " + container.Name);
            Add(container.Layers);

            (container.Layers as ObservableCollection<Layer>)
                .CollectionChanged += LayersCollectionObserver;

            //Add(container.WorkingLayer);
        }

        private void Add(Layer layer)
        {
            layer.PropertyChanged += LayerObserver;
            Debug("Add Layer: " + layer.Name);
            InitializeShapes(layer);
        }

        private void Remove(Project project)
        {
            Debug("Remove Project: " + project.Name);
            (project.Documents as ObservableCollection<Document>)
                .CollectionChanged -= DocumentsCollectionObserver;
            
            foreach (var document in project.Documents)
            {
                Remove(document);
            }
            
            (project.Templates as ObservableCollection<Container>)
                .CollectionChanged -= ContainersCollectionObserver;
            
            foreach (var template in project.Templates)
            {
                Remove(template);
            }
        }
        
        private void Remove(Document document)
        {
            Debug("Remove Document: " + document.Name);
            (document.Containers as ObservableCollection<Container>)
                .CollectionChanged -= ContainersCollectionObserver;
            
            foreach (var container in document.Containers)
            {
                Remove(container);
            }
        }
        
        private void Remove(Container container)
        {
            Debug("Remove Container: " + container.Name);
            Add(container.Layers);

            (container.Layers as ObservableCollection<Layer>)
                .CollectionChanged -= LayersCollectionObserver;

            //Remove(container.WorkingLayer);
        }
  
        private void Remove(Layer layer)
        {
            layer.PropertyChanged -= LayerObserver;
            Debug("Remove Layer: " + layer.Name);
            Remove(layer.Shapes);
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

        #endregion

        #region Shapes

        private void InitializeShapes(Layer layer)
        {
            Add(layer.Shapes);

            (layer.Shapes as ObservableCollection<BaseShape>)
                .CollectionChanged += ShapesCollectionObserver;
        }

        private void Add(BaseShape shape)
        {
            shape.PropertyChanged += ShapeObserver;

            Add(shape.Properties);
            (shape.Properties as ObservableCollection<ShapeProperty>)
                .CollectionChanged += PropertiesCollectionObserver;
                   
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
                Add(group.Database);
                (group.Shapes as ObservableCollection<BaseShape>)
                    .CollectionChanged += ShapesCollectionObserver;
                (group.Connectors as ObservableCollection<XPoint>)
                    .CollectionChanged += ShapesCollectionObserver;
                (group.Database as ObservableCollection<KeyValuePair<string, ShapeProperty>>)
                    .CollectionChanged += DatabaseCollectionObserver;
            }

            Debug("Add Shape: " + shape.GetType());
        }

        private void Remove(BaseShape shape)
        {
            shape.PropertyChanged -= ShapeObserver;

            Remove(shape.Properties);
            (shape.Properties as ObservableCollection<ShapeProperty>)
                .CollectionChanged -= PropertiesCollectionObserver;
                 
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
                Remove(group.Database);
                (group.Shapes as ObservableCollection<BaseShape>)
                    .CollectionChanged -= ShapesCollectionObserver;
                (group.Connectors as ObservableCollection<XPoint>)
                    .CollectionChanged -= ShapesCollectionObserver;
                (group.Database as ObservableCollection<KeyValuePair<string, ShapeProperty>>)
                    .CollectionChanged -= DatabaseCollectionObserver;
            }

            Debug("Remove Shape: " + shape.GetType());
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

        #endregion

        #region Properties
        
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
        
        #endregion

        #region Database

        private void Add(KeyValuePair<string, ShapeProperty> property)
        {
            property.Value.PropertyChanged += PropertyObserver;

            Debug("Add Property: " + property.Key + ", type: " + property.Value.Data.GetType());
        }

        private void Remove(KeyValuePair<string, ShapeProperty> property)
        {
            property.Value.PropertyChanged += PropertyObserver;

            Debug("Remove Property: " + property.Key + ", type: " + property.Value.Data.GetType());
        }

        private void Add(IEnumerable<KeyValuePair<string, ShapeProperty>> properties)
        {
            foreach (var property in properties)
            {
                Add(property);
            }
        }

        private void Remove(IEnumerable<KeyValuePair<string, ShapeProperty>> properties)
        {
            foreach (var property in properties)
            {
                Remove(property);
            }
        }

        #endregion
    }
}
