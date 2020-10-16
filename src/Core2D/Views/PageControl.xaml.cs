using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class PageControl : UserControl
    {
        private ScrollViewer _scrollViewer;
        private ZoomBorder _zoomBorder;
        private PresenterControl _presenterControlData;
        private PresenterControl _presenterControlTemplate;
        private PresenterControl _presenterControlEditor;
        //private TextBox _textEditor;

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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
