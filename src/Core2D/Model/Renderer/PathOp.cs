
namespace Core2D.Renderer
{
    /// <summary>
    /// Specifies path op.
    /// </summary>
    public enum PathOp
    {
        /// <summary>
        /// Difference.
        /// </summary>
        Difference = 0,

        /// <summary>
        /// Intersect.
        /// </summary>
        Intersect = 1,

        /// <summary>
        /// Union.
        /// </summary>
        Union = 2,

        /// <summary>
        /// Xor.
        /// </summary>
        Xor = 3,

        /// <summary>
        /// Reverse difference,
        /// </summary>
        ReverseDifference = 4
    }
}
