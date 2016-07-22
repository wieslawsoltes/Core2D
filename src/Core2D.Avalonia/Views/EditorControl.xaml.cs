// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="EditorControl"/> xaml.
    /// </summary>
    public class EditorControl : UserControl
    {
        private ProjectEditor _projectEditor;
        private ContainerViewControl _containerControl;
        private ZoomBorder _zoomBorder;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorControl"/> class.
        /// </summary>
        public EditorControl()
        {
            this.InitializeComponent();

            this.GetObservable(DataContextProperty).Subscribe((value) =>
            {
                DetachEditor();
                AttachEditor();
            });

            this.AttachedToVisualTree += (sender, e) =>
            {
                DetachEditor();
                AttachEditor();
            };

            this.DetachedFromVisualTree += (sender, e) =>
            {
                DetachEditor();
            };
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InvalidateChild(double zoomX, double zoomY, double offsetX, double offsetY)
        {
            if (_projectEditor != null)
            {
                var state = _projectEditor?.Renderers[0]?.State;
                if (state != null)
                {
                    bool invalidateCache = state.ZoomX != zoomX || state.ZoomY != zoomY;

                    state.ZoomX = zoomX;
                    state.ZoomY = zoomY;
                    state.PanX = offsetX;
                    state.PanY = offsetY;

                    if (invalidateCache)
                    {
                        _projectEditor.OnInvalidateCache(isZooming: true);
                    }
                }
            }
        }

        private void ZoomBorder_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var p = _zoomBorder.FixInvalidPointPosition(e.GetPosition(_containerControl));

            if (e.MouseButton == MouseButton.Left)
            {
                if (_projectEditor.IsLeftDownAvailable())
                {
                    _projectEditor.LeftDown(p.X, p.Y);
                }
            }

            if (e.MouseButton == MouseButton.Right)
            {
                if (_projectEditor.IsRightDownAvailable())
                {
                    _projectEditor.RightDown(p.X, p.Y);
                }
            }
        }

        private void ZoomBorder_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var p = _zoomBorder.FixInvalidPointPosition(e.GetPosition(_containerControl));

            if (e.MouseButton == MouseButton.Left)
            {
                if (_projectEditor.IsLeftUpAvailable())
                {
                    _projectEditor.LeftUp(p.X, p.Y);
                }
            }

            if (e.MouseButton == MouseButton.Right)
            {
                if (_projectEditor.IsRightUpAvailable())
                {
                    _projectEditor.RightUp(p.X, p.Y);
                }
            }
        }

        private void ZoomBorder_PointerMoved(object sender, PointerEventArgs e)
        {
            var p = _zoomBorder.FixInvalidPointPosition(e.GetPosition(_containerControl));

            if (_projectEditor.IsMoveAvailable())
            {
                _projectEditor.Move(p.X, p.Y);
            }
        }

        /// <summary>
        /// Attach project editor to container control.
        /// </summary>
        public void AttachEditor()
        {
            _projectEditor = this.DataContext as ProjectEditor;
            _containerControl = this.Find<ContainerViewControl>("containerControl");
            _zoomBorder = this.Find<ZoomBorder>("zoomBorder");

            if (_projectEditor != null && _containerControl != null && _zoomBorder != null)
            {
                _projectEditor.Invalidate = () => _containerControl.InvalidateVisual();
                _projectEditor.ResetZoom = () => _zoomBorder.Reset();
                _projectEditor.AutoFitZoom = () => _zoomBorder.AutoFit();
                _projectEditor.LoadLayout = () => { };
                _projectEditor.SaveLayout = () => { };
                _projectEditor.ResetLayout = () => { };

                _zoomBorder.InvalidatedChild = InvalidateChild;
                _zoomBorder.PointerPressed += ZoomBorder_PointerPressed;
                _zoomBorder.PointerReleased += ZoomBorder_PointerReleased;
                _zoomBorder.PointerMoved += ZoomBorder_PointerMoved;
            }
        }

        /// <summary>
        /// Detach project editor from container control.
        /// </summary>
        public void DetachEditor()
        {
            if (_projectEditor != null && _containerControl != null && _zoomBorder != null)
            {
                _projectEditor.Invalidate = null;
                _projectEditor.ResetZoom = null;
                _projectEditor.AutoFitZoom = null;
                _projectEditor.LoadLayout = null;
                _projectEditor.SaveLayout = null;
                _projectEditor.ResetLayout = null;

                _zoomBorder.InvalidatedChild = null;
                _zoomBorder.PointerPressed -= ZoomBorder_PointerPressed;
                _zoomBorder.PointerReleased -= ZoomBorder_PointerReleased;
                _zoomBorder.PointerMoved -= ZoomBorder_PointerMoved;
            }

            _projectEditor = null;
            _containerControl = null;
            _zoomBorder = null;
        }
    }
}
