using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D
{
    /// <summary>
    /// Defines selection contract.
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Currently hovered shape.
        /// </summary>
        IBaseShape HoveredShape { get; set; }

        /// <summary>
        /// Currently selected shape.
        /// </summary>
        IBaseShape SelectedShape { get; set; }

        /// <summary>
        /// Currently selected shapes.
        /// </summary>
        ISet<IBaseShape> SelectedShapes { get; set; }
    }
}
