
namespace Core2D.Editor.Input
{
    /// <summary>
    /// Input arguments.
    /// </summary>
    public struct InputArgs
    {
        /// <summary>
        /// Gets input X position.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets input Y position.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets input modifier flags.
        /// </summary>
        public ModifierFlags Modifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputArgs"/> struct.
        /// </summary>
        /// <param name="x">The input X position.</param>
        /// <param name="y">The input Y position.</param>
        /// <param name="modifier">The input modifier flags.</param>
        public InputArgs(double x, double y, ModifierFlags modifier)
        {
            X = x;
            Y = y;
            Modifier = modifier;
        }

        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }

        public void Deconstruct(out double x, out double y, out ModifierFlags modifier)
        {
            x = X;
            y = Y;
            modifier = Modifier;
        }
    }
}
