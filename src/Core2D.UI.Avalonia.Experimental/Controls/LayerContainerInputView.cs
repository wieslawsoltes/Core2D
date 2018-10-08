// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Core2D.UI.Avalonia.Renderers;
using Core2D.Editor;
using Core2D.ViewModels.Containers;

namespace Core2D.UI.Avalonia.Controls
{
    public class LayerContainerInputView : Border
    {
        public static readonly StyledProperty<IVisual> RelativeToProperty =
            AvaloniaProperty.Register<LayerContainerInputView, IVisual>(nameof(RelativeTo));

        public IVisual RelativeTo
        {
            get { return GetValue(RelativeToProperty); }
            set { SetValue(RelativeToProperty, value); }
        }

        public LayerContainerInputView()
        {
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        private Modifier GetModifier(InputModifiers inputModifiers)
        {
            Modifier modifier = Modifier.None;

            if (inputModifiers.HasFlag(InputModifiers.Alt))
            {
                modifier |= Modifier.Alt;
            }

            if (inputModifiers.HasFlag(InputModifiers.Control))
            {
                modifier |= Modifier.Control;
            }

            if (inputModifiers.HasFlag(InputModifiers.Shift))
            {
                modifier |= Modifier.Shift;
            }

            return modifier;
        }

        private Point AdjustGetPosition(Point point)
        {
            if (RelativeTo?.RenderTransform != null)
            {
                return MatrixHelper.TransformPoint(RelativeTo.RenderTransform.Value.Invert(), point);
            }
            return point;
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is LayerContainerViewModel vm)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    vm.CurrentTool.LeftDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is LayerContainerViewModel vm)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    vm.CurrentTool.RightDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is LayerContainerViewModel vm)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    if (vm.Mode == EditMode.Mouse)
                    {
                        vm.CurrentTool.LeftUp(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    }
                    else if (vm.Mode == EditMode.Touch)
                    {
                        vm.CurrentTool.LeftDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is LayerContainerViewModel vm)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    vm.CurrentTool.RightUp(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (this.DataContext is LayerContainerViewModel vm)
            {
                var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                vm.CurrentTool.Move(vm, point.X, point.Y, GetModifier(e.InputModifiers));
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                if (vm.CurrentContainer.WorkBackground != null)
                {
                    var color = AvaloniaBrushCache.FromDrawColor(vm.CurrentContainer.InputBackground);
                    var brush = new SolidColorBrush(color);
                    context.FillRectangle(brush, new Rect(0, 0, Bounds.Width, Bounds.Height));
                }
            }
        }
    }
}
