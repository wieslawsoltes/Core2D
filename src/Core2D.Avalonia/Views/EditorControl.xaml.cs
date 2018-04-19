// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.Utilities.Avalonia;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="EditorControl"/> xaml.
    /// </summary>
    public class EditorControl : UserControl
    {
        private ProjectEditor _projectEditor;
        private InputProcessor _inputProcessor;
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

                _zoomBorder.InvalidatedChild = InvalidateChild;

                _inputProcessor = new InputProcessor(
                    new AvaloniaInputSource(
                        _zoomBorder,
                        _containerControl,
                        _zoomBorder.FixInvalidPointPosition), 
                    _projectEditor);
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

                _zoomBorder.InvalidatedChild = null;

                _inputProcessor.Dispose();
            }

            _projectEditor = null;
            _containerControl = null;
            _zoomBorder = null;
        }
    }
}
