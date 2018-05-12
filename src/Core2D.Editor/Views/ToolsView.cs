// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Views.Core;

namespace Core2D.Editor.Views
{
    /// <summary>
    /// Tools view.
    /// </summary>
    public class ToolsView : ViewBase
    {
        private readonly IServiceProvider _serviceProvider;
        private Lazy<ProjectEditor> _context;

        /// <inheritdoc/>
        public override string Title => "Tools";

        /// <inheritdoc/>
        public override object Context => _context.Value;

        /// <summary>
        /// Initialize new instance of <see cref="ToolsView"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolsView(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _context = serviceProvider.GetServiceLazily<ProjectEditor>();
        }
    }
}
