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
        private AvaloniaInputSource _inputSource = null;
        private InputProcessor _inputProcessor = null;
        private bool _isLoaded = false;
        private ContainerControl _containerControlData = null;
        private ContainerControl _containerControlTemplate = null;
        private ContainerControl _containerControlEditor = null;
        private ZoomBorder _zoomBorder = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageControl"/> class.
        /// </summary>
        public PageControl()
        {
            InitializeComponent();

            _containerControlData = this.Find<ContainerControl>("containerControlData");
            _containerControlTemplate = this.Find<ContainerControl>("containerControlTemplate");
            _containerControlEditor = this.Find<ContainerControl>("containerControlEditor");
            _zoomBorder = this.Find<ZoomBorder>("zoomBorder");

            //this.GetObservable(DataContextProperty).Subscribe((value) =>
            //{
            //    DetachEditor();
            //    AttachEditor();
            //});
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Console.WriteLine("PageControl_AttachedToVisualTree");
            DetachEditor();
            AttachEditor();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            Console.WriteLine("PageControl_DetachedFromVisualTree");
            DetachEditor();
        }

        private void InvalidateChild(double zoomX, double zoomY, double offsetX, double offsetY)
        {
            if (DataContext is ProjectEditor projectEditor)
            {
                var state = projectEditor.Renderers[0]?.State;
                if (state != null)
                {
                    bool invalidateCache = state.ZoomX != zoomX || state.ZoomY != zoomY;

                    state.ZoomX = zoomX;
                    state.ZoomY = zoomY;
                    state.PanX = offsetX;
                    state.PanY = offsetY;

                    if (invalidateCache)
                    {
                        projectEditor.OnInvalidateCache(isZooming: true);
                    }
                }
            }
        }

        /// <summary>
        /// Attach project editor to container control.
        /// </summary>
        public void AttachEditor()
        {
            if (DataContext is ProjectEditor projectEditor)
            {
                Console.WriteLine($"AttachEditor _isLoaded: {_isLoaded}");
                if (_isLoaded)
                    return;
                else
                    _isLoaded = true;

                Console.WriteLine("AttachEditor");

                if (projectEditor.CanvasPlatform is IEditorCanvasPlatform canvasPlatform)
                {
                    canvasPlatform.Invalidate = () =>
                    {
                        _containerControlData?.InvalidateVisual();
                        _containerControlTemplate?.InvalidateVisual();
                        _containerControlEditor?.InvalidateVisual();
                    };
                    canvasPlatform.ResetZoom = () => _zoomBorder?.Reset();
                    canvasPlatform.AutoFitZoom = () => _zoomBorder?.AutoFit();
                    canvasPlatform.Zoom = _zoomBorder;
                }

                if (_zoomBorder != null)
                {
                    _zoomBorder.InvalidatedChild = InvalidateChild;
                }

                Console.WriteLine("_inputProcessor?.Dispose();");
                _inputProcessor?.Dispose();
                _inputProcessor = null;

                _inputSource = new AvaloniaInputSource(
                        _zoomBorder,
                        _containerControlEditor,
                        _zoomBorder.FixInvalidPointPosition);

                _inputProcessor = new InputProcessor(_inputSource, projectEditor);

                Console.WriteLine("_inputProcessor = new InputProcessor(_inputSource, projectEditor);");
            }
        }

        /// <summary>
        /// Detach project editor from container control.
        /// </summary>
        public void DetachEditor()
        {
            if (DataContext is ProjectEditor projectEditor)
            {
                Console.WriteLine($"DetachEditor _isLoaded: {_isLoaded}");
                if (!_isLoaded)
                    return;
                else
                    _isLoaded = false;

                Console.WriteLine("DetachEditor");

                if (projectEditor.CanvasPlatform is IEditorCanvasPlatform canvasPlatform)
                {
                    canvasPlatform.Invalidate = null;
                    canvasPlatform.ResetZoom = null;
                    canvasPlatform.AutoFitZoom = null;
                    canvasPlatform.Zoom = null;
                }

                if (_zoomBorder != null)
                {
                    _zoomBorder.InvalidatedChild = null;
                }

                _inputProcessor?.Dispose();
                _inputProcessor = null;

                Console.WriteLine("_inputProcessor?.Dispose();");
            }
        }
    }
}
