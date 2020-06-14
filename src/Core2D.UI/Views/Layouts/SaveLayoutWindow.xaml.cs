using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Layouts
{
    /// <summary>
    /// Interaction logic for <see cref="SaveLayoutWindow"/> xaml.
    /// </summary>
    public class SaveLayoutWindow : Window
    {
        /// <summary>
        /// Gets or sets string property.
        /// </summary>
        public string String { get; set; }

        /// <summary>
        /// Gets or sets result property.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveLayoutWindow"/> class.
        /// </summary>
        public SaveLayoutWindow()
        {
            this.InitializeComponent();
            this.AttachDevTools();
            // FIXME: App.Selector.EnableThemes(this);
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Performs OK button action.
        /// </summary>
        public void OnOK()
        {
            Result = true;
            Close();
        }

        /// <summary>
        /// Performs Cancel button action.
        /// </summary>
        public void OnCancel()
        {
            Result = false;
            Close();
        }
    }
}
