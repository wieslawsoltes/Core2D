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
            var elements = CreateElements(container, renderer);

            foreach (var element in elements)
            {
                canvas.Children.Add(element);
            }

            var editor = new ContainerEditor(container)
            {
                SnapToGrid = false,
                SnapX = 15.0,
                SnapY = 15.0,
                DefaultIsFilled = false,
                CurrentTool = Tool.Line,
                CurrentState = State.None
            };

            canvas.PreviewMouseLeftButtonDown += (s, e) =>
            {
                var p = e.GetPosition(canvas);
                editor.Left(p.X, p.Y);
            };
            
            canvas.PreviewMouseRightButtonDown += (s, e) =>
            {
                var p = e.GetPosition(canvas);
                editor.Right(p.X, p.Y);
            };
            
            canvas.PreviewMouseMove += (s, e) =>
            {
                var p = e.GetPosition(canvas);
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
                container.Layers.Add(
                    new XLayer() 
                    { 
                        Name = "New", 
                        Shapes = new ObservableCollection<XShape>() 
                    });
            };

            layersRemove.Click += (s, e) =>
            {
                container.Layers.Remove(container.CurrentLayer);
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

        private IList<WpfElement> CreateElements(IContainer container, IRenderer renderer)
        {
            var elements = new List<WpfElement>();

            foreach (var layer in container.Layers)
            {
                elements.Add(
                    CreateElement(renderer, layer));
            }

            elements.Add(
                CreateElement(renderer, container.WorkingLayer));

            return elements;
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
