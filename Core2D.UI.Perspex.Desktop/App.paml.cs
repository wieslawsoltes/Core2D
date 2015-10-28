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

    /// <summary>
    /// Object model Xaml value converters.
    /// </summary>
    public static class CoreConverters
    {
        /// <summary>
        /// Convert ArgbColor to Color.
        /// </summary>
        /// <param name="color">The ArgbColor object.</param>
        /// <returns>The converted Color object.</returns>
        public static Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(
                (byte)color.A,
                (byte)color.R,
                (byte)color.G,
                (byte)color.B);
        }

        /// <summary>
        /// Convert ArgbColor to SolidColorBrush.
        /// </summary>
        /// <param name="color">The ArgbColor object.</param>
        /// <returns>The converted SolidColorBrush object.</returns>
        public static SolidColorBrush ToSolidBrush(ArgbColor color)
        {
            return new SolidColorBrush(ToColor(color));
        }

        /// <summary>
        /// Convert ArgbColor to SolidColorBrush.
        /// </summary>
        public static readonly IValueConverter ArgbColorToBrush =
            new FuncValueConverter<ArgbColor, SolidColorBrush>(x => ToSolidBrush(x));

        /// <summary>
        /// 
        /// </summary>
        public static readonly IValueConverter IntToInt =
            new FuncValueConverter<int, int>(x => x);

        /// <summary>
        /// Convert ArgbColor individual properties A, R, G, B to SolidColorBrush.
        /// </summary>
        public static readonly IMultiValueConverter ArgbColorsToBrush = 
            new FuncMultiValueConverter<int, SolidColorBrush>(x =>
            {
                var values = x.ToList();
                if (values.Count() == 4)
                {
                    var color = Color.FromArgb(
                        (byte)values[0],
                        (byte)values[1],
                        (byte)values[2],
                        (byte)values[3]);
                    return new SolidColorBrush(color);
                }
                return null;
            });
    }
}
