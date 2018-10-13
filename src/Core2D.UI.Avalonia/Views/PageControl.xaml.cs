// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.UI.Avalonia.Utilities;

namespace Core2D.UI.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="PageControl"/> xaml.
    /// </summary>
    public class PageControl : UserControl
    {
        private ProjectEditor _projectEditor;
        private InputProcessor _inputProcessor;
        private ContainerControl _containerControlData;
        private ContainerControl _containerControlTemplate;
        private ContainerControl _containerControlEditor;
        private ZoomBorder _zoomBorder;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageControl"/> class.
        /// </summary>
        public PageControl()
        {
            InitializeComponent();

            this.GetObservable(DataContextProperty).Subscribe((value) =>
            {
                DetachEditor();
                AttachEditor();
            });

            AttachedToVisualTree += (sender, e) =>
            {
                DetachEditor();
                AttachEditor();
            };

            DetachedFromVisualTree += (sender, e) =>
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

        /// <summary>
        /// Attach project editor to container control.
        /// </summary>
        public void AttachEditor()
        {
            _projectEditor = DataContext as ProjectEditor;
            _containerControlData = this.Find<ContainerControl>("containerControlData");
            _containerControlTemplate = this.Find<ContainerControl>("containerControlTemplate");
            _containerControlEditor = this.Find<ContainerControl>("containerControlEditor");
            _zoomBorder = this.Find<ZoomBorder>("zoomBorder");

            if (_projectEditor != null && _containerControlEditor != null && _zoomBorder != null)
            {
                _projectEditor.CanvasPlatform.Invalidate = () =>
                {
                    _containerControlData.InvalidateVisual();
                    _containerControlTemplate.InvalidateVisual();
                    _containerControlEditor.InvalidateVisual();
                };
                _projectEditor.CanvasPlatform.ResetZoom = () => _zoomBorder.Reset();
                _projectEditor.CanvasPlatform.AutoFitZoom = () => _zoomBorder.AutoFit();
                _projectEditor.CanvasPlatform.Zoom = _zoomBorder;

                _zoomBorder.InvalidatedChild = InvalidateChild;

                _inputProcessor = new InputProcessor(
                    new AvaloniaInputSource(
                        _zoomBorder,
                        _containerControlEditor,
                        _zoomBorder.FixInvalidPointPosition), 
                    _projectEditor);
            }
        }

        /// <summary>
        /// Detach project editor from container control.
        /// </summary>
        public void DetachEditor()
        {
            if (_projectEditor?.CanvasPlatform != null)
            {
                _projectEditor.CanvasPlatform.Invalidate = null;
                _projectEditor.CanvasPlatform.ResetZoom = null;
                _projectEditor.CanvasPlatform.AutoFitZoom = null;
                _projectEditor.CanvasPlatform.Zoom = null;
            }

            if (_zoomBorder != null)
            {
                _zoomBorder.InvalidatedChild = null;
            }

            _inputProcessor?.Dispose();

            _projectEditor = null;
            _containerControlData = null;
            _containerControlTemplate = null;
            _containerControlEditor = null;
            _zoomBorder = null;
        }
    }
}
