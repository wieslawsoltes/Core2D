using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;

namespace Core2D
{
    /// <summary>
    /// Defines path converter contract.
    /// </summary>
    public interface IPathConverter
    {
        /// <summary>
        /// Convert shapes to path shape.
        /// </summary>
        /// <param name="shapes">The shapes to convert.</param>
        /// <returns>The new instance of object of type <see cref="IPathShape"/>.</returns>
        IPathShape ToPathShape(IEnumerable<IBaseShape> shapes);

        /// <summary>
        /// Convert shape to path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="IPathShape"/>.</returns>
        IPathShape ToPathShape(IBaseShape shape);

        /// <summary>
        /// Convert shape to stroke path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="IPathShape"/>.</returns>
        IPathShape ToStrokePathShape(IBaseShape shape);

        /// <summary>
        /// Convert shape to fill path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="IPathShape"/>.</returns>
        IPathShape ToFillPathShape(IBaseShape shape);

        /// <summary>
        /// Convert shapes to path shape.
        /// </summary>
        /// <param name="shapes">The shapes to convert.</param>
        /// <param name="op">The convert operation.</param>
        /// <returns>The new instance of object of type <see cref="IPathShape"/>.</returns>
        IPathShape Op(IEnumerable<IBaseShape> shapes, PathOp op);
    }
}
