using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Core2D.UI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="MainControl"/> xaml.
    /// </summary>
    public class MainControl : UserControl
    {
#if false
        private SplitView _splitView;
        private Button _pinButton;
        private TextBlock _pinTextBlock;
        private LayoutTransformControl _layoutTransform;
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="MainControl"/> class.
        /// </summary>
        public MainControl()
        {
            InitializeComponent();
#if false
            _splitView = this.FindControl<SplitView>("SplitView");
            _pinButton = this.FindControl<Button>("PinButton");
            _pinTextBlock =  this.FindControl<TextBlock>("PinTextBlock");
            _layoutTransform = this.FindControl<LayoutTransformControl>("LayoutTransform");

            _splitView.GetObservable(SplitView.IsPaneOpenProperty).Subscribe((isPaneOpen) =>
            {
                if (_layoutTransform.LayoutTransform is RotateTransform rotateTransform)
                {
                    rotateTransform.Angle = isPaneOpen ? 0 : 90;
                }
            });

            _splitView.GetObservable(SplitView.DisplayModeProperty).Subscribe((displayMode) =>
            {
                if (_splitView.DisplayMode == SplitViewDisplayMode.Inline)
                {
                    _pinButton.Content = _splitView.Resources["PinIcon"];
                }
                else if (_splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    _pinButton.Content = _splitView.Resources["PinOffIcon"];
                }
            });

            _pinButton.Click += (sender, e) =>
            {
                if (_splitView.DisplayMode == SplitViewDisplayMode.Inline)
                {
                    _splitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                    _splitView.IsPaneOpen = false;
                }
                else if (_splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    _splitView.DisplayMode = SplitViewDisplayMode.Inline;
                    _splitView.IsPaneOpen = true;
                }
            };

            _pinTextBlock.PointerPressed += (sender, e) =>
            {
                if (_splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    _splitView.IsPaneOpen = !_splitView.IsPaneOpen;
                }
            };
#endif
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
