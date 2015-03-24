using Microsoft.Win32;
using Newtonsoft.Json;
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

            var container = XContainer.Create(800, 600);
            var editor = ContainerEditor.Create(container);
            var renderer = new WpfRenderer() { DrawPoints = false };
            var layers = CreateLayers(container, renderer);

            canvasWorking.PreviewMouseLeftButtonDown += (s, e) =>
            {
                if (container.CurrentLayer != null)
                {
                    var p = e.GetPosition(canvasWorking);
                    editor.Left(p.X, p.Y);
                }
            };

            canvasWorking.PreviewMouseRightButtonDown += (s, e) =>
            {
                if (container.CurrentLayer != null)
                {
                    var p = e.GetPosition(canvasWorking);
                    editor.Right(p.X, p.Y); 
                }
            };

            canvasWorking.PreviewMouseMove += (s, e) =>
            {
                if (container.CurrentLayer != null)
                {
                    var p = e.GetPosition(canvasWorking);
                    editor.Move(p.X, p.Y); 
                }
            };

            fileOpen.Click += (s, e) =>
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                    FilterIndex = 0
                };

                if (dlg.ShowDialog() == true)
                {
                    var json = System.IO.File.ReadAllText(dlg.FileName, Encoding.UTF8);

                    var result = JsonConvert.DeserializeObject<XContainer>(
                        json,
                        new JsonSerializerSettings()
                        {
                            Formatting = Formatting.Indented,
                            TypeNameHandling = TypeNameHandling.Objects,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            ContractResolver = new ListContractResolver()
                        });

                    canvasLayers.Children.Clear();
                    canvasWorking.Children.Clear();

                    container = result;
                    editor.Container = container;

                    this.DataContext = container;

                    renderer.ClearCache();
                    layers = CreateLayers(container, renderer);

                    container.Invalidate();
                }
            };

            fileSaveAs.Click += (s, e) =>
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = "container"
                };

                if (dlg.ShowDialog() == true)
                {
                    var json = JsonConvert.SerializeObject(
                        container,
                        new JsonSerializerSettings()
                        {
                            Formatting = Formatting.Indented,
                            TypeNameHandling = TypeNameHandling.Objects,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                        });

                    System.IO.File.WriteAllText(dlg.FileName, json, Encoding.UTF8);
                }
            };

            fileExit.Click += (s, e) => this.Close();

            editClear.Click += (s, e) =>
            {
                container.Clear();
                container.Invalidate();
            };

            toolNone.Click += (s, e) => editor.CurrentTool = Tool.None;
            toolLine.Click += (s, e) => editor.CurrentTool = Tool.Line;
            toolRectangle.Click += (s, e) => editor.CurrentTool = Tool.Rectangle;
            toolEllipse.Click += (s, e) => editor.CurrentTool = Tool.Ellipse;
            toolBezier.Click += (s, e) => editor.CurrentTool = Tool.Bezier;
            toolQBezier.Click += (s, e) => editor.CurrentTool = Tool.QBezier;

            optionsDrawPoints.DataContext = renderer;
            optionsDrawPoints.Click += (s, e) => container.Invalidate();

            layersAdd.Click += (s, e) =>
            {
                var layer = XLayer.Create("New");
                container.Layers.Add(layer);

                var element = WpfElement.Create(
                    renderer, 
                    layer, 
                    container.Width, 
                    container.Height);
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
                container.CurrentLayer = container.Layers.FirstOrDefault();
                container.Invalidate();
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
                container.Invalidate();
            };

            this.DataContext = container;
            this.menu.DataContext = editor;
        }

        private IDictionary<ILayer, WpfElement> CreateLayers(IContainer container, IRenderer renderer)
        {
            var layers = new Dictionary<ILayer, WpfElement>();

            foreach (var layer in container.Layers)
            {
                var element = WpfElement.Create(
                    renderer,
                    layer,
                    container.Width,
                    container.Height);
                layers.Add(layer, element);
                canvasLayers.Children.Add(element);
            }

            var workingElement = WpfElement.Create(
                renderer,
                container.WorkingLayer,
                container.Width,
                container.Height);
            canvasWorking.Children.Add(workingElement);

            return layers;
        }

        private static void GroupTest(IContainer container)
        {
            var g = XGroup.Create("g");
            g.Shapes.Add(XLine.Create(30, 30, 30, 60, container.CurrentStyle, container.PointShape));
            g.Shapes.Add(XLine.Create(60, 30, 60, 60, container.CurrentStyle, container.PointShape));
            g.Shapes.Add(XLine.Create(30, 30, 60, 30, container.CurrentStyle, container.PointShape));
            g.Shapes.Add(XLine.Create(30, 60, 60, 60, container.CurrentStyle, container.PointShape));
            container.CurrentLayer.Shapes.Add(g);
            container.Invalidate();
        }
    }
}
