// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#define VERBOSE
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Core;

namespace Test.Core
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
                _editor.Container.Invalidate();
            };

            _invalidateLayers = () =>
            {
                _editor.Container.Invalidate();
            };

            _invalidateShapes = () =>
            {
                _editor.Container.Invalidate();
            };

            InitializeStyles(_editor.Container);
            InitializeLayers(_editor.Container);
            //Add(_editor.Container.WorkingLayer);
        }

        #region Debug

        [System.Diagnostics.Conditional("VERBOSE")]
        private void Debug(string text)
        {
            System.Diagnostics.Debug.Print(text);
        } 

        #endregion

        #region Handlers

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
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<ShapeStyle>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }

            _invalidateStyles();
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
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<Layer>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
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
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems.Cast<BaseShape>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }

            _invalidateShapes();
        }

        private void StyleObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Style Property: " + e.PropertyName);
            _invalidateStyles();
        }

        private void LayerObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Layer Property: " + e.PropertyName);
            _invalidateLayers();
        }

        private void ShapeObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug("Shape Property: " + e.PropertyName);
            _invalidateShapes();
        }

        #endregion

        #region Styles

        private void InitializeStyles(Container container)
        {
            Add(container.Styles);

            (container.Styles as ObservableCollection<ShapeStyle>)
                .CollectionChanged += StylesCollectionObserver;
        }

        private void Add(ShapeStyle style)
        {
            style.PropertyChanged += StyleObserver;
            style.Stroke.PropertyChanged += StyleObserver;
            style.Fill.PropertyChanged += StyleObserver;
            Debug("Add Style");
        }

        private void Remove(ShapeStyle style)
        {
            style.PropertyChanged -= StyleObserver;
            style.Stroke.PropertyChanged -= StyleObserver;
            style.Fill.PropertyChanged -= StyleObserver;
            Debug("Remove Style");
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

        #endregion

        #region Layers

        private void InitializeLayers(Container container)
        {
            Add(container.Layers);

            (container.Layers as ObservableCollection<Layer>)
                .CollectionChanged += LayersCollectionObserver;
        }

        private void Add(Layer layer)
        {
            layer.PropertyChanged += LayerObserver;
            Debug("Add Layer");
            InitializeShapes(layer);
        }

        private void Remove(Layer layer)
        {
            layer.PropertyChanged -= LayerObserver;
            Debug("Remove Layer");
            Remove(layer.Shapes);
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

            Debug("Add Shape");
        }

        private void Remove(BaseShape shape)
        {
            shape.PropertyChanged -= ShapeObserver;

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

            Debug("Remove Shape");
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
    }
}
