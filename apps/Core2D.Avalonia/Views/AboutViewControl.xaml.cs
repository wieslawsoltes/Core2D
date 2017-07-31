// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="AboutViewControl"/> xaml.
    /// </summary>
    public class AboutViewControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewControl"/> class.
        /// </summary>
        public AboutViewControl()
        {
            this.InitializeComponent();
            this.InitializeText();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Initializes text controls.
        /// </summary>
        private void InitializeText()
        {
            var app = (App.Current as App);
            this.Find<TextBlock>("textTitle").Text = "Core2D";
            this.Find<TextBlock>("textVersion").Text = $"{app.GetType().GetTypeInfo().Assembly.GetName().Version}";
            this.Find<TextBlock>("textAbout").Text = "A multi-platform data driven 2D diagram editor.";
            this.Find<TextBlock>("textCopyright").Text = "Copyright (c) Wiesław Šoltés. All rights reserved.";
            this.Find<TextBlock>("textLicense").Text = "Licensed under the MIT license. See LICENSE file in the project root for full license information.";
            this.Find<TextBlock>("textOperatingSystem").Text = $"{app.RuntimeInfo.OperatingSystem}";
            this.Find<TextBlock>("textIsDesktop").Text = $"{app.RuntimeInfo.IsDesktop}";
            this.Find<TextBlock>("textIsMobile").Text = $"{app.RuntimeInfo.IsMobile}";
            this.Find<TextBlock>("textIsCoreClr").Text = $"{app.RuntimeInfo.IsCoreClr}";
            this.Find<TextBlock>("textIsMono").Text = $"{app.RuntimeInfo.IsMono}";
            this.Find<TextBlock>("textIsDotNetFramework").Text = $"{app.RuntimeInfo.IsDotNetFramework}";
            this.Find<TextBlock>("textIsUnix").Text = $"{app.RuntimeInfo.IsUnix}";
            this.Find<TextBlock>("textWindowingSubsystem").Text = app.WindowingSubsystemName;
            this.Find<TextBlock>("textRenderingSubsystem").Text = app.RenderingSubsystemName;
        }
    }
}
