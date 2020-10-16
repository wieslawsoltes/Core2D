using System;
using System.Collections.Generic;
using Core2D.Editor.Tools.Settings;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// None tool.
    /// </summary>
    public class ToolNone : ObservableObject, IEditorTool
    {
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsNone _settings;

        /// <inheritdoc/>
        public string Title => "None";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsNone Settings
        {
            get => _settings;
            set => RaiseAndSetIfChanged(ref _settings, value);
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

        /// <inheritdoc/>
        public void LeftDown(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void LeftUp(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void RightDown(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void RightUp(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void Move(InputArgs args)
        {
        }

        /// <inheritdoc/>
        public void Move(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Finalize(BaseShape shape)
        {
        }

        /// <inheritdoc/>
        public void Reset()
        {
        }
    }
}
