
namespace Core2D
{
    /// <summary>
    /// Defines selectable shape contract.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Moves shape to new position using X and Y axis offset.
        /// </summary>
        /// <param name="selection">The selection state.</param>
        /// <param name="dx">The X axis position offset.</param>
        /// <param name="dy">The Y axis position offset.</param>
        void Move(ISelection? selection, decimal dx, decimal dy);

        /// <summary>
        /// Selects the shape.
        /// </summary>
        /// <param name="selection">The selection state.</param>
        void Select(ISelection selection);

        /// <summary>
        /// Deselects the shape.
        /// </summary>
        /// <param name="selection">The selection state.</param>
        void Deselect(ISelection selection);
    }
}
