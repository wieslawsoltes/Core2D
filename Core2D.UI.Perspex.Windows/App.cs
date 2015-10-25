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

namespace Core2D.UI.Perspex.Windows
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
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var app = new App();
                var window = new MainWindow();
                window.Show();
                app.Run(window);
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        static void Print(Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.GetType().ToString());
            System.Diagnostics.Debug.Print(ex.Message);
            System.Diagnostics.Debug.Print(ex.StackTrace);

            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.Print("Inner exception:");
                Print(ex.InnerException);
            }
        }
    }
}
