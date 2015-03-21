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

            double width = 800;
            double height = 600;

            var container = XContainer.Create(width, height);
            var editor = ContainerEditor.Create(container);

            var renderer = new WpfRenderer();
            var layers = new Dictionary<ILayer, WpfElement>();

            foreach (var layer in container.Layers)
            {
                var element = WpfElement.Create(renderer, layer, width, height);
                layers.Add(layer, element);
                canvasLayers.Children.Add(element);
            }

            var workingElement = WpfElement.Create(renderer, container.WorkingLayer, width, height);
            canvasWorking.Children.Add(workingElement);

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
                container.Clear();
                container.Invalidate();
            };

            toolNone.Click += (s, e) => editor.CurrentTool = Tool.None;
            toolLine.Click += (s, e) => editor.CurrentTool = Tool.Line;
            toolRectangle.Click += (s, e) => editor.CurrentTool = Tool.Rectangle;
            toolEllipse.Click += (s, e) => editor.CurrentTool = Tool.Ellipse;
            toolBezier.Click += (s, e) => editor.CurrentTool = Tool.Bezier;

            layersAdd.Click += (s, e) =>
            {
                var layer = XLayer.Create("New");
                container.Layers.Add(layer);
                var element = WpfElement.Create(renderer, layer, width, height);
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
    }
}
