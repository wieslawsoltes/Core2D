using System;
using Avalonia;
using Avalonia.Styling;
using DAC = Dock.Avalonia.Controls;

namespace Core2D.UI.Dock.Windows
{
    /// <summary>
    /// Themed host window.
    /// </summary>
    public class ThemedHostWindow : DAC.HostWindow, IStyleable
    {
        Type IStyleable.StyleKey => typeof(DAC.HostWindow);

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemedHostWindow"/> class.
        /// </summary>
        public ThemedHostWindow()
        {
            this.AttachDevTools();
            App.Selector.EnableThemes(this);
        }
    }
}
