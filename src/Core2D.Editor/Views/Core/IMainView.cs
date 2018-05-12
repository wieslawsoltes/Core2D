// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor.Views.Core
{
    /// <summary>
    /// Main view contract.
    /// </summary>
    public interface IMainView
    {
        /// <summary>
        /// Gets main view context.
        /// </summary>
        object Context { get; }

        /// <summary>
        /// Presents view.
        /// </summary>
        void Present();

        /// <summary>
        /// Destroys view.
        /// </summary>
        void Destroy();
    }
}
