// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines image shape contract.
    /// </summary>
    public interface IImageShape : ITextShape
    {
        /// <summary>
        /// Gets or sets image key used to retrieve bytes from image cache repository.
        /// </summary>
        string Key { get; set; }
    }
}
