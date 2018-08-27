// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading.Tasks;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines image key importer contract.
    /// </summary>
    public interface IImageImporter
    {
        /// <summary>
        /// Get the image key.
        /// </summary>
        /// <returns>The image key.</returns>
        Task<string> GetImageKeyAsync();
    }
}
