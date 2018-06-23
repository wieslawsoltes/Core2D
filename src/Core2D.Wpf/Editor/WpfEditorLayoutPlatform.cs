// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor;

namespace Core2D.Wpf.Editor
{
    /// <summary>
    /// Editor layout Avalonia platform.
    /// </summary>
    public class WpfEditorLayoutPlatform : ObservableObject, IEditorLayoutPlatform
    {
        private readonly IServiceProvider _serviceProvider;

        private Action _loadLayout;
        private Action _saveLayout;
        private Action _resetLayout;

        /// <summary>
        /// Initialize new instance of <see cref="WpfEditorLayoutPlatform"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public WpfEditorLayoutPlatform(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public Action LoadLayout
        {
            get => _loadLayout;
            set => Update(ref _loadLayout, value);
        }

        /// <inheritdoc/>
        public Action SaveLayout
        {
            get => _saveLayout;
            set => Update(ref _saveLayout, value);
        }

        /// <inheritdoc/>
        public Action ResetLayout
        {
            get => _resetLayout;
            set => Update(ref _resetLayout, value);
        }
    }
}
