// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private bool _isLoaded = false;

        /// <summary>
        /// 
        /// </summary>
        public ICommand EvalCommand { get; set; }

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
            SaveCommand = new DelegateCommand(() => Save());
            ImportCommand = new DelegateCommand(() => Import());
            ExportCommand = new DelegateCommand(() => Export());

            textEditor.Options.ConvertTabsToSpaces = true;
            textEditor.Options.ShowColumnRuler = true;

            Loaded += (s, e) =>
            {
                if (_isLoaded)
                    return;
                else
                    _isLoaded = true;

                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                context.PropertyChanged += ObserveContext;
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObserveContext(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentScript")
            {
                var context = DataContext as EditorContext;
                if (context == null)
                    return;

                var value = context.CurrentScript;

                if (value != null && value is ScriptFile)
                {
                    Open((value as ScriptFile).Path);
                }
                else
                {
                    textEditor.Text = "";
                }
            }
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
        /// <param name="path"></param>
        private void Open(string path)
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            try
            {
                var code = System.IO.File.ReadAllText(path);
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

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var value = context.CurrentScript;

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
        private void Export()
        {
            var context = DataContext as EditorContext;
            if (context == null)
                return;

            var value = context.CurrentScript;

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
