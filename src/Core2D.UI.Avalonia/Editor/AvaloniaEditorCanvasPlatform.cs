// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Editor;

namespace Core2D.UI.Avalonia.Editor
{
    /// <summary>
    /// Editor canvas Avalonia platform.
    /// </summary>
    public class AvaloniaEditorCanvasPlatform : ObservableObject, IEditorCanvasPlatform
    {
        private readonly IServiceProvider _serviceProvider;

        private Action _invalidate;
        private Action _resetZoom;
        private Action _extentZoom;
        private object _zoom;

        /// <inheritdoc/>
        public Action Invalidate
        {
            get => _invalidate;
            set => Update(ref _invalidate, value);
        }

        /// <inheritdoc/>
        public Action ResetZoom
        {
            get => _resetZoom;
            set => Update(ref _resetZoom, value);
        }

        /// <inheritdoc/>
        public Action AutoFitZoom
        {
            get => _extentZoom;
            set => Update(ref _extentZoom, value);
        }

        /// <inheritdoc/>
        public object Zoom
        {
            get => _zoom;
            set => Update(ref _zoom, value);
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
