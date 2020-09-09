#nullable disable

namespace Core2D.Renderer
{
    /// <summary>
    /// Generic key value cache contract.
    /// </summary>
    /// <typeparam name="TKey">The input type.</typeparam>
    /// <typeparam name="TValue">The output type.</typeparam>
    public interface ICache<TKey, TValue>
    {
        /// <summary>
        /// Gets value from storage.
        /// </summary>
        /// <param name="key">The key object.</param>
        /// <returns>The value from storage.</returns>
        TValue Get(TKey key);

        /// <summary>
        /// Sets or adds new value to storage.
        /// </summary>
        /// <param name="key">The key object.</param>
        /// <param name="value">The value object.</param>
        void Set(TKey key, TValue value);

        /// <summary>
        /// Resets cache storage.
        /// </summary>
        void Reset();
    }
}
