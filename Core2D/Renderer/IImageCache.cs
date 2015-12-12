// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public interface IImageCache
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<ImageKey> Keys { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        string AddImageFromFile(string path, byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bytes"></param>
        void AddImage(string key, byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetImage(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void RemoveImage(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="used"></param>
        void PurgeUnusedImages(ICollection<string> used);
    }
}
