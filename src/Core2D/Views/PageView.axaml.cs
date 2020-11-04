using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class PageView : UserControl
    {
        private ScrollViewer _scrollViewer;
        private ZoomBorder _zoomBorder;
        private PresenterView _presenterControlData;
        private PresenterView _presenterControlTemplate;
        private PresenterView _presenterControlEditor;
        //private TextBox _textEditor;

        public PageView()
        {
            InitializeComponent();

            _scrollViewer = this.FindControl<ScrollViewer>("scrollViewer");
            _zoomBorder = this.FindControl<ZoomBorder>("zoomBorder");
            _presenterControlData = this.FindControl<PresenterView>("presenterControlData");
            _presenterControlTemplate = this.FindControl<PresenterView>("presenterControlTemplate");
            _presenterControlEditor = this.FindControl<PresenterView>("presenterControlEditor");
            //_textEditor = this.FindControl<TextBox>("textEditor");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
