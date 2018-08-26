// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D.Renderer
{
    /// <summary>
    /// Defines key based image data cache contract.
    /// </summary>
    public interface IImageCache
    {
        /// <summary>
        /// Gets image keys collection.
        /// </summary>
        IEnumerable<IImageKey> Keys { get; }

        /// <summary>
        /// Add image key using file name as key.
        /// </summary>
        /// <param name="path">The image file path.</param>
        /// <param name="bytes">The image data.</param>
        /// <returns>The image key.</returns>
        string AddImageFromFile(string path, byte[] bytes);

        /// <summary>
        /// Add image key.
        /// </summary>
        /// <param name="key">The image key.</param>
        /// <param name="bytes">The image data.</param>
        void AddImage(string key, byte[] bytes);

        /// <summary>
        /// Get image data.
        /// </summary>
        /// <param name="key">The image key.</param>
        /// <returns>The image data.</returns>
        byte[] GetImage(string key);

        /// <summary>
        /// Remove image key.
        /// </summary>
        /// <param name="key">The image key.</param>
        void RemoveImage(string key);

        /// <summary>
        /// Removed unused image keys.
        /// </summary>
        /// <param name="used">The used keys collection.</param>
        void PurgeUnusedImages(ICollection<string> used);
    }
}
