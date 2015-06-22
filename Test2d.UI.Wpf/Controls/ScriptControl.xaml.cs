// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test2d;

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ScriptControl : UserControl
    {
        private bool _enableAutoSave = false;

        /// <summary>
        /// 
        /// </summary>
        public ICommand EvalCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand NewCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ImportCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ExportCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ScriptControl()
        {
            InitializeComponent();

            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Initialize()
        {
            EvalCommand = new DelegateCommand(() => Eval());
            NewCommand = new DelegateCommand(() => New());
            SaveCommand = new DelegateCommand(() => Save(tree.SelectedItem));
            ImportCommand = new DelegateCommand(() => Import());
            ExportCommand = new DelegateCommand(() => Export(tree.SelectedItem));

            textEditor.Options.ConvertTabsToSpaces = true;
            textEditor.Options.ShowColumnRuler = true;

            tree.SelectedItemChanged +=
                (s, e) =>
                {
                    if (_enableAutoSave)
                    {
                        Save(e.OldValue);
                    }

                    Open(e.NewValue);
                };
        }

        /// <summary>
        /// 
        /// </summary>
        private void Eval()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            try
            {
                var code = textEditor.Text;
                context.Eval(code, context);
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void New()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var directory = tree.SelectedItem as ScriptDirectory;
            if (directory == null || !(directory is ScriptDirectory))
                return;

            try
            {
                var template = "New";
                int max = 4096;
                int i = 0;
                bool success = false;

                while (success == false && i < max)
                {
                    var name = template + i;
                    var path = System.IO.Path.Combine(directory.Path, name + ".cs");

                    if (!System.IO.File.Exists(path))
                    {
                        success = true;

                        var script = new ScriptFile()
                        {
                            Name = name,
                            Path = path
                        };

                        System.IO.File.CreateText(script.Path);
                        directory.Scripts = directory.Scripts.Add(script);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                if (context.Editor.Log != null)
                {
                    context.Editor.Log.LogError("{0}{1}{2}",
                        ex.Message,
                        Environment.NewLine,
                        ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void Open(object value)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            if (value != null && value is ScriptFile)
            {
                var script = value as ScriptFile;
                try
                {
                    var code = System.IO.File.ReadAllText(script.Path);
                    textEditor.Text = code;
                }
                catch (Exception ex)
                {
                    if (context.Editor.Log != null)
                    {
                        context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
            else
            {
                textEditor.Text = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void Save(object value)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            if (value != null && value is ScriptFile)
            {
                var script = value as ScriptFile;
                try
                {
                    var code = textEditor.Text;
                    System.IO.File.WriteAllText(script.Path, code);
                }
                catch (Exception ex)
                {
                    if (context.Editor.Log != null)
                    {
                        context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Import()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var dlg = new OpenFileDialog()
            {
                Filter = "C# (*.cs)|*.cs|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var code = System.IO.File.ReadAllText(dlg.FileName);
                    textEditor.Text = code;
                }
                catch (Exception ex)
                {
                    if (context.Editor.Log != null)
                    {
                        context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void Export(object value)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            string name = "script";

            if (value != null && value is ScriptFile)
            {
                name = (value as ScriptFile).Name;
            }

            var dlg = new SaveFileDialog()
            {
                Filter = "C# (*.cs)|*.cs|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var code = textEditor.Text;
                    System.IO.File.WriteAllText(dlg.FileName, code);
                }
                catch (Exception ex)
                {
                    if (context.Editor.Log != null)
                    {
                        context.Editor.Log.LogError("{0}{1}{2}",
                            ex.Message,
                            Environment.NewLine,
                            ex.StackTrace);
                    }
                }
            }
        }
    }
}
