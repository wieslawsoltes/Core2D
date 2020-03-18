using System;
using System.Collections.Generic;
using Core2D.Editor;

namespace Core2D.UI.Avalonia.Editor
{
    /// <summary>
    /// Editor layout Avalonia platform.
    /// </summary>
    public class AvaloniaEditorLayoutPlatform : ObservableObject, IEditorLayoutPlatform
    {
        private readonly IServiceProvider _serviceProvider;

        private Action _loadLayout;
        private Action _saveLayout;
        private Action _resetLayout;

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

        /// <summary>
        /// Initialize new instance of <see cref="AvaloniaEditorLayoutPlatform"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaEditorLayoutPlatform(IServiceProvider serviceProvider)
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
