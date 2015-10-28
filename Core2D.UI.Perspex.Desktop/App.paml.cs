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
using Perspex.Markup.Xaml;
using Perspex.Themes.Default;
using Perspex.Markup;
using Perspex.Media;

namespace Core2D.UI.Perspex.Desktop
{
    /// <summary>
    /// Encapsulates a Core2D Prespex application.
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class.
        /// </summary>
        public App()
        {
            RegisterServices();
            InitializeSubsystems((int)Environment.OSVersion.Platform);
            Styles = new DefaultTheme();
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            var loader = new PerspexXamlLoader();
            loader.Load(typeof(App), this);
        }

        /// <summary>
        /// Attach development tool in debug mode.
        /// </summary>
        /// <param name="window"></param>
        public static void AttachDevTools(Window window)
        {
#if DEBUG
            DevTools.Attach(window);
#endif
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                var app = new App();
                var window = new Windows.MainWindow();
                window.Show();
                app.Run(window);
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        /// <summary>
        /// Print exception details.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        private static void Print(Exception ex)
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
