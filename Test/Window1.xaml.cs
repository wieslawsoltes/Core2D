using Microsoft.Practices.Prism.Commands;
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

            var editor = ContainerEditor.Create(
                XContainer.Create(800, 600),
                WpfRenderer.Create(false));

            Func<IDictionary<ILayer, WpfElement>> createLayers = () =>
            {
                var dict = new Dictionary<ILayer, WpfElement>();

                foreach (var layer in editor.Container.Layers)
                {
                    var element = WpfElement.Create(
                        editor.Renderer,
                        layer,
                        editor.Container.Width,
                        editor.Container.Height);
                    dict.Add(layer, element);
                    canvasLayers.Children.Add(element);
                }

                var workingElement = WpfElement.Create(
                    editor.Renderer,
                    editor.Container.WorkingLayer,
                    editor.Container.Width,
                    editor.Container.Height);
                canvasWorking.Children.Add(workingElement);

                return dict;
            };

            var layers = createLayers();
            
            canvasWorking.PreviewMouseLeftButtonDown += (s, e) =>
            {
                if (editor.Container.CurrentLayer != null)
                {
                    var p = e.GetPosition(canvasWorking);
                    editor.Left(p.X, p.Y);
                }
            };

            canvasWorking.PreviewMouseRightButtonDown += (s, e) =>
            {
                if (editor.Container.CurrentLayer != null)
                {
                    var p = e.GetPosition(canvasWorking);
                    editor.Right(p.X, p.Y); 
                }
            };

            canvasWorking.PreviewMouseMove += (s, e) =>
            {
                if (editor.Container.CurrentLayer != null)
                {
                    var p = e.GetPosition(canvasWorking);
                    editor.Move(p.X, p.Y); 
                }
            };

            Action<IContainer> loadContainer = (container) =>
            {
                canvasLayers.Children.Clear();
                canvasWorking.Children.Clear();

                editor.Renderer.ClearCache();
                editor.Container = container;

                layers = createLayers();

                editor.Container.Invalidate();

                DataContext = editor;
            };

            editor.NewCommand = new DelegateCommand(() =>
            {
                loadContainer(XContainer.Create(800, 600));
            });

            editor.OpenCommand = new DelegateCommand(() =>
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                    FilterIndex = 0
                };

                if (dlg.ShowDialog() == true)
                {
                    var path = dlg.FileName;

                    var json = System.IO.File.ReadAllText(path, Encoding.UTF8);

                    var container = JsonConvert.DeserializeObject<XContainer>(
                        json,
                        new JsonSerializerSettings()
                        {
                            Formatting = Formatting.Indented,
                            TypeNameHandling = TypeNameHandling.Objects,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            ContractResolver = new ListContractResolver()
                        });

                    loadContainer(container);
                }
            });

            editor.SaveAsCommand = new DelegateCommand(() =>
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                    FilterIndex = 0,
                    FileName = "container"
                };

                if (dlg.ShowDialog() == true)
                {
                    var path = dlg.FileName;

                    var json = JsonConvert.SerializeObject(
                        editor.Container,
                        new JsonSerializerSettings()
                        {
                            Formatting = Formatting.Indented,
                            TypeNameHandling = TypeNameHandling.Objects,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                        });

                    System.IO.File.WriteAllText(path, json, Encoding.UTF8);
                }
            });

            editor.ExitCommand = new DelegateCommand(() =>
            {
                Close();
            });

            editor.ClearCommand = new DelegateCommand(() =>
            {
                editor.Container.Clear();
                editor.Container.Invalidate();
            });

            editor.ToolNoneCommand = new DelegateCommand(() => editor.CurrentTool = Tool.None);
            editor.ToolLineCommand = new DelegateCommand(() => editor.CurrentTool = Tool.Line);
            editor.ToolRectangleCommand = new DelegateCommand(() => editor.CurrentTool = Tool.Rectangle);
            editor.ToolEllipseCommand = new DelegateCommand(() => editor.CurrentTool = Tool.Ellipse);
            editor.ToolBezierCommand = new DelegateCommand(() => editor.CurrentTool = Tool.Bezier);
            editor.ToolQBezierCommand = new DelegateCommand(() => editor.CurrentTool = Tool.QBezier);

            optionsDrawPoints.Click += (s, e) => editor.Container.Invalidate();

            layersAdd.Click += (s, e) =>
            {
                var layer = XLayer.Create("New");
                editor.Container.Layers.Add(layer);

                var element = WpfElement.Create(
                    editor.Renderer, 
                    layer,
                    editor.Container.Width,
                    editor.Container.Height);
                layers.Add(layer, element);

                canvasLayers.Children.Add(element);
            };

            layersRemove.Click += (s, e) =>
            {
                var layer = editor.Container.CurrentLayer;
                editor.Container.Layers.Remove(layer);

                var element = layers[layer];
                layers.Remove(layer);

                canvasLayers.Children.Remove(element);

                editor.Container.CurrentLayer = editor.Container.Layers.FirstOrDefault();
                editor.Container.Invalidate();
            };

            stylesAdd.Click += (s, e) =>
            {
                editor.Container.Styles.Add(
                    XStyle.Create("New", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            };

            stylesRemove.Click += (s, e) =>
            {
                editor.Container.Styles.Remove(editor.Container.CurrentStyle);
                editor.Container.CurrentStyle = editor.Container.Styles.FirstOrDefault();
            };

            shapesRemove.Click += (s, e) =>
            {
                editor.Container.CurrentLayer.Shapes.Remove(editor.Container.CurrentShape);
                editor.Container.CurrentShape = editor.Container.CurrentLayer.Shapes.FirstOrDefault();
                editor.Container.Invalidate();
            };

            DataContext = editor;

            //Action<IContainer> groupTest = (c) =>
            //{
            //    var g = XGroup.Create("g");
            //    g.Shapes.Add(XLine.Create(30, 30, 30, 60, c.CurrentStyle, c.PointShape));
            //    g.Shapes.Add(XLine.Create(60, 30, 60, 60, c.CurrentStyle, c.PointShape));
            //    g.Shapes.Add(XLine.Create(30, 30, 60, 30, c.CurrentStyle, c.PointShape));
            //    g.Shapes.Add(XLine.Create(30, 60, 60, 60, c.CurrentStyle, c.PointShape));
            //    c.CurrentLayer.Shapes.Add(g);
            //    c.Invalidate();
            //};
            //groupTest(editor.Container);
        }
    }
}
