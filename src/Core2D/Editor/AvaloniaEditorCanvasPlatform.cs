using System;
using System.Collections.Generic;
using Core2D.Editor;

namespace Core2D.Editor
{
    /// <summary>
    /// Editor canvas Avalonia platform.
    /// </summary>
    public class AvaloniaEditorCanvasPlatform : ObservableObject, IEditorCanvasPlatform
    {
        private readonly IServiceProvider _serviceProvider;

        private Action _invalidateControl;
        private Action _resetZoom;
        private Action _fillZoom;
        private Action _uniformZoom;
        private Action _uniformToFillZoom;
        private Action _autoFitZoom;
        private Action _inZoom;
        private Action _outZoom;
        private object _zoom;

        /// <inheritdoc/>
        public Action InvalidateControl
        {
            get => _invalidateControl;
            set => RaiseAndSetIfChanged(ref _invalidateControl, value);
        }

        /// <inheritdoc/>
        public Action ResetZoom
        {
            get => _resetZoom;
            set => RaiseAndSetIfChanged(ref _resetZoom, value);
        }

        /// <inheritdoc/>
        public Action FillZoom
        {
            get => _fillZoom;
            set => RaiseAndSetIfChanged(ref _fillZoom, value);
        }

        /// <inheritdoc/>
        public Action UniformZoom
        {
            get => _uniformZoom;
            set => RaiseAndSetIfChanged(ref _uniformZoom, value);
        }

        /// <inheritdoc/>
        public Action UniformToFillZoom
        {
            get => _uniformToFillZoom;
            set => RaiseAndSetIfChanged(ref _uniformToFillZoom, value);
        }

        /// <inheritdoc/>
        public Action AutoFitZoom
        {
            get => _autoFitZoom;
            set => RaiseAndSetIfChanged(ref _autoFitZoom, value);
        }

        /// <inheritdoc/>
        public Action InZoom
        {
            get => _inZoom;
            set => RaiseAndSetIfChanged(ref _inZoom, value);
        }

        /// <inheritdoc/>
        public Action OutZoom
        {
            get => _outZoom;
            set => RaiseAndSetIfChanged(ref _outZoom, value);
        }

        /// <inheritdoc/>
        public object Zoom
        {
            get => _zoom;
            set => RaiseAndSetIfChanged(ref _zoom, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="AvaloniaEditorCanvasPlatform"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaEditorCanvasPlatform(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
