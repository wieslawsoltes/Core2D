// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Core2D.ViewModels.Containers;

namespace Core2D.UI.Avalonia.Views
{
    public partial class RenderView : UserControl
    {
        public RenderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                var zoomBorder = this.FindControl<ZoomBorder>("zoomBorder");
                vm.Reset = () => zoomBorder.Reset();
                vm.AutoFit = () => zoomBorder.AutoFit();
                vm.StretchNone = () => zoomBorder.Stretch = PanAndZoom.StretchMode.None;
                vm.StretchFill = () => zoomBorder.Stretch = PanAndZoom.StretchMode.Fill;
                vm.StretchUniform = () => zoomBorder.Stretch = PanAndZoom.StretchMode.Uniform;
                vm.StretchUniformToFill = () => zoomBorder.Stretch = PanAndZoom.StretchMode.UniformToFill;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                vm.Reset = null;
                vm.AutoFit = null;
                vm.StretchNone = null;
                vm.StretchFill = null;
                vm.StretchUniform = null;
                vm.StretchUniformToFill = null;
            }
        }
    }
}
