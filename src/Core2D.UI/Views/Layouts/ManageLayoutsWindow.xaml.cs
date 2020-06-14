using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Views.Layouts
{
    /// <summary>
    /// Interaction logic for <see cref="ManageLayoutsWindow"/> xaml.
    /// </summary>
    public class ManageLayoutsWindow : Window
    {
        /// <summary>
        /// Gets or sets result property.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageLayoutsWindow"/> class.
        /// </summary>
        public ManageLayoutsWindow()
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

        /// <summary>
        /// Performs Rename button action.
        /// </summary>
        public void OnRename()
        {
            // TODO:
        }

        /// <summary>
        /// Performs Delete button action.
        /// </summary>
        public void OnDelete()
        {
            // TODO:
        }
    }
}
