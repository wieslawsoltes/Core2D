// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perspex;
using Perspex.Controls;
using Perspex.Diagnostics;
using Perspex.Themes.Default;

namespace TestPerspex
{
    /// <summary>
    /// 
    /// </summary>
    class App : Application
    {
        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            RegisterServices();
            InitializeSubsystems((int)Environment.OSVersion.Platform);
            Styles = new DefaultTheme();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public static void AttachDevTools(Window window)
        {
#if DEBUG
            DevTools.Attach(window);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var app = new App();
            var window = new MainWindow();
            window.Show();
            app.Run(window);
        }
    }
}
