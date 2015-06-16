// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class AvalonEditBehaviour : Behavior<TextEditor>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register(
            "Code", 
            typeof(string), 
            typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(
                default(string), 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                PropertyChangedCallback));

        /// <summary>
        /// 
        /// </summary>
        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor != null)
            {
                if (textEditor.Document != null)
                    Code = textEditor.Document.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="dependencyPropertyChangedEventArgs"></param>
        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehaviour;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject as TextEditor;
                if (editor.Document != null)
                {
                    if (dependencyPropertyChangedEventArgs.NewValue != null)
                    {
                        var caretOffset = editor.CaretOffset;
                        var text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                        editor.Document.Text = text;
                        int length = text.Length;
                        editor.CaretOffset = caretOffset >= length ? length : caretOffset;
                    }
                    else
                    {
                        editor.Document.Text = "";
                        editor.CaretOffset = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class CodeControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public CodeControl()
        {
            InitializeComponent();

            InitializeTextEditors();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeTextEditors()
        {
            codeEditor.Options.ConvertTabsToSpaces = true;
            codeEditor.Options.ShowColumnRuler = true;

            dataEditor.Options.ConvertTabsToSpaces = true;
            dataEditor.Options.ShowColumnRuler = true;
        }
    }
}
