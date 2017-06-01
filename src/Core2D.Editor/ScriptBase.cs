// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor
{
    /// <summary>
    /// Script base class.
    /// </summary>
    public abstract class ScriptBase
    {
        /// <summary>
        /// Gets or sets project editor.
        /// </summary>
        public ProjectEditor Editor { get; set; }

        /// <summary>
        /// Runs script.
        /// </summary>
        public abstract void Run();
    }
}
