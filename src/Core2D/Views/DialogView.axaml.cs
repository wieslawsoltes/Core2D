using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Views
{
    public class DialogView : UserControl
    {
        public static readonly StyledProperty<bool> IsOverlayVisibleProperty =
            AvaloniaProperty.Register<DialogView, bool>(nameof(IsOverlayVisible), true);

        public static readonly StyledProperty<bool> IsTitleBarVisibleProperty =
            AvaloniaProperty.Register<DialogView, bool>(nameof(IsTitleBarVisible), true);

        public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
            AvaloniaProperty.Register<DialogView, bool>(nameof(IsCloseButtonVisible), true);

        public bool IsOverlayVisible
        {
            get => GetValue(IsOverlayVisibleProperty);
            set => SetValue(IsOverlayVisibleProperty, value);
        }

        public bool IsTitleBarVisible
        {
            get => GetValue(IsTitleBarVisibleProperty);
            set => SetValue(IsTitleBarVisibleProperty, value);
        }

        public bool IsCloseButtonVisible
        {
            get => GetValue(IsCloseButtonVisibleProperty);
            set => SetValue(IsCloseButtonVisibleProperty, value);
        }

        public DialogView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
