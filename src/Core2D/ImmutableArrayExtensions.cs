using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Immutable array extension methods.
    /// </summary>
    public static class ImmutableArrayExtensions
    {
        /// <summary>
        /// Copies immutable array with shared objects.
        /// </summary>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <param name="array">The array to copy.</param>
        /// <param name="shared">The shared objects dictionary.</param>
        /// <returns>The copy of immutable array.</returns>
        public static ImmutableArray<T>.Builder Copy<T>(this ref ImmutableArray<T> array, IDictionary<object, object> shared) where T : IObservableObject
        {
            var copy = ImmutableArray.CreateBuilder<T>();

            foreach (var item in array)
            {
                copy.Add((T)item.Copy(shared));
            }

            return copy;
        }
    }
}
