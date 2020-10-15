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
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        PathShape ToPathShape(IEnumerable<BaseShape> shapes);

        /// <summary>
        /// Convert shape to path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        PathShape ToPathShape(BaseShape shape);

        /// <summary>
        /// Convert shape to stroke path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        PathShape ToStrokePathShape(BaseShape shape);

        /// <summary>
        /// Convert shape to fill path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        PathShape ToFillPathShape(BaseShape shape);

        /// <summary>
        /// Convert shape to winding path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        PathShape ToWindingPathShape(BaseShape shape);

        /// <summary>
        /// Convert shape to simplified path shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        PathShape Simplify(BaseShape shape);

        /// <summary>
        /// Convert shapes to path shape.
        /// </summary>
        /// <param name="shapes">The shapes to convert.</param>
        /// <param name="op">The convert operation.</param>
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        PathShape Op(IEnumerable<BaseShape> shapes, PathOp op);

        /// <summary>
        /// Creates a path based on the SVG path data string.
        /// </summary>
        /// <param name="svgPath">The SVG path data.</param>
        /// <param name="isStroked">The flag indicating whether path is stroked.</param>
        /// <param name="isFilled">The flag indicating whether path is filled.</param>
        /// <returns>The new instance of object of type <see cref="PathShape"/>.</returns>
        public PathShape FromSvgPathData(string svgPath, bool isStroked, bool isFilled);

        /// <summary>
        /// Returns a SVG path data representation of the shape.
        /// </summary>
        /// <param name="shape">The shape to convert.</param>
        /// <returns>The SVG path data.</returns>
        public string ToSvgPathData(BaseShape shape);
    }
}
