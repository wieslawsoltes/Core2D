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
        //private TextBox _textEditor;

        public PageView()
        {
            InitializeComponent();

            _scrollViewer = this.FindControl<ScrollViewer>("scrollViewer");
            _zoomBorder = this.FindControl<ZoomBorder>("zoomBorder");
            _presenterViewData = this.FindControl<PresenterView>("presenterViewData");
            _presenterViewTemplate = this.FindControl<PresenterView>("presenterViewTemplate");
            _presenterViewEditor = this.FindControl<PresenterView>("presenterViewEditor");
            //_textEditor = this.FindControl<TextBox>("textEditor");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
