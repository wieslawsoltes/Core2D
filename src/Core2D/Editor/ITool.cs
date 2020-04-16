using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines tool contract.
    /// </summary>
    public interface ITool : IObservableObject
    {
        /// <summary>
        /// Gets the tool title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Handle mouse left button down events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void LeftDown(InputArgs args);

        /// <summary>
        /// Handle mouse left button up events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void LeftUp(InputArgs args);

        /// <summary>
        /// Handle mouse right button down events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void RightDown(InputArgs args);

        /// <summary>
        /// Handle mouse right button up events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void RightUp(InputArgs args);

        /// <summary>
        /// Handle mouse move events.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        void Move(InputArgs args);

        /// <summary>
        /// Move edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        void Move(IBaseShape shape);

        /// <summary>
        /// Finalize edited shape.
        /// </summary>
        /// <param name="shape">The shape object.</param>
        void Finalize(IBaseShape shape);

        /// <summary>
        /// Reset tool.
        /// </summary>
        void Reset();
    }
}
