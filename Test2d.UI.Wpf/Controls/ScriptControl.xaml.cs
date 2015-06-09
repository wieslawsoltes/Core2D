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

            NewCommand = new DelegateCommand(() => New());
            SaveCommand = new DelegateCommand(() => Save(tree.SelectedItem));
            ImportCommand = new DelegateCommand(() => Import());
            ExportCommand = new DelegateCommand(() => Export(tree.SelectedItem));

            editor.Options.ConvertTabsToSpaces = true;
            editor.Options.ShowColumnRuler = true;

            tree.SelectedItemChanged +=
                (s, e) =>
                {
                    Save(e.OldValue);
                    Open(e.NewValue);
                };
        }

        private void New()
        {
            if (tree.SelectedItem is ScriptDirectory)
            {
                var directory = tree.SelectedItem as ScriptDirectory;

                var template = "New";
                try
                {
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
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private void Open(object value)
        {
            if (value != null && value is ScriptFile)
            {
                var script = value as ScriptFile;
                try
                {
                    var code = System.IO.File.ReadAllText(script.Path);
                    editor.Text = code;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
            else
            {
                editor.Text = "";
            }
        }

        private void Save(object value)
        {
            if (value != null && value is ScriptFile)
            {
                var script = value as ScriptFile;
                try
                {
                    if (editor.CanUndo)
                    {
                        var code = editor.Text;
                        System.IO.File.WriteAllText(script.Path, code);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private void Import()
        {
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
                    editor.Text = code;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private void Export(object value)
        {
            string name = "script";

            if (value != null && value is ScriptFile)
            {
                name = (value as ScriptFile).Name;
            }

            var dlg = new OpenFileDialog()
            {
                Filter = "C# (*.cs)|*.cs|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var code = editor.Text;
                    System.IO.File.WriteAllText(dlg.FileName, code);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }
    }
}
