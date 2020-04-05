
namespace Core2D.Style
{
    /// <summary>
    /// Defines ARGB color contract.
    /// </summary>
    public interface IArgbColor : IColor, IStringExporter
    {
        /// <summary>
        /// Alpha color channel.
        /// </summary>
        byte A { get; set; }

        /// <summary>
        /// Red color channel.
        /// </summary>
        byte R { get; set; }

        /// <summary>
        /// Green color channel.
        /// </summary>
        byte G { get; set; }

        /// <summary>
        /// Blue color channel.
        /// </summary>
        byte B { get; set; }
    }
}
