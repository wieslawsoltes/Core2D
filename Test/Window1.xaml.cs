// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
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
using Test.Util;

namespace Test
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            var editor = ContainerEditor.Create(XContainer.Create(800, 600), ShapeRenderer.Create());
            var observer = new StyleObserver(editor);
            
            editor.NewCommand = new DelegateCommand(() =>
            {
                var container = XContainer.Create(800, 600);
                editor.Load(container);
                observer = new StyleObserver(editor);
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
                    var container = ContainerSerializer.Deserialize(json);
                    editor.Load(container);
                    observer = new StyleObserver(editor);
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
                    var json = ContainerSerializer.Serialize(editor.Container);
                    System.IO.File.WriteAllText(path, json, Encoding.UTF8);
                }
            });

            editor.ExitCommand = new DelegateCommand(() => Close());

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
            editor.ToolTextCommand = new DelegateCommand(() => editor.CurrentTool = Tool.Text);

            editor.DefaultIsFilledCommand = new DelegateCommand(() => editor.DefaultIsFilled = !editor.DefaultIsFilled);
            editor.SnapToGridCommand = new DelegateCommand(() => editor.SnapToGrid = !editor.SnapToGrid);
            editor.DrawPointsCommand = new DelegateCommand(() => 
            {
                editor.Renderer.DrawPoints = !editor.Renderer.DrawPoints;
                editor.Container.Invalidate();
            });

            editor.AddLayerCommand = new DelegateCommand(() =>
            {
                editor.Container.Layers.Add(XLayer.Create("New"));
            });

            editor.RemoveLayerCommand = new DelegateCommand(() =>
            {
                editor.RemoveCurrentLayer();
            });

            editor.AddStyleCommand = new DelegateCommand(() =>
            {
                editor.Container.Styles.Add(XStyle.Create("New", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            });

            editor.RemoveStyleCommand = new DelegateCommand(() =>
            {
                editor.RemoveCurrentStyle();
            });

            editor.RemoveShapeCommand = new DelegateCommand(() =>
            {
                editor.RemoveCurrentShape();
            });

            editor.GroupSelectedCommand = new DelegateCommand(() => editor.GroupSelected());

            editor.GroupCurrentLayerCommand = new DelegateCommand(() => editor.GroupCurrentLayer());

            canvas.PreviewMouseLeftButtonDown += (s, e) =>
            {
                canvas.Focus();

                if (editor.IsLeftAvailable())
                {
                    var p = e.GetPosition(canvas);
                    editor.Left(p.X, p.Y);
                }
            };

            canvas.PreviewMouseRightButtonDown += (s, e) =>
            {
                canvas.Focus();

                if (editor.IsRightAvailable())
                {
                    var p = e.GetPosition(canvas);
                    editor.Right(p.X, p.Y);
                }
            };

            canvas.PreviewMouseMove += (s, e) =>
            {
                canvas.Focus();

                if (editor.IsMoveAvailable())
                {
                    var p = e.GetPosition(canvas);
                    editor.Move(p.X, p.Y);
                }
            };

            DataContext = editor;

            Loaded += (s, e) => canvas.Focus();
        }
    }
}
