using Core2D.Path;

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines path shape contract.
    /// </summary>
    public interface IPathShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets path geometry used to draw shape.
        /// </summary>
        /// <remarks>
        /// Path geometry is based on path markup syntax:
        /// - Xaml abbreviated geometry https://msdn.microsoft.com/en-us/library/ms752293(v=vs.110).aspx
        /// - Svg path specification http://www.w3.org/TR/SVG11/paths.html
        /// </remarks>
        IPathGeometry Geometry { get; set; }
    }
}
