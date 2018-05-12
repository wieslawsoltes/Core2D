// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Views.Core;

namespace Core2D.Editor.Views
{
    /// <summary>
    /// Dashboard view.
    /// </summary>
    public class DashboardView : ViewBase
    {
        private readonly IServiceProvider _serviceProvider;
        private Lazy<ProjectEditor> _context;

        /// <inheritdoc/>
        public override string Title => "Dashboard";

        /// <inheritdoc/>
        public override object Context => _context.Value;

        /// <summary>
        /// Initialize new instance of <see cref="DashboardView"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DashboardView(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _context = serviceProvider.GetServiceLazily<ProjectEditor>();
        }
    }
}
