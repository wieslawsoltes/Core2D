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
        private readonly Editor _editor;
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
            _editor = editor;

            _invalidateContainer = () =>
            {
            };

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        [System.Diagnostics.Conditional("VERBOSE")]
        private void Debug(string text)
        {
            System.Diagnostics.Debug.Print(text);
        } 

        #endregion

        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContainerObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug(
                "Container: " +
                (sender is Container ? (sender as Container).Name : sender.GetType().ToString()) +
                ", Property: " +
                e.PropertyName);
            _invalidateContainer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContainerBackgroudObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug(
                "Background: " +
                sender.GetType().ToString() +
                ", Property: " +
                e.PropertyName);
            _editor.Project.CurrentContainer.Notify("Background");
            if (_editor.Project.CurrentContainer.Template != null)
            {
                _editor.Project.CurrentContainer.Template.Notify("Background");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        private void InitializeStyles(Project project)
        {
            (project.StyleGroups as ObservableCollection<ShapeStyleGroup>)
                .CollectionChanged += StyleGroupsCollectionObserver;

            foreach (var sg in project.StyleGroups)
            {
                Add(sg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        private void Add(ShapeStyleGroup sg)
        {
            Add(sg.Styles);

            (sg.Styles as ObservableCollection<ShapeStyle>)
                .CollectionChanged += StylesCollectionObserver;

            sg.PropertyChanged += StyleGroupObserver;
            Debug("Add Style Group: " + sg.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sg"></param>
        private void Remove(ShapeStyleGroup sg)
        {
            Remove(sg.Styles);

            (sg.Styles as ObservableCollection<ShapeStyle>)
                .CollectionChanged -= StylesCollectionObserver;

            sg.PropertyChanged -= StyleGroupObserver;
            Debug("Remove Style Group: " + sg.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="styles"></param>
        private void Add(IEnumerable<ShapeStyle> styles)
        {
            foreach (var style in styles)
            {
                Add(style);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="styles"></param>
        private void Remove(IEnumerable<ShapeStyle> styles)
        {
            foreach (var style in styles)
            {
                Remove(style);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sgs"></param>
        private void Add(IEnumerable<ShapeStyleGroup> sgs)
        {
            foreach (var sg in sgs)
            {
                Add(sg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sgs"></param>
        private void Remove(IEnumerable<ShapeStyleGroup> sgs)
        {
            foreach (var sg in sgs)
            {
                Remove(sg);
            }
        }

        #endregion

        #region Project

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        private void Add(Container container)
        {
            //container.PropertyChanged += ContainerObserver;
            container.Background.PropertyChanged += ContainerBackgroudObserver;
            Debug("Add Container: " + container.Name);
            Add(container.Layers);

            (container.Layers as ObservableCollection<Layer>)
                .CollectionChanged += LayersCollectionObserver;

            Add(container.Properties);
            (container.Properties as ObservableCollection<ShapeProperty>)
                .CollectionChanged += PropertiesCollectionObserver;

            //Add(container.WorkingLayer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        private void Add(Layer layer)
        {
            layer.PropertyChanged += LayerObserver;
            Debug("Add Layer: " + layer.Name);
            InitializeShapes(layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        private void Remove(Container container)
        {
            //container.PropertyChanged -= ContainerObserver;
            container.Background.PropertyChanged -= ContainerBackgroudObserver;
            Debug("Remove Container: " + container.Name);
            Add(container.Layers);

            (container.Layers as ObservableCollection<Layer>)
                .CollectionChanged -= LayersCollectionObserver;

            Remove(container.Properties);
            (container.Properties as ObservableCollection<ShapeProperty>)
                .CollectionChanged -= PropertiesCollectionObserver;

            //Remove(container.WorkingLayer);
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        private void Remove(Layer layer)
        {
            layer.PropertyChanged -= LayerObserver;
            Debug("Remove Layer: " + layer.Name);
            Remove(layer.Shapes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        private void Add(IEnumerable<Document> documents)
        {
            foreach (var document in documents)
            {
                Add(document);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        private void Remove(IEnumerable<Document> documents)
        {
            foreach (var document in documents)
            {
                Remove(document);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containers"></param>
        private void Add(IEnumerable<Container> containers)
        {
            foreach (var container in containers)
            {
                Add(container);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containers"></param>
        private void Remove(IEnumerable<Container> containers)
        {
            foreach (var container in containers)
            {
                Remove(container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layers"></param>
        private void Add(IEnumerable<Layer> layers)
        {
            foreach (var layer in layers)
            {
                Add(layer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layers"></param>
        private void Remove(IEnumerable<Layer> layers)
        {
            foreach (var layer in layers)
            {
                Remove(layer);
            }
        }

        #endregion

        #region Shapes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        private void InitializeShapes(Layer layer)
        {
            Add(layer.Shapes);

            (layer.Shapes as ObservableCollection<BaseShape>)
                .CollectionChanged += ShapesCollectionObserver;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
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
                (group.Shapes as ObservableCollection<BaseShape>)
                    .CollectionChanged += ShapesCollectionObserver;
                (group.Connectors as ObservableCollection<XPoint>)
                    .CollectionChanged += ShapesCollectionObserver;
                (group.Database.Records as ObservableCollection<KeyValuePair<string, ShapeProperty>>)
                    .CollectionChanged += DatabaseCollectionObserver;
            }

            Debug("Add Shape: " + shape.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
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
                (group.Shapes as ObservableCollection<BaseShape>)
                    .CollectionChanged -= ShapesCollectionObserver;
                (group.Connectors as ObservableCollection<XPoint>)
                    .CollectionChanged -= ShapesCollectionObserver;
                (group.Database.Records as ObservableCollection<KeyValuePair<string, ShapeProperty>>)
                    .CollectionChanged -= DatabaseCollectionObserver;
            }

            Debug("Remove Shape: " + shape.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        private void Add(IEnumerable<BaseShape> shapes)
        {
            foreach (var shape in shapes)
            {
                Add(shape);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        private void Remove(IEnumerable<BaseShape> shapes)
        {
            foreach (var shape in shapes)
            {
                Remove(shape);
            }
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        private void Add(ShapeProperty property)
        {
            property.PropertyChanged += PropertyObserver;
            Debug("Add Property: " + property.Name + ", type: " + property.Data.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        private void Remove(ShapeProperty property)
        {
            property.PropertyChanged += PropertyObserver;
            Debug("Remove Property: " + property.Name + ", type: " + property.Data.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        private void Add(IEnumerable<ShapeProperty> properties)
        {
            foreach (var property in properties)
            {
                Add(property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        private void Remove(IEnumerable<ShapeProperty> properties)
        {
            foreach (var property in properties)
            {
                Remove(property);
            }
        }
        
        #endregion

        #region Database

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        private void Add(KeyValuePair<string, ShapeProperty> property)
        {
            property.Value.PropertyChanged += PropertyObserver;

            Debug("Add Property: " + property.Key + ", type: " + property.Value.Data.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        private void Remove(KeyValuePair<string, ShapeProperty> property)
        {
            property.Value.PropertyChanged += PropertyObserver;

            Debug("Remove Property: " + property.Key + ", type: " + property.Value.Data.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        private void Add(IEnumerable<KeyValuePair<string, ShapeProperty>> properties)
        {
            foreach (var property in properties)
            {
                Add(property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
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
