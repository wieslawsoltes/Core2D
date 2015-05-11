// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Test.Compressors;
using Test2d;
using TestEDITOR;
using TestEMF;

namespace Test.Windows
{
    internal class TextClipboard : ITextClipboard
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        public string GetText()
        {
            return Clipboard.GetText(TextDataFormat.UnicodeText);
        }

        public bool ContainsText()
        {
            return Clipboard.ContainsText(TextDataFormat.UnicodeText);
        }
    }

    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeContext();
        }

        private void InitializeContext()
        {
            var context = new EditorContext();
            context.Execute = (action) => Dispatcher.Invoke(action);
            context.Initialize(
                this,
                WpfRenderer.Create(),
                new TextClipboard(),
                new LZ4CodecCompressor());
            context.InitializeSctipts();
            context.InitializeSimulation();
            context.Editor.Renderer.DrawShapeState = ShapeState.Visible;
            context.Editor.GetImagePath = () => Image();

            context.Commands.OpenCommand = new DelegateCommand(
                () =>
                {
                    Open();
                },
                () => context.IsEditMode());

            context.Commands.SaveAsCommand = new DelegateCommand(
                () =>
                {
                    SaveAs();
                },
                () => context.IsEditMode());

            context.Commands.ExportCommand = new DelegateCommand<object>(
                (item) =>
                {
                    Export(item);
                },
                (item) => context.IsEditMode());

            context.Commands.CopyAsEmfCommand = new DelegateCommand(
                () =>
                {
                    Emf.PutOnClipboard(context.Editor.Container);
                },
                () => context.IsEditMode());

            context.Commands.EvalCommand = new DelegateCommand(
                () =>
                {
                    Eval();
                },
                () => context.IsEditMode());
            
            context.Commands.LayersWindowCommand = new DelegateCommand(
                () =>
                {
                    (new LayersWindow() { Owner = this, DataContext = context }).Show();
                },
                () => true);

            context.Commands.StyleWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StyleWindow() { Owner = this, DataContext = context }).Show();
                },
                () => true);

            context.Commands.StylesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StylesWindow() { Owner = this, DataContext = context }).Show();
                },
                () => true);

            context.Commands.ShapesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ShapesWindow() { Owner = this, DataContext = context }).Show();
                },
                () => true);

            context.Commands.ContainerWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ContainerWindow() { Owner = this, DataContext = context }).Show();
                },
                () => true);

            PropertiesWindow pw = default(PropertiesWindow);

            context.Commands.PropertiesWindowCommand = new DelegateCommand(
                () =>
                {
                    if (pw == null)
                    {
                        pw = new PropertiesWindow() { Owner = this, DataContext = context };
                        pw.Unloaded += (_s, _e) => pw = default(PropertiesWindow);
                    }
                    pw.Show();
                },
                () => true);

            context.Editor.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == "IsContextMenu")
                    {
                        if (context.Editor.IsContextMenu)
                        {
                            context.Editor.IsContextMenu = false;
                            
                            if (pw == null)
                            {
                                pw = new PropertiesWindow() { Owner = this, DataContext = context };
                                pw.Unloaded += (_s, _e) => pw = default(PropertiesWindow);
                            }
                            pw.Show();
                        }
                    }
                };

            containerControl.AllowDrop = true;

            containerControl.DragEnter += (s, e) =>
            {
                if (!e.Data.GetDataPresent("Group") || s == e.Source)
                {
                    e.Effects = DragDropEffects.None;
                }
            };

            containerControl.Drop += 
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        try
                        {
                            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            if (files != null && files.Length >= 1)
                            {
                                foreach (var path in files)
                                {
                                    if (string.IsNullOrEmpty(path))
                                        continue;

                                    string ext = System.IO.Path.GetExtension(path);
                                    if (string.Compare(ext, ".project", true, CultureInfo.InvariantCulture) == 0)
                                    {
                                        context.Open(path);
                                        e.Handled = true;
                                    }
                                    else if (string.Compare(ext, ".cs", true, CultureInfo.InvariantCulture) == 0)
                                    {
                                        context.Eval(path);
                                        e.Handled = true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print(ex.Message);
                            System.Diagnostics.Debug.Print(ex.StackTrace);
                        }
                    }

                    if (e.Data.GetDataPresent("Group"))
                    {
                        try
                        {
                            var group = e.Data.GetData("Group") as XGroup;
                            if (group != null)
                            {
                                var p = e.GetPosition(containerControl);
                                context.Drop(group, p.X, p.Y);
                                e.Handled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Print(ex.Message);
                            System.Diagnostics.Debug.Print(ex.StackTrace);
                        }
                    }
                };

            Unloaded += (s, e) => DeInitializeContext();

            DataContext = context;
        }

        private void DeInitializeContext()
        {
            (DataContext as EditorContext).Dispose();
        }

        public void Eval()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "C# (*.cs)|*.cs|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = "",
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var path in dlg.FileNames)
                {
                    (DataContext as EditorContext).Eval(path);
                }
            }
        }

        public void Open()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                (DataContext as EditorContext).Open(dlg.FileName);
            }
        }

        public void SaveAs()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = (DataContext as EditorContext).Editor.Project.Name
            };

            if (dlg.ShowDialog() == true)
            {
                (DataContext as EditorContext).Save(dlg.FileName);
            }
        }

        public void Export(object item)
        {
            string name = string.Empty;

            if (item is Container)
            {
                name = (item as Container).Name;
            }
            else if (item is Document)
            {
                name = (item as Document).Name;
            }
            else if (item is Project)
            {
                name = (item as Project).Name;
            }
            else if (item is EditorContext)
            {
                var editor = (item as EditorContext).Editor;
                name = editor.Project.Name;
                item = editor.Project;
            }
            else if (item == null)
            {
                var editor = (DataContext as EditorContext).Editor;
                name = editor.Project.Name;
                item = editor.Project;
            }

            var dlg = new SaveFileDialog()
            {
                Filter = "Pdf (*.pdf)|*.pdf|Emf (*.emf)|*.emf|Dxf AutoCAD 2000 (*.dxf)|*.dxf|Dxf R10 (*.dxf)|*.dxf|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog() == true)
            {
                switch (dlg.FilterIndex) 
                {
                    case 1:
                        (DataContext as EditorContext).ExportAsPdf(dlg.FileName, item);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    case 2:
                        (DataContext as EditorContext).ExportAsEmf(dlg.FileName);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    case 3:
                        (DataContext as EditorContext).ExportAsDxf(dlg.FileName, Dxf.DxfAcadVer.AC1015);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    case 4:
                        (DataContext as EditorContext).ExportAsDxf(dlg.FileName, Dxf.DxfAcadVer.AC1006);
                        System.Diagnostics.Process.Start(dlg.FileName);
                        break;
                    default:
                        break;
                }
            }
        }

        public string Image()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            return null;
        }
    }
}
