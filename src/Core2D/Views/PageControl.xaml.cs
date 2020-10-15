using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    /// <summary>
    /// Interaction logic for <see cref="PageControl"/> xaml.
    /// </summary>
    public class PageControl : UserControl
    {
        private ScrollViewer _scrollViewer;
        private ZoomBorder _zoomBorder;
        private PresenterControl _presenterControlData;
        private PresenterControl _presenterControlTemplate;
        private PresenterControl _presenterControlEditor;
        //private TextBox _textEditor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageControl"/> class.
        /// </summary>
        public PageControl()
        {
            InitializeComponent();

            _scrollViewer = this.FindControl<ScrollViewer>("scrollViewer");
            _zoomBorder = this.FindControl<ZoomBorder>("zoomBorder");
            _presenterControlData = this.FindControl<PresenterControl>("presenterControlData");
            _presenterControlTemplate = this.FindControl<PresenterControl>("presenterControlTemplate");
            _presenterControlEditor = this.FindControl<PresenterControl>("presenterControlEditor");
            //_textEditor = this.FindControl<TextBox>("textEditor");
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
