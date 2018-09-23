// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Editor.Tools.Settings;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// None tool.
    /// </summary>
    public class ToolNone : ToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsNone _settings;

        /// <inheritdoc/>
        public override string Title => "None";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsNone Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolNone"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolNone(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsNone();
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
