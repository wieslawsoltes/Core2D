using System;
using System.Collections.Generic;
using Core2D.Editor;

namespace Core2D.Editor
{
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

        public Action InvalidateControl
        {
            get => _invalidateControl;
            set => RaiseAndSetIfChanged(ref _invalidateControl, value);
        }

        public Action ResetZoom
        {
            get => _resetZoom;
            set => RaiseAndSetIfChanged(ref _resetZoom, value);
        }

        public Action FillZoom
        {
            get => _fillZoom;
            set => RaiseAndSetIfChanged(ref _fillZoom, value);
        }

        public Action UniformZoom
        {
            get => _uniformZoom;
            set => RaiseAndSetIfChanged(ref _uniformZoom, value);
        }

        public Action UniformToFillZoom
        {
            get => _uniformToFillZoom;
            set => RaiseAndSetIfChanged(ref _uniformToFillZoom, value);
        }

        public Action AutoFitZoom
        {
            get => _autoFitZoom;
            set => RaiseAndSetIfChanged(ref _autoFitZoom, value);
        }

        public Action InZoom
        {
            get => _inZoom;
            set => RaiseAndSetIfChanged(ref _inZoom, value);
        }

        public Action OutZoom
        {
            get => _outZoom;
            set => RaiseAndSetIfChanged(ref _outZoom, value);
        }

        public object Zoom
        {
            get => _zoom;
            set => RaiseAndSetIfChanged(ref _zoom, value);
        }

        public AvaloniaEditorCanvasPlatform(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
