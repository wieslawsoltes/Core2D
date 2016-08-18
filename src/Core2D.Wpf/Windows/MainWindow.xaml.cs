// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using Core2D.Editor.Interfaces;

namespace Core2D.Wpf.Windows
{
    /// <summary>
    /// Interaction logic for <see cref="MainWindow"/> xaml.
    /// </summary>
    public partial class MainWindow : Window, IMainView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public object Context { get; }

        /// <inheritdoc/>
        public void Present()
        {
            ShowDialog();
        }

        /// <inheritdoc/>
        public void Destroy()
        {
            Close();
        }
    }
}
