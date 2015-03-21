using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Test.Core;

namespace Test
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            var container = CreateContainer();
            var renderer = new WpfRenderer();

            var layers = new Dictionary<ILayer, WpfElement>();

            foreach (var layer in container.Layers)
            {
                var element = CreateElement(renderer, layer);
                layers.Add(layer, element);
                canvasLayers.Children.Add(element);
            }

            var workingElement = CreateElement(renderer, container.WorkingLayer);
            canvasWorking.Children.Add(workingElement);

            var editor = new ContainerEditor(container)
            {
                SnapToGrid = false,
                SnapX = 15.0,
                SnapY = 15.0,
                DefaultIsFilled = false,
                CurrentTool = Tool.Line,
                CurrentState = State.None
            };

            canvasWorking.PreviewMouseLeftButtonDown += (s, e) =>
            {
                var p = e.GetPosition(canvasWorking);
                editor.Left(p.X, p.Y);
            };

            canvasWorking.PreviewMouseRightButtonDown += (s, e) =>
            {
                var p = e.GetPosition(canvasWorking);
                editor.Right(p.X, p.Y);
            };

            canvasWorking.PreviewMouseMove += (s, e) =>
            {
                var p = e.GetPosition(canvasWorking);
                editor.Move(p.X, p.Y);
            };

            fileExit.Click += (s, e) => this.Close();

            editClear.Click += (s, e) =>
            {
                Clear(container);
                Invalidate(container);
            };

            toolNone.Click += (s, e) => editor.CurrentTool = Tool.None;
            toolLine.Click += (s, e) => editor.CurrentTool = Tool.Line;
            toolRectangle.Click += (s, e) => editor.CurrentTool = Tool.Rectangle;
            toolEllipse.Click += (s, e) => editor.CurrentTool = Tool.Ellipse;
            toolBezier.Click += (s, e) => editor.CurrentTool = Tool.Bezier;

            layersAdd.Click += (s, e) =>
            {
                var layer = new XLayer()
                {
                    Name = "New",
                    Shapes = new ObservableCollection<XShape>()
                };
                container.Layers.Add(layer);

                var element = CreateElement(renderer, layer);
                layers.Add(layer, element);
                canvasLayers.Children.Add(element);
            };

            layersRemove.Click += (s, e) =>
            {
                var layer = container.CurrentLayer;
                container.Layers.Remove(layer);
                var element = layers[layer];
                layers.Remove(layer);
                canvasLayers.Children.Remove(element);
                container.CurrentLayer = null;
                container.CurrentLayer = container.Layers.FirstOrDefault();
                Invalidate(container);
            };

            stylesAdd.Click += (s, e) =>
            {
                container.Styles.Add(
                    XStyle.Create("New", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            };

            stylesRemove.Click += (s, e) =>
            {
                container.Styles.Remove(container.CurrentStyle);
            };

            shapesRemove.Click += (s, e) =>
            {
                container.CurrentLayer.Shapes.Remove(container.CurrentShape);
                Invalidate(container);
            };

            this.DataContext = container;
            this.menu.DataContext = editor;
        }

        private IContainer CreateContainer()
        {
            var container = new XContainer()
            {
                Layers = new ObservableCollection<ILayer>(),
                Styles = new ObservableCollection<XStyle>()
            };

            container.Layers.Add(
                new XLayer() 
                { 
                    Name = "Layer1", 
                    Shapes = new ObservableCollection<XShape>() 
                });
            container.Layers.Add(
                new XLayer() 
                { 
                    Name = "Layer2", 
                    Shapes = new ObservableCollection<XShape>() 
                });
            container.Layers.Add(
                new XLayer() 
                { 
                    Name = "Layer3", 
                    Shapes = new ObservableCollection<XShape>() 
                });
            container.Layers.Add(
                new XLayer() 
                { 
                    Name = "Layer4", 
                    Shapes = new ObservableCollection<XShape>() 
                });

            container.CurrentLayer = container.Layers.FirstOrDefault();

            container.WorkingLayer = new XLayer() 
            { 
                Name = "Working", 
                Shapes = new ObservableCollection<XShape>() 
            };

            container.Styles.Add(
                XStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
            container.Styles.Add(
                XStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
            container.Styles.Add(
                XStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
            container.Styles.Add(
                XStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));
            container.Styles.Add(
                XStyle.Create("Cyan", 255, 0, 255, 255, 255, 0, 255, 255, 2.0));

            container.CurrentStyle = container.Styles.FirstOrDefault();

            return container;
        }

        private WpfElement CreateElement(IRenderer renderer, ILayer layer)
        {
            var element = new WpfElement(layer, renderer)
            {
                Width = 800,
                Height = 600
            };

            layer.Invalidate = element.Invalidate;
            return element;
        }

        private void Clear(IContainer container)
        {
            foreach (var layer in container.Layers)
            {
                layer.Shapes.Clear();
            }
            container.WorkingLayer.Shapes.Clear();
        }

        private void Invalidate(IContainer container)
        {
            foreach (var layer in container.Layers)
            {
                layer.Invalidate();
            }
            container.WorkingLayer.Invalidate();
        }
    }
}
