using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class PageView : UserControl
    {
        private ScrollViewer _scrollViewer;
        private ZoomBorder _zoomBorder;
        private PresenterView _presenterViewData;
        private PresenterView _presenterViewTemplate;
        private PresenterView _presenterViewEditor;
        private TextBox _textEditor;

        public PageView()
        {
            InitializeComponent();

            _scrollViewer = this.FindControl<ScrollViewer>("PageScrollViewer");
            _zoomBorder = this.FindControl<ZoomBorder>("PageZoomBorder");
            _presenterViewData = this.FindControl<PresenterView>("PresenterViewData");
            _presenterViewTemplate = this.FindControl<PresenterView>("PresenterViewTemplate");
            _presenterViewEditor = this.FindControl<PresenterView>("PresenterViewEditor");
            _textEditor = this.FindControl<TextBox>("EditorTextBox");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
