// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
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
    public partial class ScriptsControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public ICommand EvalCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ScriptsControl()
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

            tree.SelectedItemChanged +=
                (s, e) =>
                {
                    var context = DataContext as EditorContext;
                    if (context == null)
                        return;

                    var value = tree.SelectedItem;
                    if (value != null && value is ScriptFile)
                    {
                        context.CurrentScript = value as ScriptFile;
                    }
                    else
                    {
                        context.CurrentScript = null;
                    }
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
                var value = tree.SelectedItem;
                if (value != null && value is ScriptFile)
                {
                    var script = value as ScriptFile;
                    var code = System.IO.File.ReadAllText(script.Path);
                    context.Eval(code, context);
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
    }
}
