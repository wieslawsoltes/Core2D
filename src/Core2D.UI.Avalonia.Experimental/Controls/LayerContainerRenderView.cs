// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Core2D.UI.Avalonia.Renderers;
using Core2D.ViewModels.Containers;

namespace Core2D.UI.Avalonia.Controls
{
    public class LayerContainerRenderView : Canvas
    {
        private bool _drawWorking = false;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
                if (md != null)
                {
                    vm.Capture = () =>
                    {
                        if (md.Captured == null)
                        {
                            md.Capture(this);
                        }
                    };
                    vm.Release = () =>
                    {
                        if (md.Captured != null)
                        {
                            md.Capture(null);
                        }
                    };
                    vm.Invalidate = () => this.InvalidateVisual();
                }
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                vm.Capture = null;
                vm.Release = null;
                vm.Invalidate = null;
            }
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            _drawWorking = true;
            this.InvalidateVisual();
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            _drawWorking = false;
            this.InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                if (vm.CurrentContainer.WorkBackground != null)
                {
                    var color = AvaloniaBrushCache.FromDrawColor(vm.CurrentContainer.WorkBackground);
                    var brush = new SolidColorBrush(color);
                    context.FillRectangle(brush, new Rect(0, 0, Bounds.Width, Bounds.Height));
                }

                vm.Presenter.DrawContainer(context, vm.CurrentContainer, vm.Renderer, 0.0, 0.0, null, null);

                if (_drawWorking)
                {
                    vm.Presenter.DrawContainer(context, vm.WorkingContainer, vm.Renderer, 0.0, 0.0, null, null);
                }

                vm.Presenter.DrawHelpers(context, vm.CurrentContainer, vm.Renderer, 0.0, 0.0);

                if (_drawWorking)
                {
                    vm.Presenter.DrawHelpers(context, vm.WorkingContainer, vm.Renderer, 0.0, 0.0);
                }
            }
        }
    }
}
