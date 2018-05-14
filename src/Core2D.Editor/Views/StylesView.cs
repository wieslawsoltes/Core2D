// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dock.Model;

namespace Core2D.Editor.Views
{
    /// <summary>
    /// Styles view.
    /// </summary>
    public class StylesView : DockView
    {
        private readonly IServiceProvider _serviceProvider;
        private Lazy<ProjectEditor> _context;

        /// <inheritdoc/>
        public override string Title => "Styles";

        /// <inheritdoc/>
        public override object Context => _context.Value;

        /// <summary>
        /// Initialize new instance of <see cref="StylesView"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public StylesView(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _context = serviceProvider.GetServiceLazily<ProjectEditor>();
        }
    }
}
