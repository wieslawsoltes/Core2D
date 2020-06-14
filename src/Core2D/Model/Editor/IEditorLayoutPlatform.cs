
namespace Core2D.Editor
{
    /// <summary>
    /// Defines editor layout platform contract.
    /// </summary>
    public interface IEditorLayoutPlatform
    {
        /// <summary>
        /// Gets or sets current layout configuration.
        /// </summary>
        object Layout { get; set; }

        /// <summary>
        /// Serializes layout configuration.
        /// </summary>
        /// <param name="path">The layout configuration file path.</param>
        /// <param name="layout">The layout configuration.</param>
        void SerializeLayout(string path, object layout);

        /// <summary>
        /// Deserializes layout configuration.
        /// </summary>
        /// <param name="path">The layout configuration file path.</param>
        /// <returns>The layout configuration.</returns>
        object DeserializeLayout(string path);

        /// <summary>
        /// Navigate to view.
        /// </summary>
        /// <param name="view">The view to navigate to.</param>
        void Navigate(object view);

        /// <summary>
        /// Saves layout configuration.
        /// </summary>
        void SaveLayout();

        /// <summary>
        /// Applies layout configuration.
        /// </summary>
        /// <param name="layout">The layout configuration.</param>
        void ApplyLayout(object layout);

        /// <summary>
        /// Removes layout configuration.
        /// </summary>
        /// <param name="layout">The layout configuration.</param>
        void DeleteLayout(object layout);

        /// <summary>
        /// Resets layout configuration.
        /// </summary>
        void ResetLayout();

        /// <summary>
        /// Manages layout configurations.
        /// </summary>
        void ManageLayouts();

        /// <summary>
        /// Imports layout configurations.
        /// </summary>
        void ImportLayouts();

        /// <summary>
        /// Exports layout configurations.
        /// </summary>
        void ExportLayouts();
    }
}
