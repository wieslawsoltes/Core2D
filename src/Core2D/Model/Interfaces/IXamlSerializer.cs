
namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines xaml string serializer contract.
    /// </summary>
    public interface IXamlSerializer
    {
        /// <summary>
        /// Serialize the object value to xaml string.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <returns>The new instance of object of type <see cref="string"/>.</returns>
        string Serialize<T>(T value);

        /// <summary>
        /// Deserialize the xaml string to object.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="xaml">The xaml string.</param>
        /// <returns>The new instance of object of type <typeparamref name="T"/>.</returns>
        T Deserialize<T>(string xaml);
    }
}
